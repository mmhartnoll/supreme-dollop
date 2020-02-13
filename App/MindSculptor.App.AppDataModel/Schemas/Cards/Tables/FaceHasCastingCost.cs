using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Cards.Tables
{
    public class FaceHasCastingCost : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Cards;


        public static readonly IdField FaceId = new IdField.Definition
        {
            MappedField = Faces.Id,
            IsReadOnly = true
        };

        public static readonly IdField ManaSymbolId = new IdField.Definition
        {
            MappedField = ManaSymbols.Id,
            IsReadOnly = true
        };

        public static readonly IntegerField Ordinal = new IntegerField.Definition
        {
            MinimumValue = 0,
            MaximumValue = 7
        };

        public static readonly IntegerField Count = new IntegerField.Definition
        {
            MinimumValue = 1,
            MaximumValue = 15
        };

        public static readonly PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = FieldReference.List(FaceId, ManaSymbolId)
        };

        public static readonly ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = FaceId,
            ReferencedKey = Faces.PK
        };

        public static readonly ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = ManaSymbolId,
            ReferencedKey = ManaSymbols.PK
        };
    }
}
