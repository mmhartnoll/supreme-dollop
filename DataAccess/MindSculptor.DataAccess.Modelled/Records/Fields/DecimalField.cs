using System;
using System.Collections.Generic;
using System.Text;

namespace MindSculptor.DataAccess.Modelled.Records.Fields
{
    public class DecimalField : Field<DecimalField>
    {
        public int Precision { get; }
        public int Scale { get; }

        private DecimalField(Definition definition)
            : base(definition)
        {
            Precision = definition.Precision;
            Scale = definition.Scale;
        }

        public static implicit operator DecimalField(Definition definition)
            => new DecimalField(definition);

        protected override Type GetMappedDalType() 
            => typeof(decimal);

        public new class Definition : Field<DecimalField>.Definition
        {
            public int Precision { get; set; }
            public int Scale { get; set; }
        }
    }
}
