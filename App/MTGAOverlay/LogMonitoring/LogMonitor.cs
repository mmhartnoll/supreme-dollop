using MindSculptor.App.MtgaOverlay.LogMonitoring.Events;
using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring
{
    internal class LogMonitor : INotifyPropertyChanged
    {
        private readonly LogEventFactory logEventFactory = new LogEventFactory();

        private bool isLiveMonitoring = false;

        public string LogDirectory { get; }
        public string LogFileName { get; }
        public bool IsLiveMonitoring
        {
            get => isLiveMonitoring;
            set => PropertyChangedHelper.SetProperty(ref isLiveMonitoring, value, nameof(IsLiveMonitoring));
        }

        public LogMonitor(string logDirectory, string logFileName)
        {
            LogDirectory = logDirectory;
            LogFileName = logFileName;

            propertyChangedHelper = new Lazy<PropertyChangedHelper>(() => new PropertyChangedHelper(this, PropertyChanged));
        }

        public async IAsyncEnumerable<LogEvent> StreamLogEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var logMessage in StreamLogMessagesAsync()) 
            {
                logEventFactory.ReadLogMessage(logMessage);
                foreach (var logEvent in logEventFactory.GetQueuedEvents())
                    yield return logEvent;
            }

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
                        if (!IsLiveMonitoring)
                            IsLiveMonitoring = true;
                        await Task.Delay(PollIntervalInMilliseconds).ConfigureAwait(false);
                    }
                }
            }
        }

        private const int PollIntervalInMilliseconds = 500;

        #region INotifyPropertyChanged
        private readonly Lazy<PropertyChangedHelper> propertyChangedHelper;

        private PropertyChangedHelper PropertyChangedHelper => propertyChangedHelper.Value;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion
    }
}
