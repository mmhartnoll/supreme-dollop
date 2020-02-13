using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class ClassDeclaration : MemberDeclaration
    {
        private readonly List<MemberDeclaration> memberDeclarations = new List<MemberDeclaration>();
        private readonly TypeDeclaration? baseClassType = null;

        public bool IsDerivedClass => baseClassType != null;
        public TypeDeclaration BaseClassType => baseClassType ?? throw new InvalidOperationException($"ClassDeclaration '{Name}' is not a derived class. Please check the '{nameof(IsDerivedClass)}' property before accessing this property.");

        protected ClassDeclaration(string name, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
            : base(name, accessModifiers, modifiers) { }

        protected ClassDeclaration(string name, TypeDeclaration baseClassType, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
                : base(name, accessModifiers, modifiers)
            => this.baseClassType = baseClassType;

        protected void AddConstructor(ConstructorDeclaration ctorDeclaration)
            => memberDeclarations.Add(ctorDeclaration);

        protected void AddField(FieldDeclaration fieldDeclaration)
            => memberDeclarations.Add(fieldDeclaration);

        protected void AddProperty(PropertyDeclaration propertyDeclaration)
            => memberDeclarations.Add(propertyDeclaration);

        protected void AddMethod(MethodDeclaration methodDeclaration)
            => memberDeclarations.Add(methodDeclaration);

        protected void AddSubClass(ClassDeclaration classDeclaration)
            => memberDeclarations.Add(classDeclaration);

        protected override MemberDeclarationSyntax GetMemberDeclarationSyntax()
        {
            var memberDeclarationSyntaxes = memberDeclarations
                .Select<MemberDeclaration, MemberDeclarationSyntax>(x => x)
                .ToArray();
            var declaration = SyntaxFactory.ClassDeclaration(Name)
                .AddMembers(memberDeclarationSyntaxes);

            if (IsDerivedClass)
                declaration = declaration.AddBaseListTypes(SyntaxFactory.SimpleBaseType(BaseClassType));

            return declaration;
        }
    }
}
