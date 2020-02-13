using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public class FieldDeclaration : MemberDeclaration
    {
        public TypeDeclaration Type { get; }

        protected FieldDeclaration(TypeDeclaration type, string name, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
                : base(name, accessModifiers, modifiers)
            => Type = type;

        public static FieldDeclaration Create(TypeDeclaration type, string name, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
            => new FieldDeclaration(type, name, accessModifiers, modifiers);

        protected override MemberDeclarationSyntax GetMemberDeclarationSyntax()
            => SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(Type)
                .AddVariables(SyntaxFactory.VariableDeclarator(Name)));
    }
}
