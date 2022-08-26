using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents
{
    internal class ProfileInventoryUpdateLogEvent : LogEvent
    {
        public int GemsDelta { get; }
        public int GoldDelta { get; }
        public IReadOnlyDictionary<int, int> BoostersDelta { get; }
        public IEnumerable<int> CardIdsAdded { get; }
        public decimal VaultProgressDelta { get; }

        public int CommonWildcardDelta { get; }
        public int UncommonWildcardDelta { get; }
        public int RareWildcardDelta { get; }
        public int MythicRareWildcardDelta { get; }

        private ProfileInventoryUpdateLogEvent(int gemsDelta, int goldDelta, IReadOnlyDictionary<int, int> boostersDelta, IEnumerable<int> cardIdsAdded, decimal vaultProgressDelta, int commonWildCardDelta, int uncommonWildcardDelta, 
            int rareWildcardDelta, int mythicRareWildcardDelta)
        {
            GemsDelta               = gemsDelta;
            GoldDelta               = goldDelta;
            BoostersDelta           = boostersDelta;
            CardIdsAdded            = cardIdsAdded;
            VaultProgressDelta      = vaultProgressDelta;
            CommonWildcardDelta     = commonWildCardDelta;
            UncommonWildcardDelta   = uncommonWildcardDelta;
            RareWildcardDelta       = rareWildcardDelta;
            MythicRareWildcardDelta = mythicRareWildcardDelta;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var context = payloadElement.GetProperty("context").GetString();
            if (context != "Event.GrantCardPool")
            {
                int gemsDelta = 0, goldDelta = 0, mythicRareWildcardDelta = 0, rareWildcardDeltaDelta = 0, uncommonWildcardDelta = 0, commonWildcardDelta = 0;
                decimal vaultProgressDelta = 0;

                var boostersDelta = new Dictionary<int, int>();
                var addedCardIds = new List<int>();

                var updateElements = payloadElement.GetProperty("updates")
                    .EnumerateArray();
                foreach (var updateElement in updateElements)
                {
                    var deltaElement         = updateElement.GetProperty("delta");

                    gemsDelta               += deltaElement.GetProperty("gemsDelta")         .GetInt32();
                    goldDelta               += deltaElement.GetProperty("goldDelta")         .GetInt32();
                    vaultProgressDelta      += deltaElement.GetProperty("vaultProgressDelta").GetDecimal();
                    mythicRareWildcardDelta += deltaElement.GetProperty("wcMythicDelta")     .GetInt32();
                    rareWildcardDeltaDelta  += deltaElement.GetProperty("wcRareDelta")       .GetInt32();
                    uncommonWildcardDelta   += deltaElement.GetProperty("wcUncommonDelta")   .GetInt32();
                    commonWildcardDelta     += deltaElement.GetProperty("wcCommonDelta")     .GetInt32();

                    foreach (var boosterDeltaElement in deltaElement.GetProperty("boosterDelta").EnumerateArray())
                    {
                        var boosterId = boosterDeltaElement.GetProperty("collationId").GetInt32();
                        var count     = boosterDeltaElement.GetProperty("count")      .GetInt32();

                        if (boostersDelta.ContainsKey(boosterId))
                            boostersDelta[boosterId] += count;
                        else
                            boostersDelta.Add(boosterId, count);
                    }

                    foreach (var addedCardId in deltaElement.GetProperty("cardsAdded").EnumerateArray().Select(element => element.GetInt32()))
                        addedCardIds.Add(addedCardId);
                }

                result = new ProfileInventoryUpdateLogEvent(gemsDelta, goldDelta, new ReadOnlyDictionary<int, int>(boostersDelta), addedCardIds.Enumerate(), vaultProgressDelta, commonWildcardDelta, uncommonWildcardDelta, rareWildcardDeltaDelta, mythicRareWildcardDelta);
                return true;
            }
            result = null;
            return false;
        }
    }
}
