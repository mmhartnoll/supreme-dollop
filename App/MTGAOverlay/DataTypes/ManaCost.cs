namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class ManaCost
    {
        public string Code { get; }
        public int Count { get; }
        public ColorIdentity ColorIdentity { get; }

        public ManaCost(string code, int count, ColorIdentity colorIdentity)
        {
            Code          = code;
            Count         = count;
            ColorIdentity = colorIdentity;
        }
    }
}
