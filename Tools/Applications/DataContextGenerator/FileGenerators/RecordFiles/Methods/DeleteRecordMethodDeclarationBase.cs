using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal abstract class DeleteRecordMethodDeclarationBase : MethodDeclaration
    {
        private RecordDefinition recordDefinition;

        protected DeleteRecordMethodDeclarationBase(TypeDeclaration type, string name, MemberModifiers modifiers, RecordDefinition recordDefinition)
                : base(type, name, MemberAccessModifiers.Protected, modifiers)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DbCommand), "command");
        }

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("command.CommandText"),
                SyntaxFactory.ParseExpression($@"""{GetCommandText()}""")));

            foreach (var fieldDefinition in GetPrimaryKeyFields())
                yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("command.AddParameter"))
                    .AddArgumentListArguments(
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression($@"""{fieldDefinition.Name}""")),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(fieldDefinition.Name))));

            yield return GetExecuteStatementSyntax();

            yield return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("OnRecordChanged"))
                    .AddArgumentListArguments(
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression("RecordChangedAction.Delete")),
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression("this"))));

            IEnumerable<Field> GetPrimaryKeyFields()
                    => recordDefinition.PrimaryKey.Fields.Select(x => x.Field);

            string GetCommandText()
                => string.Format("DELETE FROM [{0}].[{1}] WHERE {2};",
                    recordDefinition.Schema.Name,
                    recordDefinition.TableName,
                    string.Join(" AND ", GetPrimaryKeyFields().Select(fieldDefinition => $@"{fieldDefinition.Name} = @{fieldDefinition.Name}")));
        }

        protected abstract StatementSyntax GetExecuteStatementSyntax();
    }
}
