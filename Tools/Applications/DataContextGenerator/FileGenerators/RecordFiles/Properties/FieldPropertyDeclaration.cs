using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Properties
{
    internal class FieldPropertyDeclaration : PropertyDeclaration
    {
        private readonly Field fieldDefinition;

        private FieldPropertyDeclaration(Field fieldDefinition)
                : base(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), fieldDefinition.Name, fieldDefinition.IsReadOnly ? PropertyAccessability.ReadOnly : PropertyAccessability.ReadWrite, MemberAccessModifiers.Public)
            => this.fieldDefinition = fieldDefinition;

        public static FieldPropertyDeclaration Create(Field fieldDefinition)
            => new FieldPropertyDeclaration(fieldDefinition);

        protected override IEnumerable<StatementSyntax> GetGetAccessorStatementSyntaxes()
        {
            if (fieldDefinition.IsReadOnly)
                yield break;
            yield return SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(fieldDefinition.Name.FormatAsVariableName("_")));
        }

        protected override IEnumerable<StatementSyntax> GetSetAccessorStatementSyntaxes()
        {
            if (fieldDefinition.IsReadOnly)
                yield break;

            yield return GetIsModifiedPropertyAssignment();
            yield return GetFieldValueAssignment();

            StatementSyntax GetIsModifiedPropertyAssignment()
            {
                var left = SyntaxFactory.IdentifierName(nameof(DatabaseRecord.IsModified));
                var right = SyntaxFactory.BinaryExpression(
                    SyntaxKind.NotEqualsExpression,
                    SyntaxFactory.IdentifierName(fieldDefinition.Name.FormatAsVariableName("_")),
                    SyntaxFactory.ParseExpression("value"));
                return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.OrAssignmentExpression, left, right));
            }

            StatementSyntax GetFieldValueAssignment()
            {
                var left = SyntaxFactory.IdentifierName(fieldDefinition.Name.FormatAsVariableName("_"));
                var right = SyntaxFactory.ParseExpression("value");
                return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right));
            }
        }
    }
}
