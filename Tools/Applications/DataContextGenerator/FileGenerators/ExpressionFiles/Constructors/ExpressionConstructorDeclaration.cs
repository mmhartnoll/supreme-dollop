using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.ExpressionFiles.Constructors
{
    internal class ExpressionConstructorDeclaration : ConstructorDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private ExpressionConstructorDeclaration(RecordDefinition recordDefinition)
            : base($"{recordDefinition.RecordName}Expression", MemberAccessModifiers.Public)
            => this.recordDefinition = recordDefinition;

        public static ExpressionConstructorDeclaration Create(RecordDefinition recordDefinition)
            => new ExpressionConstructorDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                var arg = SyntaxFactory.Argument(SyntaxFactory.ParseExpression($@"""{fieldDefinition.Name}"""));
                var invocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"{fieldDefinition.GetType().Name}Expression.Create"))
                    .AddArgumentListArguments(arg);
                var assignmentExpression = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(fieldDefinition.Name), invocationExpression);
                yield return SyntaxFactory.ExpressionStatement(assignmentExpression);
            }

            foreach (var foreignKey in recordDefinition.ForeignKeys)
            {
                var createMethodIdentifier = SyntaxFactory.IdentifierName($"ParentRecordExpression<{foreignKey.ReferencedKey.RecordDefinition.RecordName}>.Create");
                var getMethodIdentifier = SyntaxFactory.IdentifierName($"Get{foreignKey.ReferencedKey.RecordDefinition.RecordName}Expression");

                var invocationExpression = SyntaxFactory.InvocationExpression(createMethodIdentifier)
                    .AddArgumentListArguments(SyntaxFactory.Argument(getMethodIdentifier));
                var assignmentExpression = SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression, 
                    SyntaxFactory.IdentifierName(foreignKey.ReferencedKey.RecordDefinition.RecordName), 
                    invocationExpression);
                yield return SyntaxFactory.ExpressionStatement(assignmentExpression);
            }
        }
    }
}
