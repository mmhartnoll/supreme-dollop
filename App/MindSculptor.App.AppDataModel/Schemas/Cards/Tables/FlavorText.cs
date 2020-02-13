using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Cards.Tables
{
    public class FlavorText : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Cards;


        public static IdField FacePrintingId = new IdField.Definition
        {
            MappedField = FacePrintings.Id,
            IsReadOnly = true
        };

        public static TextField Value = new TextField.Definition 
        { 
            MinimumLength = 1
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = FacePrintingId
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = FacePrintingId,
            ReferencedKey = FacePrintings.PK
        };
    }
}
