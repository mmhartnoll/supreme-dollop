using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class NamespaceDeclaration
    {
        private readonly List<MemberDeclaration> memberDeclarations = new List<MemberDeclaration>();

        public string Name { get; }

        protected NamespaceDeclaration(string name)
            => Name = name;

        public static implicit operator NamespaceDeclarationSyntax(NamespaceDeclaration declaration)
        {
            var memberDeclarationSyntaxes = declaration.memberDeclarations
                .Select<MemberDeclaration, MemberDeclarationSyntax>(x => x)
                .ToArray();
            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(declaration.Name))
                .AddMembers(memberDeclarationSyntaxes);
        }

        protected void AddClass(ClassDeclaration classDeclaration)
            => memberDeclarations.Add(classDeclaration);
    }
}
