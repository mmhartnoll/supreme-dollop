using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class ConstructorDeclaration : BaseMethodDeclaration
    {
        private readonly List<ArgumentDeclaration> baseConstructorArgs = new List<ArgumentDeclaration>();

        protected ConstructorDeclaration(string name, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
            : base(name, accessModifiers, modifiers) { }

        public void AddBaseConstructorArguments(params ArgumentDeclaration[] arguments)
            => baseConstructorArgs.AddRange(arguments);

        protected override BaseMethodDeclarationSyntax GetBaseMethodDeclarationSyntax()
        {
            var declaration = SyntaxFactory.ConstructorDeclaration(Name);

            if (baseConstructorArgs.Any())
            {
                var argumentSyntaxes = baseConstructorArgs
                    .Select<ArgumentDeclaration, ArgumentSyntax>(x => x)
                    .ToArray();
                var initializer = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                    .AddArgumentListArguments(argumentSyntaxes);
                declaration = declaration.WithInitializer(initializer);
            }

            return declaration;
        }
    }
}
