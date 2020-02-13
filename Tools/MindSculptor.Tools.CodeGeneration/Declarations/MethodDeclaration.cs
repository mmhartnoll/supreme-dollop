using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class MethodDeclaration : BaseMethodDeclaration
    {
        public TypeDeclaration Type { get; }

        protected MethodDeclaration(TypeDeclaration type, string name, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
                : base(name, accessModifiers, modifiers)
            => Type = type;

        protected override BaseMethodDeclarationSyntax GetBaseMethodDeclarationSyntax()
            => SyntaxFactory.MethodDeclaration(Type, Name);
    }
}
