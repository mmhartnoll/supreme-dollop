namespace MindSculptor.Tools.Data
{
    public class ManaCost
    {
        public string SymbolType { get; }
        public int Count { get; }

        public ManaCost(string symbolType, int count)
        {
            SymbolType  = symbolType;
            Count       = count;
        }
    }
}
