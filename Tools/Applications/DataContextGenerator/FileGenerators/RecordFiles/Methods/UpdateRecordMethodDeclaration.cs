using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal class UpdateRecordMethodDeclaration : UpdateRecordMethodDeclarationBase
    {
        private UpdateRecordMethodDeclaration(RecordDefinition recordDefinition)
            : base("void", "UpdateRecord", MemberModifiers.Override, recordDefinition)
        { }

        public static UpdateRecordMethodDeclaration Create(RecordDefinition recordDefinition)
            => new UpdateRecordMethodDeclaration(recordDefinition);

        protected override StatementSyntax GetExecuteStatementSyntax()
            => SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("command.ExecuteNonQuery")));
    }
}
