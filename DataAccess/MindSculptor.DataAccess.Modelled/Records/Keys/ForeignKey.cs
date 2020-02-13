using System;

namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public class ForeignKey : Key
    {
        public IReferenceableKey ReferencedKey { get; }
        public bool Indexed { get; }

        private ForeignKey(Definition definition)
            : base(definition)
        {
            ReferencedKey = definition.ReferencedKey ?? throw new ArgumentNullException(nameof(definition.ReferencedKey));
            Indexed = definition.Indexed;
        }

        public static implicit operator ForeignKey(Definition definition)
            => new ForeignKey(definition);

        public new class Definition : Key.Definition
        {
            public IReferenceableKey? ReferencedKey { get; set; }
            public bool Indexed { get; set; } = true;
        }
    }
}
