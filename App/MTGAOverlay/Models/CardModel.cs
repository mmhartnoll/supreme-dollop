using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.DataAccess.Context.Query;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class CardModel : Model
    {
        private readonly DigitalCardRecord                   digitalCardRecord;

        private readonly Lazy<Task<IEnumerable<FaceRecord>>> faceRecordsLoader;
        private readonly Lazy<Task<RarityTypeRecord>>        rarityTypeRecordLoader;
        private readonly Lazy<Task<SetInclusionRecord>>      setInclusionRecordLoader;
        private readonly Lazy<Task<SetRecord>>               setRecordLoader;
        private readonly Lazy<Task<SubsetTypeRecord>>        subsetTypeRecordLoader;
        private readonly Lazy<Task<string>>                  nameLoader;
        private readonly Lazy<Task<ColorIdentity>>           colorIdentityLoader;

        private Task<IEnumerable<FaceRecord>> FaceRecords        => faceRecordsLoader.Value;
        private Task<RarityTypeRecord>        RarityTypeRecord   => rarityTypeRecordLoader.Value;
        private Task<SetInclusionRecord>      SetInclusionRecord => setInclusionRecordLoader.Value;
        private Task<SetRecord>               SetRecord          => setRecordLoader.Value;
        private Task<SubsetTypeRecord>        SubsetTypeRecord   => subsetTypeRecordLoader.Value;

        public Guid Id         => digitalCardRecord.Id;
        public int  MtgaCardId => digitalCardRecord.MtgaCardId;

        private CardModel(DataContext dataContext, DigitalCardRecord digitalCardRecord) 
            : base(dataContext)
        {
            this.digitalCardRecord   = digitalCardRecord;

            faceRecordsLoader        = new Lazy<Task<IEnumerable<FaceRecord>>> (LoadFaceRecordsAsync);
            rarityTypeRecordLoader   = new Lazy<Task<RarityTypeRecord>>        (LoadRarityTypeRecordAsync);
            setInclusionRecordLoader = new Lazy<Task<SetInclusionRecord>>      (LoadSetInclusionRecordAsync);
            setRecordLoader          = new Lazy<Task<SetRecord>>               (LoadSetRecordAsync);
            subsetTypeRecordLoader   = new Lazy<Task<SubsetTypeRecord>>        (LoadSubsetTypeRecordAsync);
            nameLoader               = new Lazy<Task<string>>                  (LoadNameAsync);
            colorIdentityLoader      = new Lazy<Task<ColorIdentity>>           (LoadColorIdentityAsync);
        }

        public static async Task<CardModel> LoadAsync(DataContext dataContext, Guid id)
        {
            var digitalCardRecord = await dataContext.Mtga.DigitalCards
                .QueryWhere(record => record.Id == id)
                .SingleAsync()
                .ConfigureAwait(false);
            return new CardModel(dataContext, digitalCardRecord);
        }

        public static async Task<CardModel> LoadAsync(DataContext dataContext, int mtgaCardId)
        {
            var digitalCardRecord = await dataContext.Mtga.DigitalCards
                .QueryWhere(record => record.MtgaCardId == mtgaCardId)
                .SingleAsync()
                .ConfigureAwait(false);
            return new CardModel(dataContext, digitalCardRecord);
        }

        private async Task<IEnumerable<FaceRecord>> LoadFaceRecordsAsync()
        {
            var setInclusionRecord = await SetInclusionRecord.ConfigureAwait(false);
            var baseRecord = await DataContext.Cards.Bases
                .QueryWhere(record => record.Id == setInclusionRecord.BaseId)
                .SingleAsync()
                .ConfigureAwait(false);
            return await DataContext.Cards.Faces
                .QueryWhere(record => record.BaseRecord == baseRecord)
                .OrderBy(record => record.IsPrimaryFace, SortDirection.Descending)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        private async Task<RarityTypeRecord> LoadRarityTypeRecordAsync()
        {
            var setInclusionRecord = await SetInclusionRecord.ConfigureAwait(false);
            return await DataContext.Cards.RarityTypes
                .QueryWhere(record => record.Id == setInclusionRecord.RarityTypeId)
                .SingleAsync()
                .ConfigureAwait(false);
        }

        private async Task<SetInclusionRecord> LoadSetInclusionRecordAsync()
        {
            var basePrintingRecord = await DataContext.Cards.BasePrintings
                .QueryWhere(record => record.Id == digitalCardRecord.Id)
                .SingleAsync()
                .ConfigureAwait(false);
            return await DataContext.Cards.SetInclusions
                .QueryWhere(record => record.Id == basePrintingRecord.SetInclusionId)
                .SingleAsync()
                .ConfigureAwait(false);
        }

        private async Task<SetRecord> LoadSetRecordAsync()
        {
            var setInclusionRecord = await SetInclusionRecord.ConfigureAwait(false);
            return await DataContext.Cards.Sets
                .QueryWhere(record => record.Id == setInclusionRecord.SetId)
                .SingleAsync()
                .ConfigureAwait(false);
        }

        private async Task<SubsetTypeRecord> LoadSubsetTypeRecordAsync()
        {
            var setInclusionRecord = await SetInclusionRecord.ConfigureAwait(false);
            return await DataContext.Cards.SubsetTypes
                .QueryWhere(record => record.Id == setInclusionRecord.SubsetTypeId)
                .SingleAsync()
                .ConfigureAwait(false);
        }

        public async Task<string> LoadNameAsync()
        {
            var faces = await GetFacesAsync().ConfigureAwait(false);
            return string.Join(" // ", faces.Select(face => face.Name));
        }

        private async Task<ColorIdentity> LoadColorIdentityAsync()
        {
            var faces = await GetFacesAsync().ConfigureAwait(false);
            var colorIdentities = await faces.SelectAsync(face => face.GetColorIdentityAsync())
                .ToListAsync()
                .ConfigureAwait(false);
            return colorIdentities.Aggregate(ColorIdentity.Colorless, (accum, identity) => accum |= identity);
        }

        public async Task<IEnumerable<CardFaceModel>> GetFacesAsync()
        {
            var faceRecords = await FaceRecords.ConfigureAwait(false);
            return faceRecords.Select(record => new CardFaceModel(DataContext, record));
        }

        public async Task<string> GetSetCodeAsync()
        {
            var setRecord = await SetRecord.ConfigureAwait(false);
            return setRecord.Code;
        }

        public async Task<Rarity> GetRarityAsync()
        {
            var rarityTypeRecord = await RarityTypeRecord.ConfigureAwait(false);
            return (Rarity)Enum.Parse(typeof(Rarity), rarityTypeRecord.Value.Replace(" ", string.Empty));
        }

        public async Task<int> GetCollectorsNumberAsync()
        {
            var setInclusionRecord = await SetInclusionRecord.ConfigureAwait(false);
            return setInclusionRecord.CollectorsNumber!.Value;
        }

        public async Task<int> GetLogicalOrdinalAsync()
        {
            var setInclusionRecord = await SetInclusionRecord.ConfigureAwait(false);
            return setInclusionRecord.LogicalOrdinal;
        }

        public async Task<bool> GetIsAvailableInBoostersAsync()
        {
            var subsetTypeRecord = await SubsetTypeRecord.ConfigureAwait(false);
            return subsetTypeRecord.Value == "Main Set";
        }

        public Task<string> GetName()
            => nameLoader.Value;

        public Task<ColorIdentity> GetColorIdentity()
            => colorIdentityLoader.Value;
    }
}
