using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class CardInventoryModel : Model
    {
        private readonly ProfileHasCardRecord profileHasCardRecord;

        private int count;

        public CardModel CardModel { get; }
        public int Count
        {
            get => count;
            private set => SetProperty(ref count, value, nameof(Count));
        }

        private CardInventoryModel(DataContext dataContext, ProfileHasCardRecord profileHasCardRecord, CardModel cardModel) 
            : base(dataContext)
        {
            this.profileHasCardRecord = profileHasCardRecord;
            CardModel = cardModel;
        }

        public static async Task<CardInventoryModel> CreateNewAsync(DataContext dataContext, CardModelCache cardModelCache, ProfileRecord profileRecord, int mtgaCardId, int count)
        {
            var cardModel = await cardModelCache.GetCardByMtgaCardId(mtgaCardId).ConfigureAwait(false);
            var profileHasCardRecord = await dataContext.Mtga.ProfileHasCards
                .NewRecordAsync(profileRecord.Id, cardModel.Id, count)
                .ConfigureAwait(false);
            return new CardInventoryModel(dataContext, profileHasCardRecord, cardModel);
        }

        public static async Task<IEnumerable<CardInventoryModel>> LoadAllAsync(DataContext dataContext, CardModelCache cardModelCache, ProfileRecord profileRecord)
        {
            var cardInventoryModels = new List<CardInventoryModel>();
            var profileHasCardRecords = await dataContext.Mtga.ProfileHasCards
                .QueryWhere(record => record.ProfileRecord == profileRecord)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var profileHasCardRecord in profileHasCardRecords)
            {
                var cardModel = await cardModelCache.GetCardById(profileHasCardRecord.DigitalCardId).ConfigureAwait(false);
                cardInventoryModels.Add(new CardInventoryModel(dataContext, profileHasCardRecord, cardModel));
            }
            return cardInventoryModels.Enumerate();
        }

        public async Task UpdateCountAsync(int count)
        {
            if (count > 0)
            {
                profileHasCardRecord.Count = count;
                await profileHasCardRecord.UpdateRecordAsync().ConfigureAwait(false);
            }
            else
                await profileHasCardRecord.DeleteRecordAsync().ConfigureAwait(false);
            Count = count;
        }
    }
}
