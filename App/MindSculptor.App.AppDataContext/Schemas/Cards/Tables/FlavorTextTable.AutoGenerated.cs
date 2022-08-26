using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FlavorTextTable : DatabaseTable<FlavorTextRecord, FlavorTextRecordExpression>
    {
        private FlavorTextTable(DatabaseContext dataContext) : base(dataContext, "Cards", "FlavorText")
        {
        }

        internal static FlavorTextTable Create(DatabaseContext dataContext)
        {
            return new FlavorTextTable(dataContext);
        }

        public FlavorTextRecord NewRecord(Guid facePrintingId, string value)
        {
            return Context.Execute(command => NewRecord(command, facePrintingId, value));
        }

        public async Task<FlavorTextRecord> NewRecordAsync(Guid facePrintingId, string value, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, facePrintingId, value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FlavorTextRecord NewRecord(FacePrintingRecord facePrintingRecord, string value)
        {
            return Context.Execute(command => NewRecord(command, facePrintingRecord.Id, value));
        }

        public async Task<FlavorTextRecord> NewRecordAsync(FacePrintingRecord facePrintingRecord, string value, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, facePrintingRecord.Id, value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FlavorTextRecord NewRecord(DbCommand command, Guid facePrintingId, string value)
        {
            var newRecord = FlavorTextRecord.Create(Context, this, facePrintingId, value);
            command.CommandText = "INSERT INTO [Cards].[FlavorText] ( FacePrintingId, Value ) VALUES ( @FacePrintingId, @Value );";
            command.AddParameter("FacePrintingId", newRecord.FacePrintingId);
            command.AddParameter("Value", newRecord.Value);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FlavorTextRecord> NewRecordAsync(DbCommand command, Guid facePrintingId, string value, CancellationToken cancellationToken)
        {
            var newRecord = FlavorTextRecord.Create(Context, this, facePrintingId, value);
            command.CommandText = "INSERT INTO [Cards].[FlavorText] ( FacePrintingId, Value ) VALUES ( @FacePrintingId, @Value );";
            command.AddParameter("FacePrintingId", newRecord.FacePrintingId);
            command.AddParameter("Value", newRecord.Value);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FlavorTextRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var facePrintingId = (Guid)dbDataReader["FacePrintingId"];
            var value = (string)dbDataReader["Value"];
            return FlavorTextRecord.Create(Context, this, facePrintingId, value);
        }
    }
}