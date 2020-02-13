using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MindSculptor.Tools.CodeGeneration.Directives
{
    public class UsingDirective
    {
        public string Namespace { get; }

        protected UsingDirective(string namespaceName)
            => Namespace = namespaceName;

        public static implicit operator UsingDirective(string namespaceName)
            => new UsingDirective(namespaceName);

        public static implicit operator UsingDirectiveSyntax(UsingDirective usingDirective)
            => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingDirective.Namespace));
    }
}
