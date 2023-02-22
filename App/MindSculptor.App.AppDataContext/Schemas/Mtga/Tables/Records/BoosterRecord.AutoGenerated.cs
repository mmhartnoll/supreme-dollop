using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class BoosterRecord : DatabaseRecord<BoosterRecord>
    {
        public Guid SetId { get; }
        public int MtgaBoosterId { get; }

        private BoosterRecord(DatabaseContext databaseContext, BoostersTable boostersTable, Guid setId, int mtgaBoosterId) : base(databaseContext, boostersTable)
        {
            SetId = setId;
            MtgaBoosterId = mtgaBoosterId;
        }

        internal static BoosterRecord Create(DatabaseContext databaseContext, BoostersTable boostersTable, Guid setId, int mtgaBoosterId)
        {
            return new BoosterRecord(databaseContext, boostersTable, setId, mtgaBoosterId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[Boosters] SET MtgaBoosterId = @MtgaBoosterId WHERE SetId = @SetId;";
                command.AddParameter("SetId", SetId);
                command.AddParameter("MtgaBoosterId", MtgaBoosterId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[Boosters] SET MtgaBoosterId = @MtgaBoosterId WHERE SetId = @SetId;";
                command.AddParameter("SetId", SetId);
                command.AddParameter("MtgaBoosterId", MtgaBoosterId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[Boosters] WHERE SetId = @SetId;";
            command.AddParameter("SetId", SetId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[Boosters] WHERE SetId = @SetId;";
            command.AddParameter("SetId", SetId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}