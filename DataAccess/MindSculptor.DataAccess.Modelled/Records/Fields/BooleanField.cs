using System;

namespace MindSculptor.DataAccess.Modelled.Records.Fields
{
    public class BooleanField : Field<BooleanField>
    {
        private BooleanField(Definition definition)
            : base(definition) { }

        public static implicit operator BooleanField(Definition definition)
            => new BooleanField(definition);

        protected override Type GetMappedDalType()
            => typeof(bool);

        public new class Definition : Field<BooleanField>.Definition { }
    }
}
