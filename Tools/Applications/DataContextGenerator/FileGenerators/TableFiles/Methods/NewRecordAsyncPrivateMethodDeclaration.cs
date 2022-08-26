using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Threading;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods
{
    internal class NewRecordAsyncPrivateMethodDeclaration : NewRecordPrivateMethodDeclarationBase
    {
        private NewRecordAsyncPrivateMethodDeclaration(RecordDefinition recordDefinition)
            : base($"Task<{recordDefinition.RecordName}>", "NewRecordAsync", MemberModifiers.Async, recordDefinition)
        {
            AddParameter(typeof(CancellationToken), nameof(CancellationToken).FormatAsVariableName());
        }

        public static NewRecordAsyncPrivateMethodDeclaration Create(RecordDefinition recordDefinition)
            => new NewRecordAsyncPrivateMethodDeclaration(recordDefinition);

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
