using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileInventoryRecord : DataContextRecord
    {
        private int _mythicRareWildcardCount;
        private int _rareWildcardCount;
        private int _uncommonWildcardCount;
        private int _commonWildcardCount;
        private int _goldCount;
        private int _gemCount;

        public Guid ProfileId { get; }

        public int MythicRareWildcardCount
        {
            get => _mythicRareWildcardCount;
            set
            {
                IsModified |= _mythicRareWildcardCount != value;
                _mythicRareWildcardCount = value;
            }
        }

        public int RareWildcardCount
        {
            get => _rareWildcardCount;
            set
            {
                IsModified |= _rareWildcardCount != value;
                _rareWildcardCount = value;
            }
        }

        public int UncommonWildcardCount
        {
            get => _uncommonWildcardCount;
            set
            {
                IsModified |= _uncommonWildcardCount != value;
                _uncommonWildcardCount = value;
            }
        }

        public int CommonWildcardCount
        {
            get => _commonWildcardCount;
            set
            {
                IsModified |= _commonWildcardCount != value;
                _commonWildcardCount = value;
            }
        }

        public int GoldCount
        {
            get => _goldCount;
            set
            {
                IsModified |= _goldCount != value;
                _goldCount = value;
            }
        }

        public int GemCount
        {
            get => _gemCount;
            set
            {
                IsModified |= _gemCount != value;
                _gemCount = value;
            }
        }

        private ProfileInventoryRecord(DataContext dataContext, Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount) : base(dataContext)
        {
            ProfileId = profileId;
            _mythicRareWildcardCount = mythicRareWildcardCount;
            _rareWildcardCount = rareWildcardCount;
            _uncommonWildcardCount = uncommonWildcardCount;
            _commonWildcardCount = commonWildcardCount;
            _goldCount = goldCount;
            _gemCount = gemCount;
        }

        internal static ProfileInventoryRecord Create(DataContext dataContext, Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount)
        {
            return new ProfileInventoryRecord(dataContext, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[ProfileInventories] SET MythicRareWildcardCount = @MythicRareWildcardCount, RareWildcardCount = @RareWildcardCount, UncommonWildcardCount = @UncommonWildcardCount, CommonWildcardCount = @CommonWildcardCount, GoldCount = @GoldCount, GemCount = @GemCount WHERE ProfileId = @ProfileId;";
                    command.AddParameter("ProfileId", ProfileId);
                    command.AddParameter("MythicRareWildcardCount", MythicRareWildcardCount);
                    command.AddParameter("RareWildcardCount", RareWildcardCount);
                    command.AddParameter("UncommonWildcardCount", UncommonWildcardCount);
                    command.AddParameter("CommonWildcardCount", CommonWildcardCount);
                    command.AddParameter("GoldCount", GoldCount);
                    command.AddParameter("GemCount", GemCount);
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
                    command.CommandText = "UPDATE [Mtga].[ProfileInventories] SET MythicRareWildcardCount = @MythicRareWildcardCount, RareWildcardCount = @RareWildcardCount, UncommonWildcardCount = @UncommonWildcardCount, CommonWildcardCount = @CommonWildcardCount, GoldCount = @GoldCount, GemCount = @GemCount WHERE ProfileId = @ProfileId;";
                    command.AddParameter("ProfileId", ProfileId);
                    command.AddParameter("MythicRareWildcardCount", MythicRareWildcardCount);
                    command.AddParameter("RareWildcardCount", RareWildcardCount);
                    command.AddParameter("UncommonWildcardCount", UncommonWildcardCount);
                    command.AddParameter("CommonWildcardCount", CommonWildcardCount);
                    command.AddParameter("GoldCount", GoldCount);
                    command.AddParameter("GemCount", GemCount);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ProfileInventories] WHERE ProfileId = @ProfileId;";
                command.AddParameter("ProfileId", ProfileId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ProfileInventories] WHERE ProfileId = @ProfileId;";
                command.AddParameter("ProfileId", ProfileId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}