using MindSculptor.Tools;
using System;
using System.Collections.Generic;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class MatchCreationLogEvent : LogEvent
    {
        public string MtgaEventId { get; }
        public Guid EventEntryId { get; }
        public Guid MatchId { get; }

        public string PlayerOneMtgaUserId { get; }
        public string PlayerOneName { get; }

        public string PlayerTwoMtgaUserId { get; }
        public string PlayerTwoName { get; }

        public string DeckName { get; }
        public IReadOnlyDictionary<int, int> DeckCardCounts { get; }
        public IReadOnlyDictionary<int, int> SideboardCardCounts { get; }

        private MatchCreationLogEvent(string mtgaEventId, Guid eventEntryId, Guid matchId, string playerOneMtgaUserId, string playerOneName, string playerTwoMtgaUserId, string playerTwoName, string deckName,
            IReadOnlyDictionary<int, int> deckCardCounts, IReadOnlyDictionary<int, int> sideboardCardCounts)
        {
            MtgaEventId         = mtgaEventId;
            EventEntryId        = eventEntryId;
            MatchId             = matchId;
            PlayerOneMtgaUserId = playerOneMtgaUserId;
            PlayerOneName       = playerOneName;
            PlayerTwoMtgaUserId = playerTwoMtgaUserId;
            PlayerTwoName       = playerTwoName;
            DeckName            = deckName;
            DeckCardCounts      = deckCardCounts;
            SideboardCardCounts = sideboardCardCounts;
        }

        public static bool TryCreateFromFactory(MatchCreationLogEventFactory factory, out NullableReference<LogEvent> result)
        {
            if (factory.MtgaEventId        .HasValue &&
                factory.EventEntryId       .HasValue &&
                factory.MatchId            .HasValue &&
                factory.PlayerOneMtgaUserId.HasValue &&
                factory.PlayerOneName      .HasValue &&
                factory.PlayerTwoMtgaUserId.HasValue &&
                factory.PlayerTwoName      .HasValue &&
                factory.DeckName           .HasValue &&
                factory.DeckCardCounts     .HasValue &&
                factory.SideboardCardCounts.HasValue)
            {
                result = new MatchCreationLogEvent(factory.MtgaEventId.Value, factory.EventEntryId.Value, factory.MatchId.Value, factory.PlayerOneMtgaUserId.Value, factory.PlayerOneName.Value, factory.PlayerTwoMtgaUserId.Value,
                    factory.PlayerTwoName.Value, factory.DeckName.Value, factory.DeckCardCounts.Value, factory.SideboardCardCounts.Value);
                factory.Invalidate();
                return true;
            }
            result = null;
            return true;
        }
    }
}
