using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class GameStateUpdateLogEventFactory
    {
        private readonly IDictionary<int, ResultType> gameResults = new Dictionary<int, ResultType>();

        private GameStateZone PlayerLibraryZone => PlayerSeatId switch
        {
            1 => GameStateZone.PlayerOneLibrary,
            2 => GameStateZone.PlayerTwoLibrary,
            _ => throw new InvalidOperationException($"Library zone is not defined for player seat id {PlayerSeatId}.")
        };

        public Guid? MatchId { get; private set; } = null;
        public int? PlayerSeatId { get; private set; } = null;
        public int? GameNumber { get; private set; } = null;
        public GameState GameState { get; } = new GameState();
        public GameStage? GameStage { get; private set; } = null;

        public IReadOnlyDictionary<int, ResultType> GameResults { get; }
        public ResultType? MatchResult { get; private set; } = null;

        public NullableReference<IEnumerable<LogEvent>> QueuedGameStateUpdateLogEvents { get; private set; } = null;

        public GameStateUpdateLogEventFactory()
            => GameResults = new ReadOnlyDictionary<int, ResultType>(gameResults);

        public bool TryParseAggregateGameStateMessage(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var logEvents = new List<LogEvent>();
            var messageElements = jsonElement.GetProperty("greToClientEvent")
                .GetProperty("greToClientMessages")
                .EnumerateArray();
            foreach (var messageElement in messageElements)
                if (messageElement.TryGetProperty("type", out var messageTypeElement) && messageTypeElement.GetString() == MessageTypeGameStateMessage)
                    if (TryParseGameStateMessage(messageElement, out var gameStateUpdateLogEvent))
                        logEvents.Add(gameStateUpdateLogEvent.Value);
            QueuedGameStateUpdateLogEvents = NullableReference.FromValue(logEvents.Enumerate());
            return AggregateGameStateUpdateLogEvent.TryCreateFromFactory(this, out result);
        }

        private bool TryParseGameStateMessage(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var deletedInstanceIdExclusions = new HashSet<int>(); // need to record exclusions as reported deleted instances is unreliable.

            var gameStateMessageElement = jsonElement.GetProperty("gameStateMessage");

            if (gameStateMessageElement.TryGetProperty("prevGameStateId", out var prevGameStateIdElement))
            {
                var prevGameStateId = prevGameStateIdElement.GetInt32();
                if (prevGameStateId != GameState.GameStateId)
                    GameState.RevertGameState(prevGameStateId);
            }
            GameState.SetNextGameState();

            if (gameStateMessageElement.TryGetProperty("gameInfo", out var gameInfoElement))
            {
                var matchId = gameInfoElement.GetProperty("matchID").GetGuid();
                if (!MatchId.HasValue || MatchId != matchId)
                {
                    MatchId = matchId;
                    PlayerSeatId = jsonElement.GetProperty("systemSeatIds")
                        .EnumerateArray()
                        .Single()
                        .GetInt32();
                }

                var gameNumber = gameInfoElement.GetProperty("gameNumber").GetInt32();
                if (!GameNumber.HasValue || GameNumber != gameNumber)
                    GameNumber = gameNumber;

                GameStage = gameInfoElement.GetProperty("stage").GetString() switch
                {
                    "GameStage_Start" => MatchLogEvents.GameStage.Start,
                    "GameStage_Play" => MatchLogEvents.GameStage.Play,
                    "GameStage_GameOver" => MatchLogEvents.GameStage.Complete,
                    _ => throw new InvalidOperationException()
                };

                if (gameInfoElement.TryGetProperty("results", out var resultsElement))
                {
                    if (!PlayerSeatId.HasValue)
                        throw new Exception($"Property '{nameof(PlayerSeatId)}' has not been set.");

                    var i = 1;
                    using var enumerator = resultsElement.EnumerateArray().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var resultElement = enumerator.Current;
                        var resultScope = resultElement.GetProperty("scope").GetString();
                        if (resultScope == MatchScopeGame)
                        {
                            ResultType gameResult;
                            var resultType = resultElement.GetProperty("result").GetString();
                            if (resultType == ResultTypeWinLoss)
                            {
                                var winningTeamId = resultElement.GetProperty("winningTeamId").GetInt32();
                                gameResult = winningTeamId == PlayerSeatId.Value ? ResultType.Win : ResultType.Loss;
                            }
                            else if (resultType == ResultTypeDraw)
                                gameResult = ResultType.Draw;
                            else
                                throw new InvalidOperationException($"Result type '{resultType}' is unrecognized.");

                            if (!GameResults.TryGetValue(i, out var existingGameResult))
                                gameResults.Add(i++, gameResult);
                            else if (existingGameResult != gameResult)
                                throw new Exception("Existing game result does not match the result specified in the game state update message.");
                        }
                        else if (resultScope == MatchScopeMatch)
                        {
                            ResultType matchResult;
                            var resultType = resultElement.GetProperty("result").GetString();
                            if (resultType == ResultTypeWinLoss)
                            {
                                var winningTeamId = resultElement.GetProperty("winningTeamId").GetInt32();
                                matchResult = winningTeamId == PlayerSeatId.Value ? ResultType.Win : ResultType.Loss;
                            }
                            else if (resultType == ResultTypeDraw)
                                matchResult = ResultType.Draw;
                            else
                                throw new InvalidOperationException($"Result type '{resultType}' is unrecognized.");
                            if (!MatchResult.HasValue)
                                MatchResult = matchResult;
                            else if (MatchResult.Value != matchResult)
                                throw new Exception("Existing match result does not match the result specified in the game state update message.");
                        }
                    }
                }
            }

            if (gameStateMessageElement.TryGetProperty("gameObjects", out var gameObjectsElement))
                foreach (var gameObjectElement in gameObjectsElement.EnumerateArray())
                    if (gameObjectElement.TryGetProperty("type", out var typeElement))
                        if (gameObjectElement.TryGetProperty("zoneId", out var zoneIdElement))
                        {
                            var type = typeElement.GetString();
                            var instanceId = gameObjectElement.GetProperty("instanceId").GetInt32();
                            if (type == GameObjectTypeCard)
                            {
                                var ownerId = gameObjectElement.GetProperty("ownerSeatId").GetInt32();
                                var controllerId = gameObjectElement.GetProperty("controllerSeatId").GetInt32();
                                var zone = (GameStateZone)zoneIdElement.GetInt32();

                                if (!GameState.ObjectInstances.TryGetValue(instanceId, out var objectInstance))
                                    objectInstance = GameState.CreateNewInstance(instanceId, ownerId, controllerId, zone);
                                objectInstance.ControllerId = controllerId;
                                objectInstance.Zone = zone;
                                if (!objectInstance.IsObjectKnown)
                                    objectInstance.MtgaCardId = gameObjectElement.GetProperty("grpId").GetInt32();
                            }
                        }

            if (gameStateMessageElement.TryGetProperty("zones", out var zonesElement))
                foreach (var zoneElement in zonesElement.EnumerateArray())
                    if (zoneElement.TryGetProperty("zoneId", out var zoneIdElement) && zoneElement.TryGetProperty("objectInstanceIds", out var objectInstanceIds))
                    {
                        var zoneId = zoneIdElement.GetInt32();
                        var zone = (GameStateZone)zoneId;
                        if (!zone.In(GameStateZone.Limbo, GameStateZone.Stack, GameStateZone.Pending, GameStateZone.Command))
                        {
                            var instanceIds = objectInstanceIds.EnumerateArray()
                            .Select(element => element.GetInt32());
                            foreach (var instanceId in instanceIds)
                            {
                                deletedInstanceIdExclusions.Add(instanceId);
                                if (!GameState.ObjectInstances.TryGetValue(instanceId, out var objectInstance))
                                {
                                    if (!zoneElement.TryGetProperty("ownerSeatId", out var ownerSeatIdElement))
                                        throw new Exception("Attempting to add an instance of an unknown card in a zone without an owner id.");
                                    var ownerId = ownerSeatIdElement.GetInt32();
                                    objectInstance = GameState.CreateNewInstance(instanceId, ownerId, ownerId, zone);
                                }
                                else
                                {
                                    objectInstance.Zone = zone;
                                }
                            }
                        }
                    }

            if (gameStateMessageElement.TryGetProperty("annotations", out var annotationsElement))
                foreach (var annotationElement in annotationsElement.EnumerateArray())
                {
                    var annotationTypes = annotationElement.GetProperty("type")
                        .EnumerateArray()
                        .Select(element => element.GetString())
                        .ToList();
                    if (annotationTypes.Contains(AnnotationTypeObjectIdChanged))
                    {
                        var detailsElements = annotationElement.GetProperty("details")
                            .EnumerateArray()
                            .ToDictionary(element => element.GetProperty("key").GetString());
                        var originalId = detailsElements["orig_id"]
                            .GetProperty("valueInt32")
                            .EnumerateArray()
                            .Single()
                            .GetInt32();
                        var newId = detailsElements["new_id"]
                            .GetProperty("valueInt32")
                            .EnumerateArray()
                            .Single()
                            .GetInt32();
                        if (!GameState.ObjectInstances.TryGetValue(originalId, out var originalInstance))
                            throw new Exception("Original instance could not be found.");
                        if (!GameState.ObjectInstances.TryGetValue(newId, out var newInstance))
                            throw new Exception("Original instance could not be found.");
                        if (!originalInstance.IsDeleted)
                            originalInstance.SetAsDeleted();
                        newInstance.PreviousInstance = originalInstance;
                    }
                    if (annotationTypes.Contains(AnnotationTypeShuffle))
                    {
                        var detailsElements = annotationElement.GetProperty("details")
                            .EnumerateArray()
                            .ToDictionary(element => element.GetProperty("key").GetString());
                        var originalIds = detailsElements["OldIds"]
                            .GetProperty("valueInt32")
                            .EnumerateArray()
                            .Select(element => element.GetInt32());
                        foreach (var instanceId in originalIds)
                        {
                            if (!GameState.ObjectInstances.TryGetValue(instanceId, out var originalInstance))
                                throw new Exception("Original instance could not be found.");
                            if (!originalInstance.IsDeleted)
                                originalInstance.SetAsDeleted();
                        }
                    }
                }

            if (gameStateMessageElement.TryGetProperty("diffDeletedInstanceIds", out var deletedInstanceIdsElement))
            {
                var deletedInstanceIds = deletedInstanceIdsElement.EnumerateArray()
                    .Select(element => element.GetInt32())
                    .Where(instanceId => !deletedInstanceIdExclusions.Contains(instanceId));
                foreach (var instanceId in deletedInstanceIds)
                {
                    if (GameState.ObjectInstances.TryGetValue(instanceId, out var instance) && !instance.IsDeleted)
                        instance.SetAsDeleted();
                }
            }

            // Normalize sub zone ordering
            foreach (var subZone in new[] { GameStateSubZone.Top, GameStateSubZone.Bottom })
            {
                var subZoneGroups = GameState.ObjectInstances.Values
                    .Where(instance => !instance.IsDeleted && instance.IsSubZoneSpecified && instance.SubZone == subZone)
                    .GroupBy(instance => instance.SubZoneOrdinal);
                if (subZoneGroups.Any())
                {
                    var minimumOuterOrdinal = subZoneGroups.Min(group => group.Key);
                    foreach (var subZoneGroup in subZoneGroups.Select(group => group.Enumerate()))
                    {
                        var minimumInnerOrdinal = subZoneGroup.Min(instance => instance.IsSubZoneInternallyOrdered ? instance.SubZoneInternalOrdinal : 0);
                        foreach (var instance in subZoneGroup)
                        {
                            instance.SubZoneOrdinal -= minimumOuterOrdinal;
                            if (instance.IsSubZoneInternallyOrdered)
                                instance.SubZoneInternalOrdinal -= minimumInnerOrdinal;
                        }
                    }
                }
            }

            return GameStateUpdateLogEvent.TryCreateFromFactory(this, out result);
        }

        public bool ParseGroupByMessage(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement.GetProperty("payloadElement");

            var gameStateId = payloadElement.GetProperty("gameStateId").GetInt32();
            if (GameState.GameStateId != gameStateId)
                throw new Exception("Attempting to apply sub zone groups for the wrong game state id.");
            var groupRespElement = payloadElement.GetProperty("groupResp");
            var groupsElement = groupRespElement.GetProperty("groups");

            var isOrderedGroup = groupRespElement.GetProperty("groupType").GetString() == GroupTypeOrdered;

            foreach (var groupElement in groupsElement.EnumerateArray())
                if (groupElement.TryGetProperty("zoneType", out var zoneTypeElement) && zoneTypeElement.GetString() == ZoneTypeLibrary)
                {
                    var subZone = groupElement.GetProperty("subZoneType").GetString() switch
                    {
                        SubZoneTypeBottom => GameStateSubZone.Bottom,
                        SubZoneTypeTop => GameStateSubZone.Top,
                        _ => throw new InvalidOperationException()
                    };

                    var instanceIds = groupElement.GetProperty("ids").EnumerateArray()
                        .Select(element => element.GetInt32());
                    var existingInstances = GameState.ObjectInstances.Values
                        .Where(instance =>
                            instance.Zone == PlayerLibraryZone &&
                            instance.IsSubZoneSpecified &&
                            instance.SubZone == subZone);

                    var outerOrdinal = subZone switch
                    {
                        GameStateSubZone.Top    => 0,
                        GameStateSubZone.Bottom => existingInstances.Max(instance => instance.SubZoneOrdinal) + 1,
                        _                       => throw new InvalidOperationException()
                    };

                    if (subZone == GameStateSubZone.Top)
                        foreach (var instance in existingInstances)
                            instance.SubZoneOrdinal++;

                    AddInstancesToSubZone(subZone, instanceIds, outerOrdinal, isOrderedGroup);
                }

            result = null;
            return false;

            void AddInstancesToSubZone(GameStateSubZone subZone, IEnumerable<int> instanceIds, int outerOrdinal, bool isOrderedGroup)
            {
                var innerOrdinal = 0;
                foreach (var instanceId in instanceIds)
                {
                    if (!GameState.ObjectInstances.TryGetValue(instanceId, out var instance))
                        throw new Exception("Failed to find instance.");
                    if (instance.IsSubZoneSpecified)
                        throw new Exception("Instance is already assigned to a sub zone.");

                    instance.SubZone = subZone;
                    instance.SubZoneOrdinal = outerOrdinal;
                    if (isOrderedGroup)
                        instance.SubZoneInternalOrdinal = innerOrdinal++;
                }
            }
        }

        public void InvalidateQueuedGameStateUpdateEvents()
            => QueuedGameStateUpdateLogEvents = null;

        private const string AnnotationTypeObjectIdChanged = "AnnotationType_ObjectIdChanged";
        private const string AnnotationTypeShuffle         = "AnnotationType_Shuffle";
        private const string GroupTypeOrdered              = "GroupType_Ordered";
        private const string MatchScopeGame                = "MatchScope_Game";
        private const string MatchScopeMatch               = "MatchScope_Match";
        private const string GameObjectTypeCard            = "GameObjectType_Card";
        private const string MessageTypeGameStateMessage   = "GREMessageType_GameStateMessage";
        private const string ResultTypeDraw                = "ResultType_Draw";
        private const string ResultTypeWinLoss             = "ResultType_WinLoss";
        private const string SubZoneTypeBottom             = "SubZoneType_Bottom";
        private const string SubZoneTypeTop                = "SubZoneType_Top";
        private const string ZoneTypeLibrary               = "ZoneType_Library";
    }
}
