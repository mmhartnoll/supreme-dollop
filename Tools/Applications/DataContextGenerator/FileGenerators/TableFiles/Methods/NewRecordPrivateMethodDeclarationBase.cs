using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods
{
    internal abstract class NewRecordPrivateMethodDeclarationBase : MethodDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        protected NewRecordPrivateMethodDeclarationBase(TypeDeclaration type, string name, MemberModifiers modifiers, RecordDefinition recordDefinition)
                : base(type, name, MemberAccessModifiers.Private, modifiers)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DbCommand), "command");

            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                var parameterName = fieldDefinition.Name.FormatAsVariableName();
                AddParameter(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), parameterName);
            }
        }

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var argumentList = new List<ArgumentSyntax>();
            argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("DatabaseContext")));
            argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("this")));
            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                var argumentName = fieldDefinition.Name.FormatAsVariableName();
                argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(argumentName)));
            }
            var createInvocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"{recordDefinition.RecordName}.Create"))
                .AddArgumentListArguments(argumentList.ToArray());
            var variableDeclarationSyntax = SyntaxFactory.VariableDeclaration(TypeDeclaration.Var)
                .AddVariables(SyntaxFactory.VariableDeclarator(
                    SyntaxFactory.Identifier("newRecord"),
                    null,
                    SyntaxFactory.EqualsValueClause(createInvocationExpression)));
            yield return SyntaxFactory.LocalDeclarationStatement(variableDeclarationSyntax);

            var equalsValueClause = SyntaxFactory.EqualsValueClause(SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName("DatabaseContext.Connection.CreateCommand")));
            var variableDeclaration = SyntaxFactory.VariableDeclaration(TypeDeclaration.Var)
                .AddVariables(SyntaxFactory.VariableDeclarator(
                    SyntaxFactory.Identifier("command"),
                    null,
                    equalsValueClause));

            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("command.CommandText"),
                SyntaxFactory.ParseExpression($@"""{GetCommandText()}""")));

            foreach (var fieldDefinition in recordDefinition.Fields)
                yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("command.AddParameter"))
                    .AddArgumentListArguments(
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression($@"""{fieldDefinition.Name}""")),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName($"newRecord.{fieldDefinition.Name}"))));

            yield return GetExecuteStatementSyntax();

            yield return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("OnRecordChanged"))
                    .AddArgumentListArguments(
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression("RecordChangedAction.Create")),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("newRecord"))));

            yield return SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("newRecord"));

            string GetCommandText()
            {
                return string.Format("INSERT INTO [{0}].[{1}] ( {2} ) VALUES ( {3} );",
                    recordDefinition.Schema.Name,
                    recordDefinition.TableName,
                    string.Join(", ", recordDefinition.Fields.Select(fieldDefinition => fieldDefinition.Name)),
                    string.Join(", ", recordDefinition.Fields.Select(fieldDefinition => $"@{fieldDefinition.Name}")));
            }
        }

        protected abstract StatementSyntax GetExecuteStatementSyntax();
    }
}
