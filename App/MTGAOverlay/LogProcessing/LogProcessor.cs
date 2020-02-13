using MindSculptor.App.MtgaOverlay.GUI;
using MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents;
using MindSculptor.App.MtgaOverlay.LogProcessing.Modules;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.LogProcessing
{
    internal class LogProcessor
    {
        // Should implement IDisopsable if we will keep filestream open constantly, this allows us to stop the loop and clean up streamreader


        private readonly string logFilePath;

        public LogProcessor(string logFilePath)
            => this.logFilePath = logFilePath;

        public async IAsyncEnumerable<GUIUpdateEvent> ProcessLogs()
        {
            await foreach (var logEvent in StreamLogEvents().ConfigureAwait(false))
            {
                switch (logEvent)
                {
                    case JoinDraftEventLogEvent joinDraftEventLogEvent:

                        yield return new LogUpdateEvent($"Creating/Refreshing draft entry info '{joinDraftEventLogEvent.DraftEventEntryId}'.");
                        await DraftEventProcessor.JoinDraftEventAsync(joinDraftEventLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case UpdateDraftEventLogEvent updateDraftEventLogEvent:
                        yield return new LogUpdateEvent("Refreshing draft event info.");
                        await DraftEventProcessor.RefreshDraftEventAsync(updateDraftEventLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case DraftPickOptionsLogEvent draftPickOptionsLogEvent:
                        if (!draftPickOptionsLogEvent.DraftPackCardIds.Any())
                        {
                            yield return new GUIUpdateEvent(mainWindow => mainWindow.ClearScene());
                            break;
                        }
                        var draftPickOptions = await DraftPickProcessor.StreamDraftPickOptionsAndSaveState(draftPickOptionsLogEvent)
                            .ToListAsync()
                            .ConfigureAwait(false);
                        if (draftPickOptions.Any())
                        {
                            yield return new LogUpdateEvent($"Displaying draft picks for pack {draftPickOptionsLogEvent.PackNumber}, pick {draftPickOptionsLogEvent.PickNumber}.");
                            yield return new GUIUpdateEvent(mainWindow => mainWindow.DisplayDraftPickOptions(draftPickOptions));
                        }
                        else
                        {
                            yield return new LogUpdateEvent($"Draft picks for pack {draftPickOptionsLogEvent.PackNumber}, pick {draftPickOptionsLogEvent.PickNumber} have been made previously. Display not required.");
                            yield return new GUIUpdateEvent(mainWindow => mainWindow.ClearScene());
                        }
                        break;

                    case DraftPickSelectionLogEvent draftPickSelectionLogEvent:
                        await DraftPickProcessor.SaveDraftPickSelection(draftPickSelectionLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case UpdateProfileCardsLogEvent updateProfileCardsLogEvent:
                        yield return new LogUpdateEvent("Refreshing profile cards");
                        await ProfileInventoryProcessor.RefreshProfileCardsAsync(updateProfileCardsLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case UpdateProfileInventoryLogEvent updateProfileInventoryLogEvent:
                        yield return new LogUpdateEvent("Refreshing profile inventory");
                        await ProfileInventoryProcessor.RefreshProfileInventoryAsync(updateProfileInventoryLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case SceneChangeLogEvent sceneChangeLogEvent:
                        if (sceneChangeLogEvent.ToSceneName == "Home")
                        {
                            yield return new LogUpdateEvent("Returning to home screen. All other overlays are cleared.");
                            yield return new GUIUpdateEvent(mainWindow => mainWindow.ClearScene());
                        }
                        break;

                    case CreateDraftMatchLogEvent createDraftMatchLogEvent:
                        yield return new LogUpdateEvent($"Creating/Refreshing draft match info '{createDraftMatchLogEvent.MatchId}'.");
                        await DraftEventProcessor.StartDraftEventMatchAsync(createDraftMatchLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case DraftGameEndLogEvent draftGameEndLogEvent:
                        yield return new LogUpdateEvent($"Creating/Refreshing draft game info. Game {draftGameEndLogEvent.GameNumber}, match '{draftGameEndLogEvent.MatchId}'.");
                        await DraftEventProcessor.RecordDraftGameAsync(draftGameEndLogEvent)
                            .ConfigureAwait(false);
                        break;

                    case DraftMatchEndLogEvent draftMatchEndLogEvent:
                        yield return new LogUpdateEvent($"Creating/Refreshing draft match result info '{draftMatchEndLogEvent.MatchId}'.");
                        await DraftEventProcessor.RecordDraftMatchResults(draftMatchEndLogEvent)
                            .ConfigureAwait(false);
                        break;

                    default:
                        continue;
                }
            }
        }

        private async IAsyncEnumerable<LogEvent> StreamLogEvents()
        {
            await foreach ((var preamble, var json) in StreamLogMessages().ConfigureAwait(false))
            {
                if (preamble.StartsWith("<=="))
                {
                    var methodName = preamble.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
                    switch (methodName)
                    {
                        case "Event.Draft":
                            yield return JoinDraftEventLogEvent.FromJson(json);
                            continue;

                        case "Event.GetPlayerCoursesV2":
                            foreach (var logEvent in UpdateDraftEventLogEvent.FromJson(json))
                                yield return logEvent;
                            continue;

                        case "PlayerInventory.GetPlayerCardsV3":
                            yield return UpdateProfileCardsLogEvent.FromJson(json);
                            continue;

                        case "PlayerInventory.GetPlayerInventory":
                            yield return UpdateProfileInventoryLogEvent.FromJson(json);
                            continue;

                        case "Draft.DraftStatus":
                        case "Draft.MakePick":
                            yield return DraftPickOptionsLogEvent.FromJsonDocument(json);
                            continue;

                        default:
                            continue;
                    }
                }
                else if (preamble.StartsWith("==>"))
                {
                    var methodName = preamble.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
                    switch (methodName)
                    {
                        case "Draft.MakePick":
                            yield return DraftPickSelectionLogEvent.FromJson(json);
                            continue;

                        case "Event.MatchCreated":
                            yield return CreateDraftMatchLogEvent.FromJson(json);
                            continue;

                        case "Log.BI":
                            switch (GetLogBIMessageNameFromJson(json))
                            {
                                case "Client.SceneChange":
                                    yield return SceneChangeLogEvent.FromJson(json);
                                    continue;

                                case "DuelScene.GameStop":
                                    yield return DraftGameEndLogEvent.FromJson(json);
                                    continue;

                                default:
                                    continue;
                            }

                        default:
                            continue;
                    }
                }
                else
                {
                    var regexMatch = Regex.Match(preamble, @"^(\d{1,2}\/\d{1,2}\/\d{4} \d{1,2}:\d{2}:\d{2} [AP]M): Match to (.*?): (\w*?)$");
                    if (regexMatch.Success)
                        switch (regexMatch.Groups[3].Value)
                        {
                            case "MatchGameRoomStateChangedEvent":
                                switch (GetMatchGameRoomStateChangedEventType(json))
                                {
                                    case "MatchGameRoomStateType_MatchCompleted":
                                        yield return DraftMatchEndLogEvent.FromJson(json);
                                        continue;

                                    default:
                                        continue;
                                }

                            default:
                                continue;
                        }
                }

                string GetLogBIMessageNameFromJson(JsonElement jsonElement)
                {
                    var requestElement = jsonElement.GetProperty("request");
                    var requestDocument = JsonDocument.Parse(requestElement.GetString());

                    return requestDocument.RootElement
                        .GetProperty("params")
                        .GetProperty("messageName")
                        .GetString();
                }

                string GetMatchGameRoomStateChangedEventType(JsonElement jsonElement)
                    => jsonElement.GetProperty("matchGameRoomStateChangedEvent")
                        .GetProperty("gameRoomInfo")
                        .GetProperty("stateType")
                        .GetString();
            }

            async IAsyncEnumerable<(string Preamble, JsonElement Json)> StreamLogMessages()
            {
                await using (var enumerator = StreamRawData().GetAsyncEnumerator())
                    while (true)
                    {
                        while (await enumerator.MoveNextAsync().ConfigureAwait(false) && !enumerator.Current.StartsWith(@"[UnityCrossThreadLogger]"))
                            continue;
                        var stringBuilder = new StringBuilder();
                        do
                        {
                            if (enumerator.Current.StartsWith(@"[UnityCrossThreadLogger]"))
                                stringBuilder = new StringBuilder(enumerator.Current);
                            else
                                stringBuilder.Append(enumerator.Current);

                            var regexMatch = Regex.Match(stringBuilder.ToString(), @"^\[UnityCrossThreadLogger\](.*?)(\{.*\})$");
                            if (regexMatch.Success)
                            {
                                var preamble = regexMatch.Groups[1].Value.Trim();
                                var jsonString = regexMatch.Groups[2].Value;

                                var openingBracesCount = jsonString.Count(x => x == '{');
                                var closingBracesCount = jsonString.Count(x => x == '}');
                                if (openingBracesCount == closingBracesCount)
                                {
                                    var jsonElement = JsonDocument.Parse(jsonString).RootElement;
                                    if (jsonElement.TryGetProperty("id", out var idProperty) && idProperty.GetInt32() == -1)
                                        break;

                                    yield return (preamble, jsonElement);
                                    break;
                                }
                            }
                        }
                        while (await enumerator.MoveNextAsync().ConfigureAwait(false));
                    }
            }

            async IAsyncEnumerable<string> StreamRawData()
            {
                if (!File.Exists(logFilePath))
                    throw new FileNotFoundException("Failed to find log file for MTGA");
                using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var streamReader = new StreamReader(fileStream))
                    while (true)
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var nextLine = await streamReader.ReadLineAsync()
                                .ConfigureAwait(false);
                            if (nextLine != null)
                                yield return nextLine;
                        }
                        var currentPosition = fileStream.Position;
                        await Task.Delay(500)
                            .ConfigureAwait(false);
                        if (fileStream.Length < currentPosition)
                            fileStream.Seek(0, SeekOrigin.Begin);
                    }
            }
        }
    }
}
