using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.Models;
using MindSculptor.App.MtgaOverlay.ViewModels.Commands;
using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class BoosterInventoryViewModel : ViewModel
    {
        private readonly ProfileInventoryModel profileInventoryModel;

        private bool hasBoosterInfo = false;
        private NullableReference<BoosterCollectionInfo> selectedBoosterCollectionInfo = null;
        private NullableReference<ItemCount<BoosterInfoModel>> savedBoosterCount = null;

        public bool HasBoosterInfo
        {
            get => hasBoosterInfo;
            private set => SetProperty(ref hasBoosterInfo, value, nameof(HasBoosterInfo));
        }
        public BoosterCollectionInfo SelectedBoosterCollectionInfo
        {
            get => selectedBoosterCollectionInfo.Value;
            set => SetProperty(ref selectedBoosterCollectionInfo, value, nameof(SelectedBoosterCollectionInfo));
        }

        public ICommand DisplayNextBoosterCommand { get; }
        public ICommand DisplayPreviousBoosterCommand { get; }

        private BoosterInventoryViewModel(ProfileInventoryModel profileInventoryModel)
        {
            this.profileInventoryModel = profileInventoryModel;
            profileInventoryModel.InventoryUpdatedAsync += OnProfileInventoryUpdated;

            DisplayNextBoosterCommand     = new RelayCommand(DisplayNextBoosterInfo);
            DisplayPreviousBoosterCommand = new RelayCommand(DisplayPrevBoosterInfo);
        }

        public static async Task<BoosterInventoryViewModel> LoadAsync(ProfileInventoryModel profileInventoryModel)
        {
            var viewModel = new BoosterInventoryViewModel(profileInventoryModel);
            var initialSelectedBoosterCount = profileInventoryModel.BoosterCounts.OrderBy(count => count.Item.MtgaBoosterId).FirstOrDefault();
            if (initialSelectedBoosterCount != null)
            {
                await viewModel.UpdateSelectedBoosterInfo(initialSelectedBoosterCount).ConfigureAwait(false);
                viewModel.HasBoosterInfo = true;
            }
            return viewModel;
        }

        private async void DisplayNextBoosterInfo()
        {
            var nextBoosterCount = profileInventoryModel.BoosterCounts
                .Where(count => count.Item.MtgaBoosterId > SelectedBoosterCollectionInfo.MtgaBoosterId)
                .OrderBy(count => count.Item.MtgaBoosterId)
                .FirstOrDefault();
            if (nextBoosterCount == null)
                nextBoosterCount = profileInventoryModel.BoosterCounts.OrderBy(count => count.Item.MtgaBoosterId).FirstOrDefault();
            if (nextBoosterCount != null)
                await UpdateSelectedBoosterInfo(nextBoosterCount);
            else
                HasBoosterInfo = false;
        }

        private async void DisplayPrevBoosterInfo()
        {
            var prevBoosterCount = profileInventoryModel.BoosterCounts
                .Where(count => count.Item.MtgaBoosterId < SelectedBoosterCollectionInfo.MtgaBoosterId)
                .OrderByDescending(count => count.Item.MtgaBoosterId)
                .FirstOrDefault();
            if (prevBoosterCount == null)
                prevBoosterCount = profileInventoryModel.BoosterCounts.OrderByDescending(count => count.Item.MtgaBoosterId).FirstOrDefault();
            if (prevBoosterCount != null)
                await UpdateSelectedBoosterInfo(prevBoosterCount);
            else
                HasBoosterInfo = false;
        }

        private async Task UpdateSelectedBoosterInfo(ItemCount<BoosterInfoModel> boosterCount)
        {
            savedBoosterCount = boosterCount;

            var filteredInventoryModels = await profileInventoryModel.CardInventory
                .WhereAsync(CardInventoryPrimaryFilterAsync)
                .ToListAsync()
                .ConfigureAwait(false);

            // We call blocking property 'Result' below as we have already awaited the results in the primary filter
            
            var collectedRareCount = filteredInventoryModels.Where(cardCount => cardCount.CardModel.GetRarityAsync().Result == Rarity.Rare)
                .Aggregate(0, (accum, cardCount) => accum += cardCount.Count);
            var collectedMythicRareCount = filteredInventoryModels.Where(cardCount => cardCount.CardModel.GetRarityAsync().Result == Rarity.MythicRare)
                .Aggregate(0, (accum, cardCount) => accum += cardCount.Count);

            SelectedBoosterCollectionInfo = new BoosterCollectionInfo(boosterCount.Item.MtgaBoosterId, boosterCount.Item.SetCode, boosterCount.Item.SetName, boosterCount.Item.TotalNumberRares,
                boosterCount.Item.TotalNumberMythicRares, collectedRareCount, collectedMythicRareCount, boosterCount.Count);

            async Task<bool> CardInventoryPrimaryFilterAsync(CardInventoryModel cardInventoryModel)
                => await cardInventoryModel.CardModel.GetSetCodeAsync().ConfigureAwait(false) == boosterCount.Item.SetCode &&
                    (await cardInventoryModel.CardModel.GetRarityAsync().ConfigureAwait(false)).In(Rarity.Rare, Rarity.MythicRare) &&
                    await cardInventoryModel.CardModel.GetIsAvailableInBoostersAsync().ConfigureAwait(false);
        }

        private Task OnProfileInventoryUpdated(NullableReference<object> sender, EventArgs eventArgs)
        {
            if (HasBoosterInfo)
                return UpdateSelectedBoosterInfo(savedBoosterCount.Value);
            return Task.CompletedTask;
        }
    }
}
