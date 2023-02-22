using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class BasePrintingRecord : DatabaseRecord<BasePrintingRecord>
    {
        private Guid _artistId;

        public Guid Id { get; }
        public Guid SetInclusionId { get; }
        public Guid PrintingTypeId { get; }

        public Guid ArtistId
        {
            get => _artistId;
            set
            {
                IsModified |= _artistId != value;
                _artistId = value;
            }
        }

        private BasePrintingRecord(DatabaseContext databaseContext, BasePrintingsTable basePrintingsTable, Guid id, Guid setInclusionId, Guid printingTypeId, Guid artistId) : base(databaseContext, basePrintingsTable)
        {
            Id = id;
            SetInclusionId = setInclusionId;
            PrintingTypeId = printingTypeId;
            _artistId = artistId;
        }

        internal static BasePrintingRecord Create(DatabaseContext databaseContext, BasePrintingsTable basePrintingsTable, Guid id, Guid setInclusionId, Guid printingTypeId, Guid artistId)
        {
            return new BasePrintingRecord(databaseContext, basePrintingsTable, id, setInclusionId, printingTypeId, artistId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[BasePrintings] SET SetInclusionId = @SetInclusionId, PrintingTypeId = @PrintingTypeId, ArtistId = @ArtistId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("SetInclusionId", SetInclusionId);
                command.AddParameter("PrintingTypeId", PrintingTypeId);
                command.AddParameter("ArtistId", ArtistId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[BasePrintings] SET SetInclusionId = @SetInclusionId, PrintingTypeId = @PrintingTypeId, ArtistId = @ArtistId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("SetInclusionId", SetInclusionId);
                command.AddParameter("PrintingTypeId", PrintingTypeId);
                command.AddParameter("ArtistId", ArtistId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[BasePrintings] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[BasePrintings] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}