using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class UpdateProfileInventoryLogEvent : LogEvent
    {
        public int MythicRareWildcardCount { get; }
        public int RareWildcardCount { get; }
        public int UncommonWildcardCount { get; }
        public int CommonWildcardCount { get; }

        public int GoldCount { get; }
        public int GemCount { get; }

        public IReadOnlyDictionary<int, int> BoosterCounts { get; }

        private UpdateProfileInventoryLogEvent(int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, IReadOnlyDictionary<int, int> boosterCounts)
        {
            MythicRareWildcardCount = mythicRareWildcardCount;
            RareWildcardCount       = rareWildcardCount;
            UncommonWildcardCount   = uncommonWildcardCount;
            CommonWildcardCount     = commonWildcardCount;

            GoldCount               = goldCount;
            GemCount                = gemCount;

            BoosterCounts           = boosterCounts;
        }

        public static UpdateProfileInventoryLogEvent FromJson(JsonElement jsonElement)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var mythicRareWildcardCount = payloadElement.GetProperty("wcMythic").GetInt32();
            var rareWildcardCount       = payloadElement.GetProperty("wcRare").GetInt32();
            var uncommonWildcardCount   = payloadElement.GetProperty("wcUncommon").GetInt32();
            var commonWildcardCount     = payloadElement.GetProperty("wcCommon").GetInt32();

            var goldCount               = payloadElement.GetProperty("gold").GetInt32();
            var gemCount                = payloadElement.GetProperty("gems").GetInt32();

            var boostersElements = payloadElement.GetProperty("boosters").EnumerateArray();
            var boosterCounts = new ReadOnlyDictionary<int, int>(boostersElements.ToDictionary(KeySelector, ValueSelector));

            return new UpdateProfileInventoryLogEvent(mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, boosterCounts);

            static int KeySelector(JsonElement jsonElement) => jsonElement.GetProperty("collationId").GetInt32();
            static int ValueSelector(JsonElement jsonElement) => jsonElement.GetProperty("count").GetInt32();
        }
    }
}
