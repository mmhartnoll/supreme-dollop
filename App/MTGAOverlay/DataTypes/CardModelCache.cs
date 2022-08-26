using MindSculptor.App.MtgaOverlay.Models;
using MindSculptor.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class CardModelCache
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        private readonly DataContext                        dataContext;
        private readonly AsyncCachedLookup<Guid, CardModel> idLookup;
        private readonly AsyncCachedLookup<int, CardModel>  mtgaCardIdLookup;

        public CardModelCache(DataContext dataContext)
        {
            this.dataContext = dataContext;
            idLookup         = new AsyncCachedLookup<Guid, CardModel> (LoadCardById);
            mtgaCardIdLookup = new AsyncCachedLookup<int, CardModel>  (LoadCardByMtgaCardId);
        }

        public Task<CardModel> GetCardById(Guid id)
            => idLookup.GetValueAsync(id);

        public Task<CardModel> GetCardByMtgaCardId(int mtgaCardId)
            => mtgaCardIdLookup.GetValueAsync(mtgaCardId);

        private async Task<CardModel> LoadCardById(Guid id)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            var cardModel = await CardModel.LoadAsync(dataContext, id).ConfigureAwait(false);
            await mtgaCardIdLookup.AddValueAsync(cardModel.MtgaCardId, cardModel).ConfigureAwait(false);
            semaphore.Release();
            return cardModel;
        }

        private async Task<CardModel> LoadCardByMtgaCardId(int mtgaCardId)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            var cardModel = await CardModel.LoadAsync(dataContext, mtgaCardId).ConfigureAwait(false);
            await idLookup.AddValueAsync(cardModel.Id, cardModel).ConfigureAwait(false);
            semaphore.Release();
            return cardModel;
        }
    }
}
