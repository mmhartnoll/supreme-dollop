namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public class UniqueKey : Key, IReferenceableKey
    {
        public bool Indexed { get; }

        private UniqueKey(Definition definition) : base(definition)
            => Indexed = definition.Indexed;

        public static implicit operator UniqueKey(Definition definition)
            => new UniqueKey(definition);

        public new class Definition : Key.Definition
        {
            public bool Indexed { get; set; } = true;
        }
    }
}
