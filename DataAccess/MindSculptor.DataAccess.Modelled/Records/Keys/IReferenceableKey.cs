using System.Collections.Generic;

namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public interface IReferenceableKey
    {
        RecordDefinition RecordDefinition { get; }
        IEnumerable<FieldReference> Fields { get; }
    }
}
