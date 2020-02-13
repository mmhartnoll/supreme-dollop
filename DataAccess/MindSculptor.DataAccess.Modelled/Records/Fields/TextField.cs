using System;

namespace MindSculptor.DataAccess.Modelled.Records.Fields
{
    public class TextField : Field<TextField>
    {
        private readonly int? maximumLength;

        public int MinimumLength { get; }

        public bool HasMaximumLength 
            => maximumLength != null;
        public int MaximumLength 
            => maximumLength ?? throw new InvalidOperationException($"Property '{nameof(MaximumLength)}' is not set. Please check the value of '{nameof(HasMaximumLength)}' before accessing this.");

        public bool AllowWhiteSpace { get; }

        private TextField(Definition definition)
            : base(definition)
        {
            MinimumLength = definition.MinimumLength;
            maximumLength = definition.MaximumLength;
            AllowWhiteSpace = definition.AllowWhiteSpace;

            if (MinimumLength < 0)
                throw new ArgumentException($"Parameter '{nameof(definition.MinimumLength)}' should be non-negative.");
            if (HasMaximumLength && MaximumLength < MinimumLength)
                throw new ArgumentException($"Parameter '{nameof(definition.MaximumLength)}', if set, should not be less than the parameter '{nameof(definition.MinimumLength)}' ({nameof(definition.MinimumLength)} = {MinimumLength}).");
        }

        public static implicit operator TextField(Definition definition)
            => new TextField(definition);

        protected override Type GetMappedDalType()
            => typeof(string);

        public new class Definition : Field<TextField>.Definition
        {
            public int MinimumLength { get; set; } = 0;
            public int? MaximumLength { get; set; }
            public bool AllowWhiteSpace { get; set; } = false;
        }
    }
}
