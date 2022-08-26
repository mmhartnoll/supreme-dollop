using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Threading;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal class DeleteRecordAsyncMethodDeclaration : DeleteRecordMethodDeclarationBase
    {
        private DeleteRecordAsyncMethodDeclaration(RecordDefinition recordDefinition)
            : base("Task", "DeleteRecordAsync", MemberModifiers.Async | MemberModifiers.Override, recordDefinition)
        {
            AddParameter(typeof(CancellationToken), nameof(CancellationToken).FormatAsVariableName());
        }

        public static DeleteRecordAsyncMethodDeclaration Create(RecordDefinition recordDefinition)
            => new DeleteRecordAsyncMethodDeclaration(recordDefinition);

        protected override StatementSyntax GetExecuteStatementSyntax()
            => SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.IdentifierName("command.ExecuteNonQueryAsync"))
                                    .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(CancellationToken).FormatAsVariableName()))),
                                SyntaxFactory.IdentifierName("ConfigureAwait")))
                        .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")))));
    }
}
