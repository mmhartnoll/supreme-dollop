using System;

namespace MindSculptor.DataAccess.Modelled.Records
{
    public abstract class RecordComponent : Component
    {
        public RecordDefinition RecordDefinition => Container as RecordDefinition ?? 
            throw new InvalidCastException($"RecordComponent '{GetType().Name}' was not defined in a type of '{nameof(RecordDefinition)}'.");

        protected RecordComponent(Definition definition)
            : base(definition) { }

        public new abstract class Definition : Component.Definition { }
    }
}
