using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ViewChangeLogEvents;
using MindSculptor.Tools;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents
{
    internal abstract class LogEvent
    {
        private static readonly ProfileActiveLogEventFactory profileActiveLogEventFactory = new ProfileActiveLogEventFactory();
        private static readonly MatchCreationLogEventFactory matchCreationLogEventFactory = new MatchCreationLogEventFactory();
        private static readonly GameStateUpdateLogEventFactory gameStateUpdateLogEventFactory = new GameStateUpdateLogEventFactory();

        public static bool TryParse(string logMessage, out NullableReference<LogEvent> result)
        {
            if (logMessage.StartsWith("[Accounts - Client]"))
                return profileActiveLogEventFactory.TryParseAccountLogInMessage(logMessage, out result);
            if (logMessage.StartsWith("[Accounts - AccountClient]"))
                return profileActiveLogEventFactory.TryParseAccountUpdateMessage(logMessage, out result);

            var regexMatch = Regex.Match(logMessage, @"^\[UnityCrossThreadLogger\]([^\{]*)(\{[\s\S]*\})$");
            if (regexMatch.Success)
            {
                var preamble = regexMatch.Groups[1].Value.Trim();
                var jsonString = regexMatch.Groups[2].Value;

                var jsonElement = JsonDocument.Parse(jsonString).RootElement;

                return TryParseFromBasicPreamble(preamble, jsonElement, out result);
            }
            result = null;
            return false;
        }

        private static bool TryParseFromBasicPreamble(string preamble, JsonElement jsonElement, out NullableReference<LogEvent> result)
            => preamble switch
            {
                "Client.SceneChange"                     => ViewChangeLogEvent               .TryParse(jsonElement, out result),

                //"<== Draft.MakePick"                     => DraftPickOptionsLogEvent         .TryParse(jsonElement, out result),
                //"<== Draft.DraftStatus"                  => DraftPickOptionsLogEvent         .TryParse(jsonElement, out result),
                "<== Event.DeckSubmitV3"                 => matchCreationLogEventFactory     .TryParseDeckInfo(jsonElement, out result),
                "<== Event.GetPlayerCourseV2"            => matchCreationLogEventFactory     .TryParseDeckInfo(jsonElement, out result),
                "<== Event.GetPlayerCoursesV2"           => EventInfoLogEvent                .TryParse(jsonElement, out result),
                "<== Event.Join"                         => EventJoinLogEvent                .TryParse(jsonElement, out result),
                "<== Inventory.Updated"                  => ProfileInventoryUpdateLogEvent   .TryParse(jsonElement, out result),
                "<== PlayerInventory.GetPlayerCardsV3"   => ProfileInventoryCardsInfoLogEvent.TryParse(jsonElement, out result),
                "<== PlayerInventory.GetPlayerInventory" => ProfileInventoryInfoLogEvent     .TryParse(jsonElement, out result),

                //"==> Draft.MakePick"                     => DraftPickSelectionLogEvent       .TryParse(jsonElement, out result),
                //"==> Event.JoinQueue"                    => ViewChangeLogEvent               .TryParse(jsonElement, out result),

                //"==> Log.BI"                             => TryParseFromLogBIElement(jsonElement, out result),
                _                                        => TryParseFromDynamicPreamble(preamble, jsonElement, out result)
            };

        //private static bool TryParseFromLogBIElement(JsonElement jsonElement, out NullableReference<LogEvent> result)
        //{
        //    if (jsonElement.TryGetProperty("request", out var requestElement))
        //    {
        //        var requestDocument = JsonDocument.Parse(requestElement.GetString());
        //        var paramsElement = requestDocument.RootElement.GetProperty("params");
        //        var messageName = paramsElement.GetProperty("messageName").GetString();

        //        return messageName switch
        //        {
        //            "Client.Connected"   => ProfileActiveLogEvent  .TryParse(paramsElement, out result),
        //            "Client.SceneChange" => ViewChangeLogEvent     .TryParse(paramsElement, out result),
        //            "DuelScene.GameStop" => MatchGameResultLogEvent.TryParse(paramsElement, out result),

        //            _                    => DefaultTryParse(jsonElement, out result)
        //        };
        //    }
        //    result = null;
        //    return false;
        //}

        private static bool TryParseFromDynamicPreamble(string preamble, JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var regexMatch = Regex.Match(preamble, @"^1?\d\/[1-3]?\d\/\d{4} 1?\d:[0-5]\d:[0-5]\d [AP]M: [^:]*: (\w*?)$");
            if (regexMatch.Success)
            {
                var messageType = regexMatch.Groups[1].Value;
                return messageType switch
                {
                    "GreToClientEvent"                                   => gameStateUpdateLogEventFactory.TryParseAggregateGameStateMessage(jsonElement, out result),

                    "ClientToMatchServiceMessageType_ClientToGREMessage" => TryParseFromClientToGREMessage(jsonElement, out result),
                    "MatchGameRoomStateChangedEvent"                     => TryParseFromMatchGameRoomStateChangedElement(jsonElement, out result),
                    _                                                    => DefaultTryParse(out result)
                };
            }

            result = null;
            return false;
        }

        private static bool TryParseFromClientToGREMessage(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            //if (jsonElement.TryGetProperty("payload", out var payloadElement) && payloadElement.TryGetProperty("type", out var typeElement) && typeElement.GetString() == "ClientMessageType_CancelActionReq")
            //{
            //    var gameStateId = payloadElement.GetProperty("gameStateId").GetInt32();
            //    return gameStateUpdateLogEventFactory.TryRevertToPreviousGameState(gameStateId, out result);
            //}
            //else
            return MatchDeckConfigurationUpdateLogEvent.TryParse(jsonElement, out result);
        }

        private static bool TryParseFromMatchGameRoomStateChangedElement(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var gameRoomStateType = jsonElement.GetProperty("matchGameRoomStateChangedEvent")
                .GetProperty("gameRoomInfo")
                .GetProperty("stateType")
                .GetString();
            return gameRoomStateType switch
            {
                "MatchGameRoomStateType_Playing"        => matchCreationLogEventFactory.TryParsePlayerInfo(jsonElement, out result),
                //"MatchGameRoomStateType_MatchCompleted" => MatchResultLogEvent         .TryParse(jsonElement, out result),

                _                                       => DefaultTryParse(out result)
            };
        }

        private static bool DefaultTryParse(out NullableReference<LogEvent> result)
        {
            result = null;
            return false;
        }
    }
}
