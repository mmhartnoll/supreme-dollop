using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Modelled.Schemas;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.SchemaFiles.Constructors
{
    internal class SchemaConstructorDeclaration : ConstructorDeclaration
    {
        private readonly Schema schemaDefinition;

        private SchemaConstructorDeclaration(Schema schemaDefinition)
            : base($"{schemaDefinition.Name}Schema", MemberAccessModifiers.Private)
        {
            this.schemaDefinition = schemaDefinition;

            AddParameter(typeof(DatabaseContext), nameof(DatabaseContext).FormatAsVariableName());
            AddBaseConstructorArguments(nameof(DatabaseContext).FormatAsVariableName());
        }

        public static SchemaConstructorDeclaration Create(Schema schemaDefinition)
            => new SchemaConstructorDeclaration(schemaDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            foreach (var recordDefinition in schemaDefinition.Records)
            {
                var invocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"{recordDefinition.TableName}Table.Create"))
                    .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DatabaseContext).FormatAsVariableName())));
                var anonymousMethodExpression = SyntaxFactory.ParenthesizedLambdaExpression()
                    .WithExpressionBody(invocationExpression);

                var lazyCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName($"Lazy<{recordDefinition.TableName}Table>"))
                    .AddArgumentListArguments(SyntaxFactory.Argument(anonymousMethodExpression));
                var assignmentStatement = SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName($"{recordDefinition.TableName.FormatAsVariableName()}TableLoader"),
                    lazyCreationExpression);

                yield return SyntaxFactory.ExpressionStatement(assignmentStatement);
            }
        }
    }
}
