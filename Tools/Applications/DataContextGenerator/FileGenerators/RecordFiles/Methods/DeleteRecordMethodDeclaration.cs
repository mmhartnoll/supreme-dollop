using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal class DeleteRecordMethodDeclaration : DeleteRecordMethodDeclarationBase
    {
        private DeleteRecordMethodDeclaration(RecordDefinition recordDefinition)
            : base("void", "DeleteRecord", MemberModifiers.Override, recordDefinition)
        { }

        public static DeleteRecordMethodDeclaration Create(RecordDefinition recordDefinition)
            => new DeleteRecordMethodDeclaration(recordDefinition);

        protected override StatementSyntax GetExecuteStatementSyntax()
            => SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("command.ExecuteNonQuery")));
    }
}
