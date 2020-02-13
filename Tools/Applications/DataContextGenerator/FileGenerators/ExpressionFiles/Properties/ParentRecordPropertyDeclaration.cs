using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.ExpressionFiles.Properties
{
    internal class ParentRecordPropertyDeclaration : PropertyDeclaration
    {
        private ParentRecordPropertyDeclaration(ForeignKey foreignKeyDefinition)
            : base($"ParentRecordExpression<{foreignKeyDefinition.ReferencedKey.RecordDefinition.RecordName}>", foreignKeyDefinition.ReferencedKey.RecordDefinition.RecordName, PropertyAccessability.ReadOnly, MemberAccessModifiers.Public)
        { }

        public static ParentRecordPropertyDeclaration Create(ForeignKey foreignKeyDefinition)
            => new ParentRecordPropertyDeclaration(foreignKeyDefinition);
    }
}
