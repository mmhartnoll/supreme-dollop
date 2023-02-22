using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FlavorTextRecord : DatabaseRecord<FlavorTextRecord>
    {
        private string _value;

        public Guid FacePrintingId { get; }

        public string Value
        {
            get => _value;
            set
            {
                IsModified |= _value != value;
                _value = value;
            }
        }

        private FlavorTextRecord(DatabaseContext databaseContext, FlavorTextTable flavorTextTable, Guid facePrintingId, string value) : base(databaseContext, flavorTextTable)
        {
            FacePrintingId = facePrintingId;
            _value = value;
        }

        internal static FlavorTextRecord Create(DatabaseContext databaseContext, FlavorTextTable flavorTextTable, Guid facePrintingId, string value)
        {
            return new FlavorTextRecord(databaseContext, flavorTextTable, facePrintingId, value);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FlavorText] SET Value = @Value WHERE FacePrintingId = @FacePrintingId;";
                command.AddParameter("FacePrintingId", FacePrintingId);
                command.AddParameter("Value", Value);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FlavorText] SET Value = @Value WHERE FacePrintingId = @FacePrintingId;";
                command.AddParameter("FacePrintingId", FacePrintingId);
                command.AddParameter("Value", Value);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[FlavorText] WHERE FacePrintingId = @FacePrintingId;";
            command.AddParameter("FacePrintingId", FacePrintingId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[FlavorText] WHERE FacePrintingId = @FacePrintingId;";
            command.AddParameter("FacePrintingId", FacePrintingId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}