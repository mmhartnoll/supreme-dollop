using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Constructors
{
    internal class TableConstructorDeclaration : ConstructorDeclaration
    {
        protected TableConstructorDeclaration(RecordDefinition recordDefinition) 
            : base($"{recordDefinition.TableName}Table", MemberAccessModifiers.Private)
        {
            AddParameter(typeof(DataContext), nameof(DataContext).FormatAsVariableName());

            AddBaseConstructorArguments(
                nameof(DataContext).FormatAsVariableName(),
                $@"""{recordDefinition.Schema.Name}""", 
                $@"""{recordDefinition.TableName}""");
        }

        public static TableConstructorDeclaration Create(RecordDefinition recordDefinition)
            => new TableConstructorDeclaration(recordDefinition);
    }
}
