using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class ProfileModel : Model
    {
        public Guid Id { get; }
        public string MtgaUserId { get; }
        public string Name { get; }
        public int NameId { get; }

        public ProfileInventoryModel InventoryModel { get; }

        private ProfileModel(DataContext dataContext, Guid id, string mtgaUserId, string name, int nameId, ProfileInventoryModel inventoryModel)
            : base(dataContext)
        {
            Id             = id;
            MtgaUserId     = mtgaUserId;
            Name           = name;
            NameId         = nameId;
            InventoryModel = inventoryModel;

            InventoryModel.LogMessageAsync      += OnLogMessageAsync;
            InventoryModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
        }

        public static async Task<ProfileModel> LoadOrCreateAsync(DataContext dataContext, CardModelCache cardModelCache, string mtgaUserId, string name, int nameId, Func<string, Task> onLogMessage)
        {
            var transactionScope = await dataContext.BeginTransactionAsync().ConfigureAwait(false);
            await using (transactionScope.ConfigureAwait(false))
                try
                {
                    var result = await transactionScope.ExecuteAsync(TransactionScope).ConfigureAwait(false);
                    await transactionScope.CommitAsync().ConfigureAwait(false);
                    return result;
                }
                catch
                {
                    await transactionScope.RollbackAsync().ConfigureAwait(false);
                    throw;
                }

            async Task<ProfileModel> TransactionScope()
            {
                PlayerRecord playerRecord;
                var playerResult = await dataContext.Mtga.Players
                    .QueryWhere(record => record.MtgaUserId == mtgaUserId)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (playerResult.Success)
                    playerRecord = playerResult.Value;
                else
                    playerRecord = await dataContext.Mtga.Players.NewRecordAsync(mtgaUserId, name, nameId).ConfigureAwait(false);

                ProfileRecord profileRecord;
                var profileResult = await dataContext.Mtga.Profiles
                    .QueryWhere(record => record.PlayerRecord == playerRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (profileResult.Success)
                {
                    profileRecord = profileResult.Value;
                    await onLogMessage($"Profile record loaded for '{name}#{nameId}'.").ConfigureAwait(false);
                }
                else
                {
                    profileRecord = await dataContext.Mtga.Profiles.NewRecordAsync(playerRecord).ConfigureAwait(false);
                    await onLogMessage($"New profile record created for '{name}#{nameId}'.").ConfigureAwait(false);
                }

                var profileInventoryModel = await ProfileInventoryModel.LoadOrCreateAsync(dataContext, cardModelCache, profileRecord, onLogMessage)
                    .ConfigureAwait(false);

                return new ProfileModel(dataContext, profileRecord.Id, mtgaUserId, name, nameId, profileInventoryModel);
            }
        }
    }
}
