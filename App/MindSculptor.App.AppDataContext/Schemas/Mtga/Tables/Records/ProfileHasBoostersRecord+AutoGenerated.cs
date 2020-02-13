using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileHasBoostersRecord : DataContextRecord
    {
        private int _count;

        public Guid ProfileId { get; }
        public Guid BoosterId { get; }

        public int Count
        {
            get => _count;
            set
            {
                IsModified |= _count != value;
                _count = value;
            }
        }

        private ProfileHasBoostersRecord(DataContext dataContext, Guid profileId, Guid boosterId, int count) : base(dataContext)
        {
            ProfileId = profileId;
            BoosterId = boosterId;
            _count = count;
        }

        internal static ProfileHasBoostersRecord Create(DataContext dataContext, Guid profileId, Guid boosterId, int count)
        {
            return new ProfileHasBoostersRecord(dataContext, profileId, boosterId, count);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[ProfileHasBoosters] SET Count = @Count WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
                    command.AddParameter("ProfileId", ProfileId);
                    command.AddParameter("BoosterId", BoosterId);
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
                    command.CommandText = "UPDATE [Mtga].[ProfileHasBoosters] SET Count = @Count WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
                    command.AddParameter("ProfileId", ProfileId);
                    command.AddParameter("BoosterId", BoosterId);
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
                command.CommandText = "DELETE FROM [Mtga].[ProfileHasBoosters] WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("BoosterId", BoosterId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ProfileHasBoosters] WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("BoosterId", BoosterId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}