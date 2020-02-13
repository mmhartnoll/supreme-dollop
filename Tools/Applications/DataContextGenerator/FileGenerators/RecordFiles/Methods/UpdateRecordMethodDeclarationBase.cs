using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal abstract class UpdateRecordMethodDeclarationBase : MethodDeclaration
    {
        private RecordDefinition recordDefinition;

        protected UpdateRecordMethodDeclarationBase(TypeDeclaration type, string name, MemberModifiers modifiers, RecordDefinition recordDefinition)
                : base(type, name, MemberAccessModifiers.Public, modifiers)
            => this.recordDefinition = recordDefinition;

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var conditionSyntax = SyntaxFactory.IdentifierName(nameof(DataContextRecord.IsModified));
            var equalsValueClause = SyntaxFactory.EqualsValueClause(SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName("DataContext.Connection.CreateCommand")));
            var variableDeclaration = SyntaxFactory.VariableDeclaration(TypeDeclaration.Var)
                .AddVariables(SyntaxFactory.VariableDeclarator(
                    SyntaxFactory.Identifier("command"),
                    null,
                    equalsValueClause));
            var usingStatementSyntax = SyntaxFactory.UsingStatement(SyntaxFactory.Block(GetUsingBlockStatements()))
                .WithDeclaration(variableDeclaration);

            yield return SyntaxFactory.IfStatement(conditionSyntax, usingStatementSyntax);

            IEnumerable<StatementSyntax> GetUsingBlockStatements()
            {
                var assignmentExpression = SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("command.Transaction"),
                    SyntaxFactory.ParseExpression("DataContext.Transaction"));
                yield return SyntaxFactory.IfStatement(
                    SyntaxFactory.IdentifierName("DataContext.HasTransaction"),
                    SyntaxFactory.ExpressionStatement(assignmentExpression));

                yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("command.CommandText"),
                    SyntaxFactory.ParseExpression($@"""{GetCommandText()}""")));

                foreach (var fieldDefinition in recordDefinition.Fields)
                    yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("command.AddParameter"))
                        .AddArgumentListArguments(
                            SyntaxFactory.Argument(SyntaxFactory.ParseExpression($@"""{fieldDefinition.Name}""")),
                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(fieldDefinition.Name))));

                yield return GetExecuteStatementSyntax();

                string GetCommandText()
                {
                    return string.Format("UPDATE [{0}].[{1}] SET {2} WHERE {3};",
                        recordDefinition.Schema.Name,
                        recordDefinition.TableName,
                        string.Join(", ", GetNonPrimaryKeyFields().Select(fieldDefinition => $@"{fieldDefinition.Name} = @{fieldDefinition.Name}")),
                        string.Join(" AND ", GetPrimaryKeyFields().Select(fieldDefinition => $@"{fieldDefinition.Name} = @{fieldDefinition.Name}")));

                    IEnumerable<Field> GetPrimaryKeyFields()
                        => recordDefinition.PrimaryKey.Fields.Select(fieldDefinition => fieldDefinition.Field);

                    IEnumerable<Field> GetNonPrimaryKeyFields()
                        => recordDefinition.Fields.Except(GetPrimaryKeyFields());
                }
            }
        }

        protected abstract StatementSyntax GetExecuteStatementSyntax();
    }
}
