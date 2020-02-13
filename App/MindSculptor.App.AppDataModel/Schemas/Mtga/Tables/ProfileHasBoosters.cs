using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class ProfileHasBoosters : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;
        public override string SingularName => nameof(ProfileHasBoosters);


        public static IdField ProfileId = new IdField.Definition
        {
            MappedField = Profiles.Id,
            IsReadOnly = true
        };

        public static IdField BoosterId = new IdField.Definition
        {
            MappedField = Boosters.SetId,
            IsReadOnly = true
        };

        public static IntegerField Count = new IntegerField.Definition
        {
            MinimumValue = 1,
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = FieldReference.List(ProfileId, BoosterId)
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = ProfileId,
            ReferencedKey = Profiles.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = BoosterId,
            ReferencedKey = Boosters.PK
        };
    }
}
