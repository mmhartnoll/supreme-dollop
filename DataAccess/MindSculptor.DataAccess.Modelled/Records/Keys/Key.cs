using MindSculptor.Tools.Extensions;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public abstract class Key : RecordComponent
    {
        private readonly FieldReferenceList fields;

        public IEnumerable<FieldReference> Fields => fields.Enumerate();

        protected Key(Definition definition) : base(definition)
            => fields = definition.Fields;

        public new abstract class Definition : RecordComponent.Definition
        {
            public FieldReferenceList Fields { get; set; } = FieldReference.List();
        }
    }
}
