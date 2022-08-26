using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DigitalCardRecord : DatabaseRecord<DigitalCardRecord>
    {
        public Guid Id { get; }
        public int MtgaCardId { get; }

        private DigitalCardRecord(DatabaseContext dataContext, DigitalCardsTable digitalCardsTable, Guid id, int mtgaCardId) : base(dataContext, digitalCardsTable)
        {
            Id = id;
            MtgaCardId = mtgaCardId;
        }

        internal static DigitalCardRecord Create(DatabaseContext dataContext, DigitalCardsTable digitalCardsTable, Guid id, int mtgaCardId)
        {
            return new DigitalCardRecord(dataContext, digitalCardsTable, id, mtgaCardId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DigitalCards] SET MtgaCardId = @MtgaCardId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("MtgaCardId", MtgaCardId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DigitalCards] SET MtgaCardId = @MtgaCardId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("MtgaCardId", MtgaCardId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[DigitalCards] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[DigitalCards] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}