using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal class DeleteRecordAsyncMethodDeclaration : DeleteRecordMethodDeclarationBase
    {
        private DeleteRecordAsyncMethodDeclaration(RecordDefinition recordDefinition)
            : base("Task", "DeleteRecordAsync", MemberModifiers.Async | MemberModifiers.Override, recordDefinition)
        { }

        public static DeleteRecordAsyncMethodDeclaration Create(RecordDefinition recordDefinition)
            => new DeleteRecordAsyncMethodDeclaration(recordDefinition);

        protected override StatementSyntax GetExecuteStatementSyntax()
            => SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName("command.ExecuteNonQueryAsync")),
                                SyntaxFactory.IdentifierName("ConfigureAwait")))
                        .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")))));
    }
}
