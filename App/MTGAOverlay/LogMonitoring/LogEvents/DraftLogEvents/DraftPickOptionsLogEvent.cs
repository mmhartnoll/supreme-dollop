using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.DraftLogEvents
{
    internal class DraftPickOptionsLogEvent : LogEvent
    {
        public string MtgaEventId { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public IEnumerable<int> DraftPackCardIds { get; }

        private DraftPickOptionsLogEvent(string mtgaEventId, int packNumber, int pickNumber, IEnumerable<int> draftPackCardIds)
        {
            MtgaEventId      = mtgaEventId;
            PackNumber       = packNumber;
            PickNumber       = pickNumber;
            DraftPackCardIds = draftPackCardIds.Enumerate();
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement.GetProperty("payload");
            var draftIdElement = payloadElement.GetProperty("DraftId").GetString();

            var regex = Regex.Match(draftIdElement, @"^[^:]*:([^_]*_[^_]*)_[^:]*:\w*$");
            if (regex.Success)
            {
                var mtgaEventId       = regex.Groups[1].Value;
                var packNumber        = payloadElement.GetProperty("PackNumber").GetInt32() + 1;
                var pickNumber        = payloadElement.GetProperty("PickNumber").GetInt32() + 1;
                var draftPackProperty = payloadElement.GetProperty("DraftPack");

                var draftPackCardIds = draftPackProperty.ValueKind == JsonValueKind.Null ?
                    Enumerable.Empty<int>() :
                    draftPackProperty.EnumerateArray().Select(jsonElement => Convert.ToInt32(jsonElement.GetString()));

                result = new DraftPickOptionsLogEvent(mtgaEventId, packNumber, pickNumber, draftPackCardIds);
                return true;
            }
            result = null;
            return false;
        }
    }
}
