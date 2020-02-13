using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Data.Common;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Methods
{
    internal class CreateDataContextMethodDeclaration : MethodDeclaration
    {
        private CreateDataContextMethodDeclaration(DataModel dataModel) 
            : base("AppDataContext", "Create", MemberAccessModifiers.Public, MemberModifiers.Static)

        {
            AddParameter(typeof(DbConnection), nameof(DbConnection).FormatAsVariableName());
        }

        public static CreateDataContextMethodDeclaration Create(DataModel dataModel)
            => new CreateDataContextMethodDeclaration(dataModel);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName("AppDataContext"))
                .AddArgumentListArguments(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DbConnection).FormatAsVariableName())),
                    SyntaxFactory.Argument(SyntaxFactory.ParseExpression("null")));
            yield return SyntaxFactory.ReturnStatement(objectCreationExpression);
        }
    }
}
