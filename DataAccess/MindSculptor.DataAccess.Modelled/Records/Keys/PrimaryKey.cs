namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public class PrimaryKey : Key, IReferenceableKey
    {
        public bool Clustered { get; }

        private PrimaryKey(Definition definition) : base(definition)
            => Clustered = definition.Clustered;

        public static implicit operator PrimaryKey(Definition definition)
            => new PrimaryKey(definition);

        public new class Definition : Key.Definition
        {
            public bool Clustered { get; set; } = true;
        }
    }
}
