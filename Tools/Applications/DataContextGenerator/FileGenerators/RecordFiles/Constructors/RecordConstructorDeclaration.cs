using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Constructors
{
    internal class RecordConstructorDeclaration : ConstructorDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private RecordConstructorDeclaration(RecordDefinition recordDefinition)
            : base(recordDefinition.RecordName, MemberAccessModifiers.Private, MemberModifiers.None)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DatabaseContext), nameof(DatabaseContext).FormatAsVariableName());
            AddParameter($"{recordDefinition.TableName}Table", $"{recordDefinition.TableName}Table".FormatAsVariableName());

            foreach (var fieldDefinition in recordDefinition.Fields)
                AddParameter(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), fieldDefinition.Name.FormatAsVariableName());

            AddBaseConstructorArguments(nameof(DatabaseContext).FormatAsVariableName());
            AddBaseConstructorArguments($"{recordDefinition.TableName}Table".FormatAsVariableName());
        }

        public static RecordConstructorDeclaration Create(RecordDefinition recordDefinition)
            => new RecordConstructorDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                var fieldName = fieldDefinition.IsReadOnly ?
                    fieldDefinition.Name :
                    fieldDefinition.Name.FormatAsVariableName("_");
                yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(fieldName),
                    SyntaxFactory.ParseExpression(fieldDefinition.Name.FormatAsVariableName())));
            }
        }
    }
}
