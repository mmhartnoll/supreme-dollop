namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class BoosterCollectionInfo
    {
        public int MtgaBoosterId { get; }
        public string SetName { get; }
        public string SetCode { get; }
        public int TotalRareCount { get; }
        public int TotalMythicRareCount { get; }
        public int CollectedRareCount { get; }
        public int CollectedMythicRareCount { get; }
        public int InventoryBoosterCount { get; }
        public decimal ExpectedBoosterOpensToCompleteRares { get; }
        public decimal ExpectedBoosterOpensToCompleteMythicRares { get; }

        public BoosterCollectionInfo(int mtgaBoosterId, string setCode, string setName, int totalRareCount, int totalMythicRareCount, int collectedRareCount, int collectedMythicRareCount, int inventoryBoosterCount)
        {
            MtgaBoosterId            = mtgaBoosterId;
            SetCode                  = setCode;
            SetName                  = setName;
            TotalRareCount           = totalRareCount;
            TotalMythicRareCount     = totalMythicRareCount;
            CollectedRareCount       = collectedRareCount;
            CollectedMythicRareCount = collectedMythicRareCount;
            InventoryBoosterCount    = inventoryBoosterCount;

            // Mythic rares replace rares at a rate of 1:8. Rare and mythic rare wildcards replace their respective rarity at a rate of approx 1:30

            ExpectedBoosterOpensToCompleteRares       = (TotalRareCount - CollectedRareCount)             * 240 / 203m;   // (8 * 30) / (7 * 29)
            ExpectedBoosterOpensToCompleteMythicRares = (TotalMythicRareCount - CollectedMythicRareCount) * 240 / 29m;    // (8 * 30) / (1 * 29)
        }
    }
}
