using MindSculptor.App.MtgaOverlay.Models;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class DraftPickOption
    {
        public CardModel CardModel { get; }
        public string Name { get; }
        public Rarity Rarity { get; }
        public int LogicalOrdinal { get; }
        public int InventoryCount { get; }

        public DraftPickOption(CardModel cardModel, string name, Rarity rarity, int logicalOrdinal, int inventoryCount)
        {
            CardModel      = cardModel;
            Name           = name;
            Rarity         = rarity;
            LogicalOrdinal = logicalOrdinal;
            InventoryCount = inventoryCount;
        }
    }
}
