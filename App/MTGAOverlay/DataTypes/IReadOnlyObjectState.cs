namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal interface IReadOnlyObjectState
    {
        public int GameStateId { get; }
        public int ControllerId { get; }
        public GameStateZone Zone { get; }
        public bool IsSubZoneSpecified { get; }
        public GameStateSubZone SubZone { get; }
        public int SubZoneOrdinal { get; }
        public bool IsSubZoneInternallyOrdered { get; }
        public int SubZoneInternalOrdinal { get; }
        public bool IsObjectKnown { get; }
        public int MtgaCardId { get; }
        public bool IsDeleted { get; }
    }
}
