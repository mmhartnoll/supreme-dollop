namespace MindSculptor.Tools.Applications.Harvester.Processing.Gatherer
{
    internal class ManaCost
    {
        public string SymbolType { get; }
        public int Count { get; }
        public int Ordinal { get; }

        private ManaCost(string symbolType, int count, int ordinal)
        {
            SymbolType = symbolType;
            Count = count;
            Ordinal = ordinal;
        }

        public static ManaCost Create(string symbolType, int count, int ordinal)
            => new ManaCost(symbolType, count, ordinal);
    }
}
