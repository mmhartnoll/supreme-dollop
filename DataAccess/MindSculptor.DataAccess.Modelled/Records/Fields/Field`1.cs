using System;

namespace MindSculptor.DataAccess.Modelled.Records.Fields
{
    public abstract class Field<T> : Field
        where T : Field<T>
    {
        private readonly T mappedField = null!;

        public bool IsFieldMapped => mappedField != null;

        public T MappedField => mappedField != null ? 
            mappedField : 
            throw new InvalidOperationException($"'{nameof(MappedField)}' is not defined. Please check the value of '{nameof(IsFieldMapped)}' before accessing this property.");

        protected Field(Definition definition)
            : base(definition)
        {
            mappedField = definition.MappedField;
        }

        public new abstract class Definition : Field.Definition
        {
            public T MappedField { get; set; } = null!;
        }
    }
}
