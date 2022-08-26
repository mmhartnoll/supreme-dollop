using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class DraftEvents : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;


        public static IdField EventId = new IdField.Definition
        {
            MappedField = Events.Id,
            IsReadOnly = true
        };

        public static IdField SetId = new IdField.Definition
        {
            MappedField = Cards.Tables.Sets.Id,
            IsReadOnly = true
        };

        public static TextField DraftType = new TextField.Definition
        {
            MinimumLength = 1,
            MaximumLength = 20,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = EventId
        };

        public static UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = FieldReference.List(SetId, DraftType)
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = EventId,
            ReferencedKey = Events.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = SetId,
            ReferencedKey = Cards.Tables.Sets.PK
        };
    }
}
