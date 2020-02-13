using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Schemas;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Properties
{
    internal class SchemaPropertyDeclaration : ExpressionBodiedPropertyDeclaration
    {
        private readonly Schema schemaDefinition;

        private SchemaPropertyDeclaration(Schema schemaDefinition)
                : base($"{schemaDefinition.Name}Schema", schemaDefinition.Name, MemberAccessModifiers.Public)
            => this.schemaDefinition = schemaDefinition;

        public static SchemaPropertyDeclaration Create(Schema schemaDefinition)
            => new SchemaPropertyDeclaration(schemaDefinition);

        protected override IEnumerable<StatementSyntax> GetGetAccessorStatementSyntaxes()
            => SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName($"{schemaDefinition.Name.FormatAsVariableName()}SchemaLoader.Value")).ToEnumerable();
    }
}
