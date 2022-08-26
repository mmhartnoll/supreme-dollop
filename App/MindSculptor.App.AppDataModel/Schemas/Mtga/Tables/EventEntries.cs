using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class EventEntries : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;
        public override string SingularName => "EventEntry";


        public static readonly IdField Id = new IdField.Definition
        {
            IsReadOnly = true
        };

        public static readonly IdField EventId = new IdField.Definition
        {
            MappedField = Events.Id,
            IsReadOnly = true
        };

        public static readonly IdField ProfileId = new IdField.Definition
        {
            MappedField = Profiles.Id,
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = FieldReference.List(Id, EventId, ProfileId)
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = EventId,
            ReferencedKey = Events.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = ProfileId,
            ReferencedKey = Profiles.PK
        };
    }
}
