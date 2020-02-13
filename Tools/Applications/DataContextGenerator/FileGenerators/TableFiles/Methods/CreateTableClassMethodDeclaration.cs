using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods
{
    internal class CreateTableClassMethodDeclaration : MethodDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private CreateTableClassMethodDeclaration(RecordDefinition recordDefinition) 
            : base($"{recordDefinition.TableName}Table", "Create", MemberAccessModifiers.Internal, MemberModifiers.Static)
        {
            this.recordDefinition = recordDefinition;

            AddParameter(typeof(DataContext), nameof(DataContext).FormatAsVariableName());
        }

        public static CreateTableClassMethodDeclaration Create(RecordDefinition recordDefinition)
            => new CreateTableClassMethodDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName($"{recordDefinition.TableName}Table"))
                .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(DataContext).FormatAsVariableName())));
            yield return SyntaxFactory.ReturnStatement(objectCreationExpression);
        }
    }
}
