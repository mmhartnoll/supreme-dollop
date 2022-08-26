using MindSculptor.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents
{
    internal class ProfileInventoryInfoLogEvent : LogEvent
    {
        public int GemCount { get; }
        public int GoldCount { get; }
        public int CommonWildcardCount { get; }
        public int UncommonWildcardCount { get; }
        public int RareWildcardCount { get; }
        public int MythicRareWildcardCount { get; }
        public decimal VaultProgress { get; }
        public IReadOnlyDictionary<int, int> BoosterCounts { get; }

        private ProfileInventoryInfoLogEvent(int gemCount, int goldCount, int commonWildcardCount, int uncommonWildcardCount, int rareWildcardCount, int mythicRareWildcardCount, decimal vaultProgress, IReadOnlyDictionary<int, int> boosterCounts)
        {
            GemCount                = gemCount;
            GoldCount               = goldCount;
            CommonWildcardCount     = commonWildcardCount;
            UncommonWildcardCount   = uncommonWildcardCount;
            RareWildcardCount       = rareWildcardCount;
            MythicRareWildcardCount = mythicRareWildcardCount;
            VaultProgress           = vaultProgress;
            BoosterCounts           = boosterCounts;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var gemCount                = payloadElement.GetProperty("gems").GetInt32();
            var goldCount               = payloadElement.GetProperty("gold").GetInt32();
            var commonWildcardCount     = payloadElement.GetProperty("wcCommon").GetInt32();
            var uncommonWildcardCount   = payloadElement.GetProperty("wcUncommon").GetInt32();
            var rareWildcardCount       = payloadElement.GetProperty("wcRare").GetInt32();
            var mythicRareWildcardCount = payloadElement.GetProperty("wcMythic").GetInt32();
            var vaultProgress           = payloadElement.GetProperty("vaultProgress").GetDecimal();

            var boostersElements = payloadElement.GetProperty("boosters").EnumerateArray();
            var boosterCounts = new ReadOnlyDictionary<int, int>(boostersElements.ToDictionary(KeySelector, ValueSelector));

            result = new ProfileInventoryInfoLogEvent(gemCount, goldCount, commonWildcardCount, uncommonWildcardCount, rareWildcardCount, mythicRareWildcardCount, vaultProgress, boosterCounts);
            return true;

            static int KeySelector(JsonElement jsonElement) => jsonElement.GetProperty("collationId").GetInt32();
            static int ValueSelector(JsonElement jsonElement) => jsonElement.GetProperty("count").GetInt32();
        }
    }
}
