using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class Profiles : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;

        public static IdField Id = new IdField.Definition 
        {
            MappedField = Players.Id,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static ForeignKey FK = new ForeignKey.Definition
        {
            Fields = Id,
            ReferencedKey = Players.PK
        };
    }
}
