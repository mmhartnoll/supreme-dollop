using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class GameState
    {
        private readonly IDictionary<int, GameStateObjectInstance> objectInstances = new Dictionary<int, GameStateObjectInstance>();

        public int GameStateId { get; private set; }
        public int PreviousGameStateId { get; private set; }
        public IReadOnlyDictionary<int, GameStateObjectInstance> ObjectInstances { get; }

        public GameState()
            => ObjectInstances = new ReadOnlyDictionary<int, GameStateObjectInstance>(objectInstances);

        public GameStateObjectInstance CreateNewInstance(int instanceId, int ownerId, int controllerId, GameStateZone zone)
        {
            var newInstance = new GameStateObjectInstance(this, instanceId, ownerId, controllerId, zone);
            objectInstances.Add(instanceId, newInstance);
            return newInstance;
        }

        public void SetNextGameState()
        {
            PreviousGameStateId = GameStateId;
            GameStateId++;
        }

        public void RevertGameState(int targetGameStateId)
        {
            if (targetGameStateId > 0)
            {
                foreach (var (instanceId, objectInstance) in objectInstances)
                    if (!objectInstance.TryRevertObjectState(targetGameStateId))
                        objectInstances.Remove(instanceId);
                GameStateId = targetGameStateId;
                PreviousGameStateId = targetGameStateId - 1;
            }
            else
                throw new InvalidOperationException("Target game state id is not in range.");
        }
    }
}
