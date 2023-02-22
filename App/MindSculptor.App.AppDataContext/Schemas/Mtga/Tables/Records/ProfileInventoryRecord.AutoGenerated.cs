using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileInventoryRecord : DatabaseRecord<ProfileInventoryRecord>
    {
        private int _mythicRareWildcardCount;
        private int _rareWildcardCount;
        private int _uncommonWildcardCount;
        private int _commonWildcardCount;
        private int _goldCount;
        private int _gemCount;
        private decimal _vaultProgress;

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

        public decimal VaultProgress
        {
            get => _vaultProgress;
            set
            {
                IsModified |= _vaultProgress != value;
                _vaultProgress = value;
            }
        }

        private ProfileInventoryRecord(DatabaseContext databaseContext, ProfileInventoriesTable profileInventoriesTable, Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress) : base(databaseContext, profileInventoriesTable)
        {
            ProfileId = profileId;
            _mythicRareWildcardCount = mythicRareWildcardCount;
            _rareWildcardCount = rareWildcardCount;
            _uncommonWildcardCount = uncommonWildcardCount;
            _commonWildcardCount = commonWildcardCount;
            _goldCount = goldCount;
            _gemCount = gemCount;
            _vaultProgress = vaultProgress;
        }

        internal static ProfileInventoryRecord Create(DatabaseContext databaseContext, ProfileInventoriesTable profileInventoriesTable, Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress)
        {
            return new ProfileInventoryRecord(databaseContext, profileInventoriesTable, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[ProfileInventories] SET MythicRareWildcardCount = @MythicRareWildcardCount, RareWildcardCount = @RareWildcardCount, UncommonWildcardCount = @UncommonWildcardCount, CommonWildcardCount = @CommonWildcardCount, GoldCount = @GoldCount, GemCount = @GemCount, VaultProgress = @VaultProgress WHERE ProfileId = @ProfileId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("MythicRareWildcardCount", MythicRareWildcardCount);
                command.AddParameter("RareWildcardCount", RareWildcardCount);
                command.AddParameter("UncommonWildcardCount", UncommonWildcardCount);
                command.AddParameter("CommonWildcardCount", CommonWildcardCount);
                command.AddParameter("GoldCount", GoldCount);
                command.AddParameter("GemCount", GemCount);
                command.AddParameter("VaultProgress", VaultProgress);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[ProfileInventories] SET MythicRareWildcardCount = @MythicRareWildcardCount, RareWildcardCount = @RareWildcardCount, UncommonWildcardCount = @UncommonWildcardCount, CommonWildcardCount = @CommonWildcardCount, GoldCount = @GoldCount, GemCount = @GemCount, VaultProgress = @VaultProgress WHERE ProfileId = @ProfileId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("MythicRareWildcardCount", MythicRareWildcardCount);
                command.AddParameter("RareWildcardCount", RareWildcardCount);
                command.AddParameter("UncommonWildcardCount", UncommonWildcardCount);
                command.AddParameter("CommonWildcardCount", CommonWildcardCount);
                command.AddParameter("GoldCount", GoldCount);
                command.AddParameter("GemCount", GemCount);
                command.AddParameter("VaultProgress", VaultProgress);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[ProfileInventories] WHERE ProfileId = @ProfileId;";
            command.AddParameter("ProfileId", ProfileId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[ProfileInventories] WHERE ProfileId = @ProfileId;";
            command.AddParameter("ProfileId", ProfileId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}