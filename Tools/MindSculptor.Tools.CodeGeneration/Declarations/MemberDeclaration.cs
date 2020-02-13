using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class MemberDeclaration
    {
        private readonly MemberAccessModifiers accessModifiers;
        private readonly MemberModifiers modifiers;

        public string Name { get; }
        public IEnumerable<MemberAccessModifiers> AccessModifiers => accessModifiers.Enumerate();
        public IEnumerable<MemberModifiers> Modifiers => modifiers.Enumerate();

        protected MemberDeclaration(string name, MemberAccessModifiers accessModifiers, MemberModifiers modifiers)
        {
            Name = name;
            this.accessModifiers = accessModifiers;
            this.modifiers = modifiers;
        }

        public static implicit operator MemberDeclarationSyntax(MemberDeclaration declaration)
            => declaration.GetMemberDeclarationSyntax()
                .AddModifiers(declaration.AccessModifiers.ToSyntaxTokenArray())
                .AddModifiers(declaration.Modifiers.ToSyntaxTokenArray());

        protected abstract MemberDeclarationSyntax GetMemberDeclarationSyntax();
    }
}
