using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class DigitalCards : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;

        
        public static IdField Id = new IdField.Definition
        {
            MappedField = Cards.Tables.BasePrintings.Id,
            IsReadOnly = true
        };

        public static IntegerField MtgaCardId = new IntegerField.Definition
        {
            MinimumValue = 0,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = MtgaCardId
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = Id,
            ReferencedKey = Cards.Tables.BasePrintings.PK
        };
    }
}
