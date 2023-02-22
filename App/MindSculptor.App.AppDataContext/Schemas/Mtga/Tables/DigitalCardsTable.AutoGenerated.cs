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
    public class DigitalCardsTable : DatabaseTable<DigitalCardRecord, DigitalCardRecordExpression>
    {
        private DigitalCardsTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "DigitalCards")
        {
        }

        internal static DigitalCardsTable Create(DatabaseContext databaseContext)
        {
            return new DigitalCardsTable(databaseContext);
        }

        public DigitalCardRecord NewRecord(Guid id, int mtgaCardId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, id, mtgaCardId));
        }

        public async Task<DigitalCardRecord> NewRecordAsync(Guid id, int mtgaCardId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, id, mtgaCardId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public DigitalCardRecord NewRecord(BasePrintingRecord basePrintingRecord, int mtgaCardId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, basePrintingRecord.Id, mtgaCardId));
        }

        public async Task<DigitalCardRecord> NewRecordAsync(BasePrintingRecord basePrintingRecord, int mtgaCardId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, basePrintingRecord.Id, mtgaCardId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private DigitalCardRecord NewRecord(DbCommand command, Guid id, int mtgaCardId)
        {
            var newRecord = DigitalCardRecord.Create(DatabaseContext, this, id, mtgaCardId);
            command.CommandText = "INSERT INTO [Mtga].[DigitalCards] ( Id, MtgaCardId ) VALUES ( @Id, @MtgaCardId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("MtgaCardId", newRecord.MtgaCardId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<DigitalCardRecord> NewRecordAsync(DbCommand command, Guid id, int mtgaCardId, CancellationToken cancellationToken)
        {
            var newRecord = DigitalCardRecord.Create(DatabaseContext, this, id, mtgaCardId);
            command.CommandText = "INSERT INTO [Mtga].[DigitalCards] ( Id, MtgaCardId ) VALUES ( @Id, @MtgaCardId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("MtgaCardId", newRecord.MtgaCardId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override DigitalCardRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var mtgaCardId = Convert.ToInt32(dbDataReader["MtgaCardId"]);
            return DigitalCardRecord.Create(DatabaseContext, this, id, mtgaCardId);
        }
    }
}