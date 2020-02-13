using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.RecordFiles.Methods
{
    internal class CreateRecordMethodDeclaration : MethodDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private CreateRecordMethodDeclaration(RecordDefinition recordDefinition) 
            : base(recordDefinition.RecordName, "Create", MemberAccessModifiers.Internal, MemberModifiers.Static)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DataContext), nameof(DataContext).FormatAsVariableName());

            foreach (var fieldDefinition in recordDefinition.Fields)
                AddParameter(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), fieldDefinition.Name.FormatAsVariableName());
        }

        public static CreateRecordMethodDeclaration Create(RecordDefinition recordDefinition)
            => new CreateRecordMethodDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var argumentSyntaxes = recordDefinition.Fields
                .Select(fieldDefinition => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(fieldDefinition.Name.FormatAsVariableName())));
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(recordDefinition.RecordName))
                .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DataContext).FormatAsVariableName())))
                .AddArgumentListArguments(argumentSyntaxes.ToArray());
            yield return SyntaxFactory.ReturnStatement(objectCreationExpression);
        }
    }
}
