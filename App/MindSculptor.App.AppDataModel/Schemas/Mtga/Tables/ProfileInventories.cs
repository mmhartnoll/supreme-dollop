using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class ProfileInventories : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;
        public override string SingularName => "ProfileInventory";


        public static IdField ProfileId = new IdField.Definition
        {
            MappedField = Profiles.Id,
            IsReadOnly = true
        };

        public static IntegerField MythicRareWildcardCount = new IntegerField.Definition
        {
            MinimumValue = 0
        };

        public static IntegerField RareWildcardCount = new IntegerField.Definition
        {
            MinimumValue = 0
        };

        public static IntegerField UncommonWildcardCount = new IntegerField.Definition
        {
            MinimumValue = 0
        };

        public static IntegerField CommonWildcardCount = new IntegerField.Definition
        {
            MinimumValue = 0
        };

        public static IntegerField GoldCount = new IntegerField.Definition
        {
            MinimumValue = 0
        };

        public static IntegerField GemCount = new IntegerField.Definition
        {
            MinimumValue = 0
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = ProfileId
        };

        public static ForeignKey FK = new ForeignKey.Definition
        {
            Fields = ProfileId,
            ReferencedKey = Profiles.PK
        };
    }
}
