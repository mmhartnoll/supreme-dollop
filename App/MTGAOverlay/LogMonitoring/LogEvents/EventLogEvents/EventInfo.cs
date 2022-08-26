using MindSculptor.Tools.Exceptions;
using System;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents
{
    internal class EventInfo
    {
        private readonly Guid? activeEventId;

        public string MtgaEventId { get; }
        public bool HasActiveDraftEventEntry => activeEventId.HasValue;
        public Guid ActiveDraftEventEntryId => activeEventId.HasValue ? activeEventId.Value : throw new PropertyUndefinedException(nameof(ActiveDraftEventEntryId), nameof(HasActiveDraftEventEntry));

        public EventInfo(string mtgaEventId, Guid? activeEventId)
        {
            MtgaEventId        = mtgaEventId;
            this.activeEventId = activeEventId;
        }
    }
}
