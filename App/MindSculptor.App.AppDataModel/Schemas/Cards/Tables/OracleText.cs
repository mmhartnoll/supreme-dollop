using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Cards.Tables
{
    public class OracleText : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Cards;


        public static readonly IdField FaceId = new IdField.Definition
        {
            MappedField = Faces.Id,
            IsReadOnly = true
        };

        public static readonly TextField Value = new TextField.Definition 
        { 
            MinimumLength = 1
        };

        public static readonly PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = FaceId
        };

        public static readonly ForeignKey FK = new ForeignKey.Definition
        {
            Fields = FaceId,
            ReferencedKey = Faces.PK
        };
    }
}
