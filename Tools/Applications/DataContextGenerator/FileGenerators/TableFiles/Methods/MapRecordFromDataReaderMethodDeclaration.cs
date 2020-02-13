using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Data.Common;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods
{
    internal class MapRecordFromDataReaderMethodDeclaration : MethodDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private MapRecordFromDataReaderMethodDeclaration(RecordDefinition recordDefinition)
            : base(recordDefinition.RecordName, "MapRecordFromDataReader", MemberAccessModifiers.Protected, MemberModifiers.Override)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DbDataReader), nameof(DbDataReader).FormatAsVariableName());
        }

        public static MapRecordFromDataReaderMethodDeclaration Create(RecordDefinition recordDefinition)
            => new MapRecordFromDataReaderMethodDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var argumentList = new List<ArgumentSyntax>();
            argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DataContext))));

            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                ExpressionSyntax expressionSyntax;

                argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(fieldDefinition.Name.FormatAsVariableName())));

                var indexExpression = SyntaxFactory.ParseExpression($@"""{fieldDefinition.Name}""");
                var elementAccessExpression = SyntaxFactory.ElementAccessExpression(SyntaxFactory.IdentifierName(nameof(DbDataReader).FormatAsVariableName()))
                    .AddArgumentListArguments(SyntaxFactory.Argument(indexExpression));

                if (fieldDefinition is IntegerField)
                {
                    expressionSyntax = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("Convert.ToInt32"))
                        .AddArgumentListArguments(SyntaxFactory.Argument(elementAccessExpression));
                    if (fieldDefinition.IsNullable)
                        expressionSyntax = SyntaxFactory.CastExpression(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), expressionSyntax);
                }
                else
                    expressionSyntax = SyntaxFactory.CastExpression(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), elementAccessExpression);

                if (fieldDefinition.IsNullable)
                {
                    var binaryExpression = SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, elementAccessExpression, SyntaxFactory.ParseExpression("DBNull.Value"));
                    expressionSyntax = SyntaxFactory.ConditionalExpression(binaryExpression, SyntaxFactory.ParseExpression("null"), expressionSyntax);
                }

                var variableDeclarationSyntax = SyntaxFactory.VariableDeclaration(TypeDeclaration.Var)
                    .AddVariables(SyntaxFactory.VariableDeclarator(
                        SyntaxFactory.Identifier(fieldDefinition.Name.FormatAsVariableName()),
                        null,
                        SyntaxFactory.EqualsValueClause(expressionSyntax)));
                yield return SyntaxFactory.LocalDeclarationStatement(variableDeclarationSyntax);
            }

            var invocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"{recordDefinition.RecordName}.Create"))
                .AddArgumentListArguments(argumentList.ToArray());
            yield return SyntaxFactory.ReturnStatement(invocationExpression);
        }
    }
}
