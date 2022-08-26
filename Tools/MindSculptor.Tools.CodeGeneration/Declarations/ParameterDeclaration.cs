using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public class ParameterDeclaration
    {
        private readonly ExpressionSyntax? defaultValue = null;

        public TypeDeclaration Type { get; }
        public string Name { get; }

        public bool HasDefaultValue => defaultValue != null;
        public ExpressionSyntax DefaultValue => defaultValue ?? throw new InvalidOperationException($"Property '{nameof(DefaultValue)}' is undefined. Please check the value of '{nameof(HasDefaultValue)}' prior to accessing this property.");

        public ParameterDeclaration(TypeDeclaration type, string name)
        {
            Type = type;
            Name = name;
        }

        public ParameterDeclaration(TypeDeclaration type, string name, ExpressionSyntax defaultValue)
        {
            Type = type;
            Name = name;
            this.defaultValue = defaultValue;
        }

        public static implicit operator ParameterSyntax(ParameterDeclaration declaration)
            => declaration.HasDefaultValue ? 
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(declaration.Name))
                    .WithType(declaration.Type)
                    .WithDefault(SyntaxFactory.EqualsValueClause(declaration.DefaultValue)) :
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(declaration.Name))
                    .WithType(declaration.Type);
    }
}
