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
    public class BasePrintingsTable : DatabaseTable<BasePrintingRecord, BasePrintingRecordExpression>
    {
        private BasePrintingsTable(DatabaseContext dataContext) : base(dataContext, "Cards", "BasePrintings")
        {
        }

        internal static BasePrintingsTable Create(DatabaseContext dataContext)
        {
            return new BasePrintingsTable(dataContext);
        }

        public BasePrintingRecord NewRecord(Guid setInclusionId, Guid printingTypeId, Guid artistId)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), setInclusionId, printingTypeId, artistId));
        }

        public async Task<BasePrintingRecord> NewRecordAsync(Guid setInclusionId, Guid printingTypeId, Guid artistId, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), setInclusionId, printingTypeId, artistId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public BasePrintingRecord NewRecord(SetInclusionRecord setInclusionRecord, PrintingTypeRecord printingTypeRecord, ArtistRecord artistRecord)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), setInclusionRecord.Id, printingTypeRecord.Id, artistRecord.Id));
        }

        public async Task<BasePrintingRecord> NewRecordAsync(SetInclusionRecord setInclusionRecord, PrintingTypeRecord printingTypeRecord, ArtistRecord artistRecord, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), setInclusionRecord.Id, printingTypeRecord.Id, artistRecord.Id, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private BasePrintingRecord NewRecord(DbCommand command, Guid id, Guid setInclusionId, Guid printingTypeId, Guid artistId)
        {
            var newRecord = BasePrintingRecord.Create(Context, this, id, setInclusionId, printingTypeId, artistId);
            command.CommandText = "INSERT INTO [Cards].[BasePrintings] ( Id, SetInclusionId, PrintingTypeId, ArtistId ) VALUES ( @Id, @SetInclusionId, @PrintingTypeId, @ArtistId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("SetInclusionId", newRecord.SetInclusionId);
            command.AddParameter("PrintingTypeId", newRecord.PrintingTypeId);
            command.AddParameter("ArtistId", newRecord.ArtistId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<BasePrintingRecord> NewRecordAsync(DbCommand command, Guid id, Guid setInclusionId, Guid printingTypeId, Guid artistId, CancellationToken cancellationToken)
        {
            var newRecord = BasePrintingRecord.Create(Context, this, id, setInclusionId, printingTypeId, artistId);
            command.CommandText = "INSERT INTO [Cards].[BasePrintings] ( Id, SetInclusionId, PrintingTypeId, ArtistId ) VALUES ( @Id, @SetInclusionId, @PrintingTypeId, @ArtistId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("SetInclusionId", newRecord.SetInclusionId);
            command.AddParameter("PrintingTypeId", newRecord.PrintingTypeId);
            command.AddParameter("ArtistId", newRecord.ArtistId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override BasePrintingRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var setInclusionId = (Guid)dbDataReader["SetInclusionId"];
            var printingTypeId = (Guid)dbDataReader["PrintingTypeId"];
            var artistId = (Guid)dbDataReader["ArtistId"];
            return BasePrintingRecord.Create(Context, this, id, setInclusionId, printingTypeId, artistId);
        }
    }
}