using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class EventMatches : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;
        public override string SingularName => "EventMatch";


        public static IdField Id = new IdField.Definition
        {
            IsReadOnly = true
        };

        public static IdField EventEntryId = new IdField.Definition
        {
            MappedField = EventEntries.Id,
            IsReadOnly = true
        };

        public static IdField OpponentId = new IdField.Definition
        {
            MappedField = Players.Id,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = EventEntryId,
            ReferencedKey = EventEntries.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = OpponentId,
            ReferencedKey =  Players.PK
        };
    }
}
