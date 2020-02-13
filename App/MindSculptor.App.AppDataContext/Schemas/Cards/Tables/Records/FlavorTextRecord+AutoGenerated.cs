using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FlavorTextRecord : DataContextRecord
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

        private FlavorTextRecord(DataContext dataContext, Guid facePrintingId, string value) : base(dataContext)
        {
            FacePrintingId = facePrintingId;
            _value = value;
        }

        internal static FlavorTextRecord Create(DataContext dataContext, Guid facePrintingId, string value)
        {
            return new FlavorTextRecord(dataContext, facePrintingId, value);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FlavorText] SET Value = @Value WHERE FacePrintingId = @FacePrintingId;";
                    command.AddParameter("FacePrintingId", FacePrintingId);
                    command.AddParameter("Value", Value);
                    command.ExecuteNonQuery();
                }
        }

        public async override Task UpdateRecordAsync()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FlavorText] SET Value = @Value WHERE FacePrintingId = @FacePrintingId;";
                    command.AddParameter("FacePrintingId", FacePrintingId);
                    command.AddParameter("Value", Value);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FlavorText] WHERE FacePrintingId = @FacePrintingId;";
                command.AddParameter("FacePrintingId", FacePrintingId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FlavorText] WHERE FacePrintingId = @FacePrintingId;";
                command.AddParameter("FacePrintingId", FacePrintingId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}