using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class GameStateObjectInstance
    {
        private readonly GameState gameState;
        private readonly Stack<ObjectState> objectStates = new Stack<ObjectState>();

        private NullableReference<IReadOnlyObjectState> previousState = null;

        private ObjectState CurrentState => objectStates.Peek();
        public int InstanceId { get; }
        public int OwnerId { get; }

        public int ControllerId
        {
            get => CurrentState.ControllerId;
            set
            {
                if (CurrentState.ControllerId != value)
                {
                    if (CurrentState.GameStateId != gameState.GameStateId)
                        CreateNextState();
                    CurrentState.ControllerId = value;
                }
            }
        }

        public GameStateZone Zone
        {
            get => CurrentState.Zone;
            set
            {
                if (CurrentState.Zone != value)
                {
                    if (CurrentState.GameStateId != gameState.GameStateId)
                        CreateNextState();
                    CurrentState.Zone = value;
                    CurrentState.ClearSubZone();
                }
            }
        }

        public bool IsSubZoneSpecified => CurrentState.IsSubZoneSpecified;

        public GameStateSubZone SubZone
        {
            get => CurrentState.IsSubZoneSpecified ? CurrentState.SubZone : throw new PropertyUndefinedException(nameof(SubZone), nameof(IsSubZoneSpecified));
            set 
            {
                if (!CurrentState.IsSubZoneSpecified || CurrentState.SubZone != value)
                {
                    if (CurrentState.GameStateId != gameState.GameStateId)
                        CreateNextState();
                    CurrentState.SubZone = value;
                }
            }
        }

        public int SubZoneOrdinal
        {
            get => CurrentState.IsSubZoneSpecified ? CurrentState.SubZoneOrdinal : throw new PropertyUndefinedException(nameof(SubZoneOrdinal), nameof(IsSubZoneSpecified));
            set
            {
                if (!CurrentState.IsSubZoneSpecified || CurrentState.SubZoneOrdinal != value)
                {
                    if (CurrentState.GameStateId != gameState.GameStateId)
                        CreateNextState();
                    CurrentState.SubZoneOrdinal = value;
                }
            }
        }

        public bool IsSubZoneInternallyOrdered => CurrentState.IsSubZoneInternallyOrdered;

        public int SubZoneInternalOrdinal
        {
            get => CurrentState.IsSubZoneInternallyOrdered ? CurrentState.SubZoneInternalOrdinal : throw new PropertyUndefinedException(nameof(SubZoneInternalOrdinal), nameof(IsSubZoneInternallyOrdered));
            set
            {
                if (!CurrentState.IsSubZoneInternallyOrdered || CurrentState.SubZoneInternalOrdinal != value)
                {
                    if (CurrentState.GameStateId != gameState.GameStateId)
                        CreateNextState();
                    CurrentState.SubZoneInternalOrdinal = value;
                }
            }
        }

        public bool IsObjectKnown => CurrentState.IsObjectKnown;

        public int MtgaCardId
        {
            get => IsObjectKnown ? CurrentState.MtgaCardId : throw new PropertyUndefinedException(nameof(MtgaCardId), nameof(IsObjectKnown));
            set
            {
                if (CurrentState.IsObjectKnown)
                    throw new Exception($"Property '{nameof(MtgaCardId)}' has already been set for this instance.");
                if (CurrentState.GameStateId != gameState.GameStateId)
                    CreateNextState();
                CurrentState.MtgaCardId = value;
            }
        }

        public bool IsNew => CurrentState.GameStateId == gameState.GameStateId && objectStates.Count == 1;
        public bool IsUpdated => CurrentState.GameStateId == gameState.GameStateId && objectStates.Count > 1;
        public bool IsDeleted => CurrentState.IsDeleted;

        public bool HasPreviousState => previousState.HasValue;

        public IReadOnlyObjectState PreviousState => previousState.HasValue ? previousState.Value : throw new PropertyUndefinedException(nameof(PreviousState), nameof(HasPreviousState));

        public bool HasPreviousInstance => CurrentState.HasPreviousInstance;

        public GameStateObjectInstance PreviousInstance
        {
            get => CurrentState.HasPreviousInstance ? CurrentState.PreviousInstance : throw new PropertyUndefinedException(nameof(PreviousInstance), nameof(HasPreviousInstance));
            set
            {
                if (CurrentState.HasPreviousInstance)
                    throw new Exception($"Property '{nameof(PreviousInstance)}' has already been set for this instance.");
                if (CurrentState.GameStateId != gameState.GameStateId)
                    CreateNextState();
                CurrentState.PreviousInstance = value;
            }
        }

        public GameStateObjectInstance(GameState gameState, int instanceId, int ownerId, int controllerId, GameStateZone zone)
        {
            this.gameState = gameState;
            InstanceId = instanceId;
            OwnerId = ownerId;
            objectStates.Push(ObjectState.CreateInitialState(gameState.GameStateId, controllerId, zone, null));
        }

        public void SetAsDeleted()
        {
            if (CurrentState.IsDeleted)
                throw new Exception("Instance has already been deleted.");
            if (CurrentState.GameStateId != gameState.GameStateId)
                CreateNextState();
            CurrentState.SetAsDeleted();
        }

        public bool TryRevertObjectState(int targetGameStateId)
        {
            while (objectStates.Any() && CurrentState.GameStateId > targetGameStateId)
                objectStates.Pop();
            return objectStates.Any();
        }

        public IReadOnlyObjectState AsReadOnlyObjectState()
            => CurrentState.AsReadOnly();

        private void CreateNextState()
        {
            previousState = NullableReference.FromValue(CurrentState.AsReadOnly());
            objectStates.Push(CurrentState.CreateNextState(gameState.GameStateId));
        }

        private class ObjectState
        {
            private int? mtgaCardId = null;
            private GameStateSubZone? subZone = null;
            private int? subZoneOrdinal = null;
            private int? subZoneInternalOrdinal = null;
            private NullableReference<GameStateObjectInstance> previousInstance = null;

            public int GameStateId { get; }
            public int ControllerId { get; set; }
            public GameStateZone Zone { get; set; }
            public bool IsSubZoneSpecified => subZone.HasValue;
            public GameStateSubZone SubZone
            {
                get => subZone ?? throw new PropertyUndefinedException(nameof(SubZone), nameof(IsSubZoneSpecified));
                set => subZone = value;
            }
            public int SubZoneOrdinal 
            { 
                get => subZoneOrdinal ?? throw new PropertyUndefinedException(nameof(SubZoneOrdinal), nameof(IsSubZoneSpecified));
                set => subZoneOrdinal = value; 
            }
            public bool IsSubZoneInternallyOrdered => subZoneInternalOrdinal.HasValue;
            public int SubZoneInternalOrdinal
            {
                get => subZoneInternalOrdinal ?? throw new PropertyUndefinedException(nameof(SubZoneInternalOrdinal), nameof(IsSubZoneInternallyOrdered));
                set => subZoneInternalOrdinal = value;
            }
            public bool IsObjectKnown => mtgaCardId.HasValue;
            public int MtgaCardId
            {
                get => mtgaCardId ?? throw new PropertyUndefinedException(nameof(MtgaCardId), nameof(IsObjectKnown));
                set => mtgaCardId = value;
            }
            public bool IsDeleted { get; private set; } = false;

            public bool HasPreviousInstance => previousInstance.HasValue;

            public GameStateObjectInstance PreviousInstance
            {
                get => previousInstance.HasValue ? previousInstance.Value : throw new PropertyUndefinedException(nameof(PreviousInstance), nameof(HasPreviousInstance));
                set => previousInstance = value;
            }

            private ObjectState(int gameStateId, int controllerId, GameStateZone zone, int? mtgaCardId)
            {
                GameStateId = gameStateId;
                ControllerId = controllerId;
                Zone = zone;
                this.mtgaCardId = mtgaCardId;
            }

            public static ObjectState CreateInitialState(int currentGameStateId, int controllerId, GameStateZone zone, int? mtgaCardId)
                => new ObjectState(currentGameStateId, controllerId, zone, mtgaCardId);

            public ObjectState CreateNextState(int currentGameStateId)
                => new ObjectState(currentGameStateId, ControllerId, Zone, mtgaCardId);

            public IReadOnlyObjectState AsReadOnly() 
                => new ReadOnlyObjectState(this);

            public void ClearSubZone()
            {
                subZone = null;
                subZoneOrdinal = null;
                subZoneInternalOrdinal = null;
            }

            public void SetAsDeleted()
                => IsDeleted = true;

            private class ReadOnlyObjectState : IReadOnlyObjectState
            {
                private readonly ObjectState objectState;

                public int GameStateId => objectState.GameStateId;
                public int ControllerId => objectState.ControllerId;
                public GameStateZone Zone => objectState.Zone;
                public bool IsSubZoneSpecified => objectState.IsSubZoneSpecified;
                public GameStateSubZone SubZone => objectState.IsSubZoneSpecified ? objectState.SubZone : throw new PropertyUndefinedException(nameof(SubZone), nameof(IsSubZoneSpecified));
                public int SubZoneOrdinal => objectState.IsSubZoneSpecified ? objectState.SubZoneOrdinal : throw new PropertyUndefinedException(nameof(SubZoneOrdinal), nameof(IsSubZoneSpecified));
                public bool IsSubZoneInternallyOrdered => objectState.IsSubZoneInternallyOrdered;
                public int SubZoneInternalOrdinal => objectState.IsSubZoneInternallyOrdered ? objectState.SubZoneInternalOrdinal : throw new PropertyUndefinedException(nameof(SubZoneInternalOrdinal), nameof(IsSubZoneInternallyOrdered));
                public bool IsObjectKnown => objectState.IsObjectKnown;
                public int MtgaCardId => objectState.IsObjectKnown ? objectState.MtgaCardId : throw new PropertyUndefinedException(nameof(MtgaCardId), nameof(IsObjectKnown));
                public bool IsDeleted => objectState.IsDeleted;

                public ReadOnlyObjectState(ObjectState objectState)
                    => this.objectState = objectState;
            }
        }
    }
}
