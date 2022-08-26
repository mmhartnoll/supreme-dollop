using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class BaseMethodDeclaration : MemberDeclaration
    {
        private readonly List<ParameterDeclaration> parameterDeclarations = new List<ParameterDeclaration>();

        protected BaseMethodDeclaration(string name, MemberAccessModifiers accessModifiers, MemberModifiers modifiers)
            : base(name, accessModifiers, modifiers) { }

        public void AddParameter(TypeDeclaration type, string name)
            => parameterDeclarations.Add(new ParameterDeclaration(type, name));

        public void AddParameter(TypeDeclaration type, string name, ExpressionSyntax defaultValue)
            => parameterDeclarations.Add(new ParameterDeclaration(type, name, defaultValue));

        protected override MemberDeclarationSyntax GetMemberDeclarationSyntax()
        {
            var parameterSyntaxes = parameterDeclarations
                .Select<ParameterDeclaration, ParameterSyntax>(x => x)
                .ToArray();
            return GetBaseMethodDeclarationSyntax()
                .AddParameterListParameters(parameterSyntaxes)
                .WithBody(SyntaxFactory.Block(GetMethodStatementSyntaxes()));
        }

        protected abstract BaseMethodDeclarationSyntax GetBaseMethodDeclarationSyntax();

        protected virtual IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
            => Enumerable.Empty<StatementSyntax>();
    }
}
