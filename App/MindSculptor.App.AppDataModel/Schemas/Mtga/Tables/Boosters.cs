using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class Boosters : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;


        public static IdField SetId = new IdField.Definition
        {
            MappedField = Cards.Tables.Sets.Id,
            IsReadOnly = true
        };

        public static IntegerField MtgaBoosterId = new IntegerField.Definition
        {
            MinimumValue = 10000,
            MaximumValue = 19999,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = SetId
        };

        public static UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = MtgaBoosterId
        };

        public static ForeignKey FK = new ForeignKey.Definition
        {
            Fields = SetId,
            ReferencedKey = Cards.Tables.Sets.PK
        };
    }
}
