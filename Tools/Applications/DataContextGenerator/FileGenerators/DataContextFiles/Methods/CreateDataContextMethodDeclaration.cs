using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Methods
{
    internal class CreateDataContextMethodDeclaration : MethodDeclaration
    {
        private CreateDataContextMethodDeclaration(DataModel dataModel) 
            : base("AppDataContext", "Create", MemberAccessModifiers.Public, MemberModifiers.Static)

        {
            AddParameter(typeof(string), "connectionString");
        }

        public static CreateDataContextMethodDeclaration Create(DataModel dataModel)
            => new CreateDataContextMethodDeclaration(dataModel);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName("AppDataContext"))
                .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("connectionString")));
            yield return SyntaxFactory.ReturnStatement(objectCreationExpression);
        }
    }
}
