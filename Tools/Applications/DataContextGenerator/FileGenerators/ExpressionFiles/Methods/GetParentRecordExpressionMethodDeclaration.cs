using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.ExpressionFiles.Methods
{
    internal class GetParentRecordExpressionMethodDeclaration : MethodDeclaration
    {
        private readonly ForeignKey foreignKeyDefinition;

        private GetParentRecordExpressionMethodDeclaration(ForeignKey foreignKeyDefinition) 
            : base(typeof(BooleanValueExpression), $"Get{foreignKeyDefinition.ReferencedKey.RecordDefinition.RecordName}Expression", MemberAccessModifiers.Private)
        {
            this.foreignKeyDefinition = foreignKeyDefinition;

            AddParameter(
                foreignKeyDefinition.ReferencedKey.RecordDefinition.RecordName, 
                foreignKeyDefinition.ReferencedKey.RecordDefinition.RecordName.FormatAsVariableName());
        }

        public static GetParentRecordExpressionMethodDeclaration Create(ForeignKey foreignKeyDefiniton)
            => new GetParentRecordExpressionMethodDeclaration(foreignKeyDefiniton);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var zippedFields = foreignKeyDefinition.Fields.Select(field => field.Field)
                .Zip(foreignKeyDefinition.ReferencedKey.Fields.Select(field => field.Field));

            using (var enumerator = zippedFields.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new Exception($"Foreign key '{foreignKeyDefinition.Name}' does not define any fields.");

                var expression = GetEqualsExpression(enumerator.Current.First, enumerator.Current.Second);
                while (enumerator.MoveNext())
                {
                    expression = SyntaxFactory.BinaryExpression(
                        SyntaxKind.LogicalAndExpression,
                        expression,
                        GetEqualsExpression(enumerator.Current.First, enumerator.Current.Second));
                }
                yield return SyntaxFactory.ReturnStatement(expression);
            }

            BinaryExpressionSyntax GetEqualsExpression(Field first, Field second) 
                => SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    SyntaxFactory.IdentifierName(first.Name),
                    SyntaxFactory.IdentifierName($"{foreignKeyDefinition.ReferencedKey.RecordDefinition.RecordName.FormatAsVariableName()}.{second.Name}"));
        }
    }
}
