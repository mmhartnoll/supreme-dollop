using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.Tools.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class Player
    {
        public Guid Id { get; }
        public string MtgaPlayerId { get; }
        public string ScreenName { get; }
        public int UserId { get; }

        public Player(Guid id, string mtgaPlayerId, string screenName, int userId)
        {
            Id           = id;
            MtgaPlayerId = mtgaPlayerId;
            ScreenName   = screenName;
            UserId       = userId;
        }

        public static async Task<Player> LoadAsync(DataContext dataContext, Guid id)
        {
            var playerRecord = await dataContext.Mtga.Players
                .QueryWhere(record => record.Id == id)
                .SingleAsync()
                .ConfigureAwait(false);
            return new Player(id, playerRecord.MtgaUserId, playerRecord.Name, playerRecord.NameId);
        }

        public static async Task<Player> LoadOrCreateAsync(DataContext dataContext, string mtgaUserId, string name)
        {
            var nameString = name.Split('#', StringSplitOptions.RemoveEmptyEntries).First();
            var nameId = Convert.ToInt32(name.Split('#', StringSplitOptions.RemoveEmptyEntries).Last());

            PlayerRecord playerRecord;
            var playerResult = await dataContext.Mtga.Players
                .QueryWhere(record => record.MtgaUserId == mtgaUserId)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (playerResult.Success)
                playerRecord = playerResult.Value;
            else
                playerRecord = await dataContext.Mtga.Players.NewRecordAsync(mtgaUserId, nameString, nameId).ConfigureAwait(false);
            return new Player(playerRecord.Id, mtgaUserId, nameString, nameId);
        }
    }
}
