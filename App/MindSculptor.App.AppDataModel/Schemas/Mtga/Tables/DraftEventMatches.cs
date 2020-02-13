using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class DraftEventMatches : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;
        public override string SingularName => "DraftEventMatch";


        public static IdField Id = new IdField.Definition
        {
            IsReadOnly = true
        };

        public static IdField DraftEventEntryId = new IdField.Definition
        {
            MappedField = DraftEventEntries.Id,
            IsReadOnly = true
        };

        public static IdField OpponentId = new IdField.Definition
        {
            MappedField = Opponents.Id,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = DraftEventEntryId,
            ReferencedKey = DraftEventEntries.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = OpponentId,
            ReferencedKey =  Opponents.PK
        };
    }
}
