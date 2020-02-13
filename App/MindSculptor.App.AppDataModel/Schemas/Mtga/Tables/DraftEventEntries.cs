using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class DraftEventEntries : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;
        public override string SingularName => "DraftEventEntry";


        public static readonly IdField Id = new IdField.Definition
        {
            IsReadOnly = true
        };

        public static readonly IdField DraftEventId = new IdField.Definition
        {
            MappedField = DraftEvents.Id,
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
            Fields = FieldReference.List(Id, DraftEventId, ProfileId)
        };

        public static ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = DraftEventId,
            ReferencedKey = DraftEvents.PK
        };

        public static ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = ProfileId,
            ReferencedKey = Profiles.PK
        };
    }
}
