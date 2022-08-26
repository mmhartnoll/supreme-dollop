using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal abstract class UpdateRecordMethodDeclarationBase : MethodDeclaration
    {
        private RecordDefinition recordDefinition;

        protected UpdateRecordMethodDeclarationBase(TypeDeclaration type, string name, MemberModifiers modifiers, RecordDefinition recordDefinition)
                : base(type, name, MemberAccessModifiers.Protected, modifiers)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DbCommand), "command");
        }

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var conditionSyntax = SyntaxFactory.IdentifierName(nameof(DatabaseRecord.IsModified));
            yield return SyntaxFactory.IfStatement(conditionSyntax, SyntaxFactory.Block(GetConditionalBlockStatements()));

            IEnumerable<StatementSyntax> GetConditionalBlockStatements()
            {
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

                yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("IsModified"),
                    SyntaxFactory.ParseExpression("false")));

                yield return SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("OnRecordChanged"))
                        .AddArgumentListArguments(
                            SyntaxFactory.Argument(SyntaxFactory.ParseExpression("RecordChangedAction.Update")),
                            SyntaxFactory.Argument(SyntaxFactory.ParseExpression("this"))));

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
