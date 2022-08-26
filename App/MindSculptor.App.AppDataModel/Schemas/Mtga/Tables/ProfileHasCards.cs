using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class ProfileHasCards : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;


        public static IdField ProfileId = new IdField.Definition()
        {
            MappedField = Profiles.Id,
            IsReadOnly = true
        };

        public static IdField DigitalCardId = new IdField.Definition
        {
            MappedField = DigitalCards.Id,
            IsReadOnly = true
        };

        public static IntegerField Count = new IntegerField.Definition
        {
            MinimumValue = 1,
            MaximumValue = 4
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = FieldReference.List(ProfileId, DigitalCardId)
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = ProfileId,
            ReferencedKey = Profiles.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = DigitalCardId,
            ReferencedKey = DigitalCards.PK
        };
    }
}
