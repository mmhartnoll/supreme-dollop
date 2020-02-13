using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Data.Common;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Methods
{
    internal class CreateDataContextWithTransactionMethodDeclaration : MethodDeclaration
    {
        private CreateDataContextWithTransactionMethodDeclaration(DataModel dataModel) 
            : base("AppDataContext", "Create", MemberAccessModifiers.Public, MemberModifiers.Static)

        {
            AddParameter(typeof(DbConnection), nameof(DbConnection).FormatAsVariableName());
            AddParameter(typeof(DbTransaction), nameof(DbTransaction).FormatAsVariableName());
        }

        public static CreateDataContextWithTransactionMethodDeclaration Create(DataModel dataModel)
            => new CreateDataContextWithTransactionMethodDeclaration(dataModel);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName("AppDataContext"))
                .AddArgumentListArguments(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DbConnection).FormatAsVariableName())),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DbTransaction).FormatAsVariableName())));
            yield return SyntaxFactory.ReturnStatement(objectCreationExpression);
        }
    }
}
