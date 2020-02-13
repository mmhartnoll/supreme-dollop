using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class ExpressionBodiedPropertyDeclaration : PropertyDeclaration
    {
        protected ExpressionBodiedPropertyDeclaration(TypeDeclaration type, string name, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
            : base(type, name, PropertyAccessability.ReadOnly, accessModifiers, modifiers) 
        { }

        protected override MemberDeclarationSyntax GetMemberDeclarationSyntax()
            => SyntaxFactory.PropertyDeclaration(Type, Name)
                .WithExpressionBody(GetAccessorDeclarationSyntaxes().First().ExpressionBody)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }
}
