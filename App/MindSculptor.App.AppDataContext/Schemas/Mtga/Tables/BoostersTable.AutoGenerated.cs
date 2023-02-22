using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
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
    public class BoostersTable : DatabaseTable<BoosterRecord, BoosterRecordExpression>
    {
        private BoostersTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "Boosters")
        {
        }

        internal static BoostersTable Create(DatabaseContext databaseContext)
        {
            return new BoostersTable(databaseContext);
        }

        public BoosterRecord NewRecord(Guid setId, int mtgaBoosterId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, setId, mtgaBoosterId));
        }

        public async Task<BoosterRecord> NewRecordAsync(Guid setId, int mtgaBoosterId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, setId, mtgaBoosterId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public BoosterRecord NewRecord(SetRecord setRecord, int mtgaBoosterId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, setRecord.Id, mtgaBoosterId));
        }

        public async Task<BoosterRecord> NewRecordAsync(SetRecord setRecord, int mtgaBoosterId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, setRecord.Id, mtgaBoosterId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private BoosterRecord NewRecord(DbCommand command, Guid setId, int mtgaBoosterId)
        {
            var newRecord = BoosterRecord.Create(DatabaseContext, this, setId, mtgaBoosterId);
            command.CommandText = "INSERT INTO [Mtga].[Boosters] ( SetId, MtgaBoosterId ) VALUES ( @SetId, @MtgaBoosterId );";
            command.AddParameter("SetId", newRecord.SetId);
            command.AddParameter("MtgaBoosterId", newRecord.MtgaBoosterId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<BoosterRecord> NewRecordAsync(DbCommand command, Guid setId, int mtgaBoosterId, CancellationToken cancellationToken)
        {
            var newRecord = BoosterRecord.Create(DatabaseContext, this, setId, mtgaBoosterId);
            command.CommandText = "INSERT INTO [Mtga].[Boosters] ( SetId, MtgaBoosterId ) VALUES ( @SetId, @MtgaBoosterId );";
            command.AddParameter("SetId", newRecord.SetId);
            command.AddParameter("MtgaBoosterId", newRecord.MtgaBoosterId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override BoosterRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var setId = (Guid)dbDataReader["SetId"];
            var mtgaBoosterId = Convert.ToInt32(dbDataReader["MtgaBoosterId"]);
            return BoosterRecord.Create(DatabaseContext, this, setId, mtgaBoosterId);
        }
    }
}