using System;

namespace MindSculptor.DataAccess.Modelled.Records.Fields
{
    public abstract class Field : RecordComponent
    {
        public bool IsNullable { get; }
        public bool IsReadOnly { get; }

        public Type MappedDalType => GetMappedDalType();

        protected Field(Definition definition)
            : base(definition)
        {
            IsNullable = definition.IsNullable;
            IsReadOnly = definition.IsReadOnly;
        }

        protected abstract Type GetMappedDalType();

        public new abstract class Definition : RecordComponent.Definition
        {
            public bool IsNullable { get; set; } = false;
            public bool IsReadOnly { get; set; } = false;
        }
    }
}
