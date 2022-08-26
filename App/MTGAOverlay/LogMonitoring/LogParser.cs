using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents;
using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring
{
    internal class LogParser
    {
        public string LogDirectory { get; }
        public string LogFileName { get; }

        public event AsyncEventHandler PollingAsync = delegate { return Task.CompletedTask; };

        public LogParser(string logDirectory, string logFileName)
        {
            LogDirectory = logDirectory;
            LogFileName = logFileName;
        }

        public async IAsyncEnumerable<LogEvent> StreamLogEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var logMessage in StreamLogMessagesAsync())
                if (LogEvent.TryParse(logMessage, out var logEvent))
                    yield return logEvent.Value;

            async IAsyncEnumerable<string> StreamLogMessagesAsync()
            {
                await using (var enumerator = StreamRawDataAsync().GetAsyncEnumerator())
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        while (await enumerator.MoveNextAsync().ConfigureAwait(false) && !enumerator.Current.StartsWith(@"["))
                            continue;
                        var partialMessage = new StringBuilder();
                        var openingBraceCount = 0;
                        var closingBraceCount = 0;
                        do
                        {
                            var regexMatch = Regex.Match(enumerator.Current, @"^\[[^\[]+\].*$");
                            if (regexMatch.Success && partialMessage.Length > 0)
                            {
                                yield return partialMessage.ToString();
                                partialMessage.Clear();
                            }
                            if (partialMessage.Length > 0)
                                partialMessage.AppendLine();
                            partialMessage.Append(enumerator.Current);

                            openingBraceCount += enumerator.Current.Count(c => c == '{');
                            closingBraceCount += enumerator.Current.Count(c => c == '}');

                            if (openingBraceCount == closingBraceCount && openingBraceCount > 0)
                            {
                                yield return partialMessage.ToString();
                                break;
                            }
                        }
                        while (await enumerator.MoveNextAsync().ConfigureAwait(false));
                    }

                async IAsyncEnumerable<string> StreamRawDataAsync()
                {
                    var filePath = Path.Combine(LogDirectory, LogFileName);
                    if (!File.Exists(filePath))
                        throw new FileNotFoundException($"{nameof(LogParser)}: Failed to find log file located at '{filePath}'.");
                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    using var streamReader = new StreamReader(fileStream);
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var nextLine = await streamReader.ReadLineAsync().ConfigureAwait(false);
                            if (nextLine != null)
                                yield return nextLine;
                        }
                        await PollingAsync.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
                        await Task.Delay(PollIntervalInMilliseconds).ConfigureAwait(false);
                    }
                }
            }
        }

        private const int PollIntervalInMilliseconds = 500;
    }
}
