using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.GUI.BrowserWindow.Views
{
    internal class DraftEventViewOld : DataView
    {
        public Guid Id { get; }
        public string EventType { get; }
        public string SetName { get; }

        private DraftEventViewOld(Guid id, string eventType, string setName)
        {
            Id = id;
            EventType = eventType;
            SetName = setName;
        }

        public static async IAsyncEnumerable<DraftEventViewOld> GetDraftEventData()
        {
            var stream = StreamDataAsync(GetDraftEventData);
            await foreach (var draftEventView in stream.ConfigureAwait(false))
                yield return draftEventView;
        }

        public static async IAsyncEnumerable<DraftEventViewOld> GetDraftEventData(DataContext dataContext)
        {
            var draftEventRecords = await dataContext.Mtga.DraftEvents
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var draftEventRecord in draftEventRecords)
            {
                var setRecord = await dataContext.Cards.Sets
                    .QueryWhere(record => record.Id == draftEventRecord.SetId)
                    .SingleAsync()
                    .ConfigureAwait(false);

                yield return new DraftEventViewOld(draftEventRecord.Id, draftEventRecord.DraftType, setRecord.Name);
            }
        }
    }
}
