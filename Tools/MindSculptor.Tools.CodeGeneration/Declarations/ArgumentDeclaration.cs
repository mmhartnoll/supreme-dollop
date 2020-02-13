using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public class ArgumentDeclaration
    {
        private readonly ExpressionSyntax expressionSyntax;

        protected ArgumentDeclaration(ExpressionSyntax expressionSyntax)
            => this.expressionSyntax = expressionSyntax;

        public static implicit operator ArgumentDeclaration(string identifier)
            => new ArgumentDeclaration(SyntaxFactory.IdentifierName(identifier));

        public static implicit operator ArgumentDeclaration(ExpressionSyntax expressionSyntax)
            => new ArgumentDeclaration(expressionSyntax);

        public static implicit operator ArgumentSyntax(ArgumentDeclaration declaration)
            => SyntaxFactory.Argument(declaration.expressionSyntax);
    }
}
