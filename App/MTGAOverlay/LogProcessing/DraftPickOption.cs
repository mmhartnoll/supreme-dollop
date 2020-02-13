namespace MindSculptor.App.MtgaOverlay.LogProcessing
{
    internal class DraftPickOption
    {
        public string Name { get; }
        public Rarity Rarity { get; }
        public int InventoryCount { get; }

        private DraftPickOption(string name, Rarity rarity, int inventoryCount)
        {
            Name = name;
            Rarity = rarity;
            InventoryCount = inventoryCount;
        }

        public static DraftPickOption Create(string name, Rarity rarity, int inventoryCount)
            => new DraftPickOption(name, rarity, inventoryCount);
    }
}
