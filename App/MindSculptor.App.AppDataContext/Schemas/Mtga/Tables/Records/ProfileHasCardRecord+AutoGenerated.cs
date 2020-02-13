using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileHasCardRecord : DataContextRecord
    {
        private int _count;

        public Guid ProfileId { get; }
        public Guid BasePrintingId { get; }

        public int Count
        {
            get => _count;
            set
            {
                IsModified |= _count != value;
                _count = value;
            }
        }

        private ProfileHasCardRecord(DataContext dataContext, Guid profileId, Guid basePrintingId, int count) : base(dataContext)
        {
            ProfileId = profileId;
            BasePrintingId = basePrintingId;
            _count = count;
        }

        internal static ProfileHasCardRecord Create(DataContext dataContext, Guid profileId, Guid basePrintingId, int count)
        {
            return new ProfileHasCardRecord(dataContext, profileId, basePrintingId, count);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[ProfileHasCards] SET Count = @Count WHERE ProfileId = @ProfileId AND BasePrintingId = @BasePrintingId;";
                    command.AddParameter("ProfileId", ProfileId);
                    command.AddParameter("BasePrintingId", BasePrintingId);
                    command.AddParameter("Count", Count);
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
                    command.CommandText = "UPDATE [Mtga].[ProfileHasCards] SET Count = @Count WHERE ProfileId = @ProfileId AND BasePrintingId = @BasePrintingId;";
                    command.AddParameter("ProfileId", ProfileId);
                    command.AddParameter("BasePrintingId", BasePrintingId);
                    command.AddParameter("Count", Count);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ProfileHasCards] WHERE ProfileId = @ProfileId AND BasePrintingId = @BasePrintingId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("BasePrintingId", BasePrintingId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ProfileHasCards] WHERE ProfileId = @ProfileId AND BasePrintingId = @BasePrintingId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("BasePrintingId", BasePrintingId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}