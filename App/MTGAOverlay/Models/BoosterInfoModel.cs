using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class BoosterInfoModel : Model
    {
        public Guid Id { get; }
        public int MtgaBoosterId { get; }
        public string SetCode { get; }
        public string SetName { get; }

        public int TotalNumberRares { get; }
        public int TotalNumberMythicRares { get; }


        private BoosterInfoModel(DataContext dataContext, Guid id, int mtgaBoosterId, string setCode, string setName, int totalNumberRares, int totalNumberMythics) 
            : base(dataContext)
        {
            Id                     = id;
            MtgaBoosterId          = mtgaBoosterId;
            SetCode                = setCode;
            SetName                = setName;
            TotalNumberRares       = totalNumberRares;
            TotalNumberMythicRares = totalNumberMythics;
        }

        public static async Task<BoosterInfoModel> LoadByIdAsync(DataContext dataContext, Guid id)
        {
            var boosterRecord = await dataContext.Mtga.Boosters
                .QueryWhere(record => record.SetId == id)
                .SingleAsync()
                .ConfigureAwait(false);
            return await LoadAsync(dataContext, boosterRecord).ConfigureAwait(false);
        }

        public static async Task<BoosterInfoModel> LoadByMtgaBoosterIdAsync(DataContext dataContext, int mtgaBoosterId)
        {
            var boosterRecord = await dataContext.Mtga.Boosters
                .QueryWhere(record => record.MtgaBoosterId == mtgaBoosterId)
                .SingleAsync()
                .ConfigureAwait(false);
            return await LoadAsync(dataContext, boosterRecord).ConfigureAwait(false);
        }

        private static async Task<BoosterInfoModel> LoadAsync(DataContext dataContext, BoosterRecord boosterRecord)
        {
            var setRecord = await dataContext.Cards.Sets
                .QueryWhere(record => record.Id == boosterRecord.SetId)
                .SingleAsync()
                .ConfigureAwait(false);
            var subsetTypeRecord = await dataContext.Cards.SubsetTypes
                .QueryWhere(record => record.Value == "Main Set")
                .SingleAsync()
                .ConfigureAwait(false);
            var rareRarityTypeRecord = await dataContext.Cards.RarityTypes
                .QueryWhere(record => record.Value == "Rare")
                .SingleAsync()
                .ConfigureAwait(false);
            var mythicRareRarityTypeRecord = await dataContext.Cards.RarityTypes
                .QueryWhere(record => record.Value == "Mythic Rare")
                .SingleAsync()
                .ConfigureAwait(false);

            var totalNumberRares       = await GetTotalNumberOfCardsInRarityAsync(rareRarityTypeRecord)      .ConfigureAwait(false);
            var totalNumberMythicRares = await GetTotalNumberOfCardsInRarityAsync(mythicRareRarityTypeRecord).ConfigureAwait(false);

            return new BoosterInfoModel(dataContext, boosterRecord.SetId, boosterRecord.MtgaBoosterId, setRecord.Code, setRecord.Name, totalNumberRares, totalNumberMythicRares);

            async Task<int> GetTotalNumberOfCardsInRarityAsync(RarityTypeRecord rarityTypeRecord)
                => (await dataContext.Cards.SetInclusions
                    .QueryWhere(record =>
                        record.SetRecord == setRecord &&
                        record.SubsetTypeRecord == subsetTypeRecord &&
                        record.RarityTypeRecord == rarityTypeRecord)
                    .ToListAsync()
                    .ConfigureAwait(false))
                    .Count * 4;
        }
    }
}
