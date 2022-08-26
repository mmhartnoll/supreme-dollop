using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Modelled.Schemas;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.SchemaFiles.Methods
{
    internal class CreateSchemaMethodDeclaration : MethodDeclaration
    {
        private readonly Schema schemaDefinition;

        public CreateSchemaMethodDeclaration(Schema schemaDefinition) 
            : base($"{schemaDefinition.Name}Schema", "Create", MemberAccessModifiers.Internal, MemberModifiers.Static)
        {
            this.schemaDefinition = schemaDefinition;

            AddParameter(typeof(DatabaseContext), nameof(DatabaseContext).FormatAsVariableName());
        }

        public static CreateSchemaMethodDeclaration Create(Schema schemaDefinition)
            => new CreateSchemaMethodDeclaration(schemaDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName($"{schemaDefinition.Name}Schema"))
                .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DatabaseContext).FormatAsVariableName())));
            yield return SyntaxFactory.ReturnStatement(objectCreationExpression);
        }
    }
}
