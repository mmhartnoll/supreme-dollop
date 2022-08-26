using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class PlayersTable : DatabaseTable<PlayerRecord, PlayerRecordExpression>
    {
        private PlayersTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "Players")
        {
        }

        internal static PlayersTable Create(DatabaseContext dataContext)
        {
            return new PlayersTable(dataContext);
        }

        public PlayerRecord NewRecord(string mtgaUserId, string name, int nameId)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), mtgaUserId, name, nameId));
        }

        public async Task<PlayerRecord> NewRecordAsync(string mtgaUserId, string name, int nameId, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), mtgaUserId, name, nameId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private PlayerRecord NewRecord(DbCommand command, Guid id, string mtgaUserId, string name, int nameId)
        {
            var newRecord = PlayerRecord.Create(Context, this, id, mtgaUserId, name, nameId);
            command.CommandText = "INSERT INTO [Mtga].[Players] ( Id, MtgaUserId, Name, NameId ) VALUES ( @Id, @MtgaUserId, @Name, @NameId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("MtgaUserId", newRecord.MtgaUserId);
            command.AddParameter("Name", newRecord.Name);
            command.AddParameter("NameId", newRecord.NameId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<PlayerRecord> NewRecordAsync(DbCommand command, Guid id, string mtgaUserId, string name, int nameId, CancellationToken cancellationToken)
        {
            var newRecord = PlayerRecord.Create(Context, this, id, mtgaUserId, name, nameId);
            command.CommandText = "INSERT INTO [Mtga].[Players] ( Id, MtgaUserId, Name, NameId ) VALUES ( @Id, @MtgaUserId, @Name, @NameId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("MtgaUserId", newRecord.MtgaUserId);
            command.AddParameter("Name", newRecord.Name);
            command.AddParameter("NameId", newRecord.NameId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override PlayerRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var mtgaUserId = (string)dbDataReader["MtgaUserId"];
            var name = (string)dbDataReader["Name"];
            var nameId = Convert.ToInt32(dbDataReader["NameId"]);
            return PlayerRecord.Create(Context, this, id, mtgaUserId, name, nameId);
        }
    }
}