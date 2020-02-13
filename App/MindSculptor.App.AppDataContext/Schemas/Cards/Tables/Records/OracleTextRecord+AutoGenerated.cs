using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class OracleTextRecord : DataContextRecord
    {
        private string _value;

        public Guid FaceId { get; }

        public string Value
        {
            get => _value;
            set
            {
                IsModified |= _value != value;
                _value = value;
            }
        }

        private OracleTextRecord(DataContext dataContext, Guid faceId, string value) : base(dataContext)
        {
            FaceId = faceId;
            _value = value;
        }

        internal static OracleTextRecord Create(DataContext dataContext, Guid faceId, string value)
        {
            return new OracleTextRecord(dataContext, faceId, value);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[OracleText] SET Value = @Value WHERE FaceId = @FaceId;";
                    command.AddParameter("FaceId", FaceId);
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
                    command.CommandText = "UPDATE [Cards].[OracleText] SET Value = @Value WHERE FaceId = @FaceId;";
                    command.AddParameter("FaceId", FaceId);
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
                command.CommandText = "DELETE FROM [Cards].[OracleText] WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[OracleText] WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}