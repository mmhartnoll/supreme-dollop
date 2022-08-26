using MindSculptor.Tools;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents
{
    internal class ProfileActiveLogEvent : LogEvent
    {
        public string MtgaUserId { get; }
        public string Name { get; }
        public int NameId { get; }

        private ProfileActiveLogEvent(string userId, string name, int nameId)
        {
            MtgaUserId = userId;
            Name       = name;
            NameId     = nameId;
        }

        public static bool TryCreateFromFactory(ProfileActiveLogEventFactory factory, out NullableReference<LogEvent> result)
        {
            if (factory.MtgaUserId.HasValue && factory.Name.HasValue && factory.NameId.HasValue)
            {
                result = new ProfileActiveLogEvent(factory.MtgaUserId.Value, factory.Name.Value, factory.NameId.Value);
                factory.Invalidate();
                return true;
            }
            result = null;
            return false;
        }
    }
}
