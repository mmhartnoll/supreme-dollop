using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class DraftEventMatchResults : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;


        public static IdField DraftEventMatchId = new IdField.Definition
        {
            MappedField = DraftEventMatches.Id,
            IsReadOnly = true
        };

        public static BooleanField MatchWon = new BooleanField.Definition
        {
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = DraftEventMatchId
        };

        public static ForeignKey FK = new ForeignKey.Definition
        {
            Fields = DraftEventMatchId,
            ReferencedKey = DraftEventMatches.PK
        };
    }
}
