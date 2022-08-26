using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileHasBoostersRecord : DatabaseRecord<ProfileHasBoostersRecord>
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

        private ProfileHasBoostersRecord(DatabaseContext dataContext, ProfileHasBoostersTable profileHasBoostersTable, Guid profileId, Guid boosterId, int count) : base(dataContext, profileHasBoostersTable)
        {
            ProfileId = profileId;
            BoosterId = boosterId;
            _count = count;
        }

        internal static ProfileHasBoostersRecord Create(DatabaseContext dataContext, ProfileHasBoostersTable profileHasBoostersTable, Guid profileId, Guid boosterId, int count)
        {
            return new ProfileHasBoostersRecord(dataContext, profileHasBoostersTable, profileId, boosterId, count);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[ProfileHasBoosters] SET Count = @Count WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("BoosterId", BoosterId);
                command.AddParameter("Count", Count);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[ProfileHasBoosters] SET Count = @Count WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("BoosterId", BoosterId);
                command.AddParameter("Count", Count);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[ProfileHasBoosters] WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
            command.AddParameter("ProfileId", ProfileId);
            command.AddParameter("BoosterId", BoosterId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[ProfileHasBoosters] WHERE ProfileId = @ProfileId AND BoosterId = @BoosterId;";
            command.AddParameter("ProfileId", ProfileId);
            command.AddParameter("BoosterId", BoosterId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}