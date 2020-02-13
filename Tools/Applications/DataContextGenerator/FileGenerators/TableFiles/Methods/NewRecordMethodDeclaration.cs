using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods
{
    internal class NewRecordMethodDeclaration : NewRecordMethodDeclarationBase
    {
        private NewRecordMethodDeclaration(RecordDefinition recordDefinition)
            : base(recordDefinition.RecordName, "NewRecord", MemberModifiers.None, recordDefinition)
        { }

        public static NewRecordMethodDeclaration Create(RecordDefinition recordDefinition)
            => new NewRecordMethodDeclaration(recordDefinition);

        protected override StatementSyntax GetExecuteStatementSyntax()
            => SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("command.ExecuteNonQuery")));
    }
}
