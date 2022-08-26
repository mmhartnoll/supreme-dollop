using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Constructors
{
    internal class DataContextConstructorDeclaration : ConstructorDeclaration
    {
        private readonly DataModel dataModel;

        public DataContextConstructorDeclaration(DataModel dataModel) 
            : base("AppDataContext", MemberAccessModifiers.Private)
        {
            this.dataModel = dataModel;

            AddParameter(typeof(string), "connectionString");

            AddBaseConstructorArguments("connectionString");
        }

        public static DataContextConstructorDeclaration Create(DataModel dataModel)
            => new DataContextConstructorDeclaration(dataModel);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            foreach (var schemaDefinition in dataModel.Schemata)
            {
                var invocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"{schemaDefinition.Name}Schema.Create"))
                    .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("this")));
                var anonymousMethodExpression = SyntaxFactory.ParenthesizedLambdaExpression()
                    .WithExpressionBody(invocationExpression);

                var lazyCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName($"Lazy<{schemaDefinition.Name}Schema>"))
                    .AddArgumentListArguments(SyntaxFactory.Argument(anonymousMethodExpression));
                var assignmentStatement = SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName($"{schemaDefinition.Name.FormatAsVariableName()}SchemaLoader"),
                    lazyCreationExpression);

                yield return SyntaxFactory.ExpressionStatement(assignmentStatement);
            }
        }
    }
}
