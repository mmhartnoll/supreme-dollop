using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.SchemaFiles.Properties
{
    internal class TablePropertyDeclaration : ExpressionBodiedPropertyDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private TablePropertyDeclaration(RecordDefinition recordDefinition)
                : base($"{recordDefinition.TableName}Table", recordDefinition.TableName, MemberAccessModifiers.Public)
            => this.recordDefinition = recordDefinition;

        public static TablePropertyDeclaration Create(RecordDefinition recordDefinition)
            => new TablePropertyDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetGetAccessorStatementSyntaxes()
            => SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName($"{recordDefinition.TableName.FormatAsVariableName()}TableLoader.Value")).ToEnumerable();
    }
}
