using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public abstract class PropertyDeclaration : MemberDeclaration
    {
        public TypeDeclaration Type { get; }
        public PropertyAccessability Accessability { get; }

        protected PropertyDeclaration(TypeDeclaration type, string name, PropertyAccessability accessability, MemberAccessModifiers accessModifiers = Declarations.MemberAccessModifiers.Default, MemberModifiers modifiers = Declarations.MemberModifiers.Default)
            : base(name, accessModifiers, modifiers)
        {
            Type = type;
            Accessability = accessability;
        }

        protected override MemberDeclarationSyntax GetMemberDeclarationSyntax()
            => SyntaxFactory.PropertyDeclaration(Type, Name)
                .AddAccessorListAccessors(GetAccessorDeclarationSyntaxes().ToArray());

        protected IEnumerable<AccessorDeclarationSyntax> GetAccessorDeclarationSyntaxes()
        {
            if (Accessability == PropertyAccessability.ReadWrite || Accessability == PropertyAccessability.ReadOnly)
                yield return SetAccessorDeclarationBody(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration), GetGetAccessorStatementSyntaxes());
            if (Accessability == PropertyAccessability.ReadWrite || Accessability == PropertyAccessability.WriteOnly)
                yield return SetAccessorDeclarationBody(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration), GetSetAccessorStatementSyntaxes());

            static AccessorDeclarationSyntax SetAccessorDeclarationBody(AccessorDeclarationSyntax declaration, IEnumerable<StatementSyntax> source)
            {
                var statementSyntaxes = source.ToList();
                if (statementSyntaxes.Any())
                {
                    if (statementSyntaxes.Count() == 1)
                    {
                        var statementSyntax = statementSyntaxes.First();
                        ExpressionSyntax? expressionSyntax = null;
                        if (statementSyntax is ExpressionStatementSyntax expressionStatementSyntax)
                            expressionSyntax = expressionStatementSyntax.Expression;
                        else if (statementSyntax is ReturnStatementSyntax returnStatementSyntax)
                            expressionSyntax = returnStatementSyntax.Expression;
                        if (expressionSyntax != null)
                        {
                            var arrowExpression = SyntaxFactory.ArrowExpressionClause(expressionSyntax);
                            return declaration.WithExpressionBody(arrowExpression).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                        }
                    }
                    return declaration.WithBody(SyntaxFactory.Block(statementSyntaxes));
                }
                return declaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
        }

        protected virtual IEnumerable<StatementSyntax> GetGetAccessorStatementSyntaxes()
            => Enumerable.Empty<StatementSyntax>();

        protected virtual IEnumerable<StatementSyntax> GetSetAccessorStatementSyntaxes()
            => Enumerable.Empty<StatementSyntax>();

        public enum PropertyAccessability
        {
            ReadWrite,
            ReadOnly,
            WriteOnly,
            Default = ReadWrite
        }
    }
}
