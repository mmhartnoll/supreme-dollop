using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public class ParameterDeclaration
    {
        public TypeDeclaration Type { get; }
        public string Name { get; }

        public ParameterDeclaration(TypeDeclaration type, string name)
        {
            Type = type;
            Name = name;
        }

        public static implicit operator ParameterSyntax(ParameterDeclaration declaration)
            => SyntaxFactory.Parameter(SyntaxFactory.Identifier(declaration.Name))
                .WithType(declaration.Type);
    }
}
