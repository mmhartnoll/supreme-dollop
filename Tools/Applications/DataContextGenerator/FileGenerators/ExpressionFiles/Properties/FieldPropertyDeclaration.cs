using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.ExpressionFiles.Properties
{
    internal class FieldPropertyDeclaration : PropertyDeclaration
    {
        private FieldPropertyDeclaration(Field fieldDefinition)
            : base($"{fieldDefinition.GetType().Name}Expression", fieldDefinition.Name, PropertyAccessability.ReadOnly, MemberAccessModifiers.Public)
        { }

        public static FieldPropertyDeclaration Create(Field fieldDeclaration)
            => new FieldPropertyDeclaration(fieldDeclaration);
    }
}
