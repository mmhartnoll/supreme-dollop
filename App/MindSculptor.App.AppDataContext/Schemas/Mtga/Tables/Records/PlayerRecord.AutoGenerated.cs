using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class PlayerRecord : DatabaseRecord<PlayerRecord>
    {
        public Guid Id { get; }
        public string MtgaUserId { get; }
        public string Name { get; }
        public int NameId { get; }

        private PlayerRecord(DatabaseContext dataContext, PlayersTable playersTable, Guid id, string mtgaUserId, string name, int nameId) : base(dataContext, playersTable)
        {
            Id = id;
            MtgaUserId = mtgaUserId;
            Name = name;
            NameId = nameId;
        }

        internal static PlayerRecord Create(DatabaseContext dataContext, PlayersTable playersTable, Guid id, string mtgaUserId, string name, int nameId)
        {
            return new PlayerRecord(dataContext, playersTable, id, mtgaUserId, name, nameId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[Players] SET MtgaUserId = @MtgaUserId, Name = @Name, NameId = @NameId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("MtgaUserId", MtgaUserId);
                command.AddParameter("Name", Name);
                command.AddParameter("NameId", NameId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[Players] SET MtgaUserId = @MtgaUserId, Name = @Name, NameId = @NameId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("MtgaUserId", MtgaUserId);
                command.AddParameter("Name", Name);
                command.AddParameter("NameId", NameId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[Players] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[Players] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}