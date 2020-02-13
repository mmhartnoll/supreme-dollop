using System;

namespace MindSculptor.DataAccess.Modelled.Records.Fields
{
    public class IntegerField : Field<IntegerField>
    {
        private readonly long? minimumValue;
        private readonly long? maximumValue;

        public bool HasMinimumValue 
            => minimumValue != null;
        public long MinimumValue 
            => minimumValue ?? throw new InvalidOperationException($"Property '{nameof(MinimumValue)}' is not set. Please check the value of '{nameof(HasMinimumValue)}' before accessing this.");

        public bool HasMaximumValue 
            => maximumValue != null;
        public long MaximumValue 
            => maximumValue ?? throw new InvalidOperationException($"Property '{nameof(MaximumValue)}' is not set. Please check the value of '{nameof(HasMaximumValue)}' before accessing this.");

        private IntegerField(Definition definition)
            : base(definition)
        {
            minimumValue = definition.MinimumValue;
            maximumValue = definition.MaximumValue;
        }

        public static implicit operator IntegerField(Definition definition) 
            => new IntegerField(definition);

        protected override Type GetMappedDalType() 
            => typeof(int);

        public new class Definition : Field<IntegerField>.Definition
        {
            public long? MinimumValue { get; set; }
            public long? MaximumValue { get; set; }
        }
    }
}
