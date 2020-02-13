using MindSculptor.Tools.Extensions;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class CreateDraftMatchLogEvent : LogEvent
    {
        public Guid MatchId { get; }
        public DraftEventType DraftEventType { get; }
        public string SetCode { get; }
        public string OpponentScreenName { get; }
        public int OpponentUserId { get; }

        private CreateDraftMatchLogEvent(Guid matchId, DraftEventType draftEventType, string setCode, string opponentScreenName, int opponentUserId)
        {
            MatchId             = matchId;
            DraftEventType      = draftEventType;
            SetCode             = setCode;
            OpponentScreenName  = opponentScreenName;
            OpponentUserId      = opponentUserId;
        }

        public static CreateDraftMatchLogEvent FromJson(JsonElement jsonElement)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var matchId = Guid.Parse(payloadElement.GetProperty("matchId").GetString());

            var eventIdElement = payloadElement.GetProperty("eventId")
                .GetString();

            var regex = Regex.Match(eventIdElement, @"^(\w*?)_(\w{0,3})_\d*$");
            if (!regex.Success)
                throw new Exception($"Regex failure while creating '{nameof(DraftPickOptionsLogEvent)}' with unmatchable value '{eventIdElement}'.");

            var rawEventType = regex.Groups[1].Value;
            var draftEventType = rawEventType switch
            {
                "CompDraft" => DraftEventType.Traditional,
                "QuickDraft" => DraftEventType.Ranked,
                _ => throw new NotSupportedException($"Event type '{rawEventType}' is not supported.")
            };

            var setCode = regex.Groups[2].Value;

            var opponentScreenNameFull = payloadElement.GetProperty("opponentScreenName").GetString().Split('#');
            var opponentScreenName = opponentScreenNameFull[0];
            var opponentUserId = Convert.ToInt32(opponentScreenNameFull[1]);

            return new CreateDraftMatchLogEvent(matchId, draftEventType, setCode, opponentScreenName, opponentUserId);
        }
    }
}
