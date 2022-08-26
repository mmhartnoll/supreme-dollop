using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class DraftEventModel : EventModel
    {
        public DraftEventType DraftEventType { get; }
        public string SetCode { get; }

        private DraftEventModel(DataContext dataContext, ProfileRecord profileRecord, EventRecord eventRecord, DraftEventType draftEventType, string setCode) 
            : base(dataContext, profileRecord, eventRecord)
        {
            DraftEventType = draftEventType;
            SetCode = setCode;
        }

        public static async Task<DraftEventModel> CreateAsync(DataContext dataContext, ProfileRecord profileRecord, EventRecord eventRecord, DraftEventType draftEventType, string setCode)
        {
            var setRecord = await dataContext.Cards.Sets
                .QueryWhere(record => 
                    record.Code == setCode &&
                    record.CodeExtension == null)
                .SingleAsync()
                .ConfigureAwait(false);
            var draftEventRecord = await dataContext.Mtga.DraftEvents.NewRecordAsync(eventRecord, setRecord, draftEventType.ToString()).ConfigureAwait(false);
            return new DraftEventModel(dataContext, profileRecord, eventRecord, draftEventType, setRecord.Code);
        }

        public static async Task<NullableReference<DraftEventModel>> LoadOrDefaultAsync(DataContext dataContext, ProfileRecord profileRecord, EventRecord eventRecord)
        {
            var draftEventResult = await dataContext.Mtga.DraftEvents
                .QueryWhere(record => record.EventRecord == eventRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (draftEventResult.Success)
            {
                var setRecord = await dataContext.Cards.Sets
                    .QueryWhere(record => record.Id == draftEventResult.Value.SetId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var draftEventType = (DraftEventType)Enum.Parse(typeof(DraftEventType), draftEventResult.Value.DraftType);
                return new DraftEventModel(dataContext, profileRecord, eventRecord, draftEventType, setRecord.Code);
            }
            return null;
        }
    }
}
