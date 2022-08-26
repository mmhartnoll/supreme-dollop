using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileHasCardRecord : DatabaseRecord<ProfileHasCardRecord>
    {
        private int _count;

        public Guid ProfileId { get; }
        public Guid DigitalCardId { get; }

        public int Count
        {
            get => _count;
            set
            {
                IsModified |= _count != value;
                _count = value;
            }
        }

        private ProfileHasCardRecord(DatabaseContext dataContext, ProfileHasCardsTable profileHasCardsTable, Guid profileId, Guid digitalCardId, int count) : base(dataContext, profileHasCardsTable)
        {
            ProfileId = profileId;
            DigitalCardId = digitalCardId;
            _count = count;
        }

        internal static ProfileHasCardRecord Create(DatabaseContext dataContext, ProfileHasCardsTable profileHasCardsTable, Guid profileId, Guid digitalCardId, int count)
        {
            return new ProfileHasCardRecord(dataContext, profileHasCardsTable, profileId, digitalCardId, count);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[ProfileHasCards] SET Count = @Count WHERE ProfileId = @ProfileId AND DigitalCardId = @DigitalCardId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("DigitalCardId", DigitalCardId);
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
                command.CommandText = "UPDATE [Mtga].[ProfileHasCards] SET Count = @Count WHERE ProfileId = @ProfileId AND DigitalCardId = @DigitalCardId;";
                command.AddParameter("ProfileId", ProfileId);
                command.AddParameter("DigitalCardId", DigitalCardId);
                command.AddParameter("Count", Count);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[ProfileHasCards] WHERE ProfileId = @ProfileId AND DigitalCardId = @DigitalCardId;";
            command.AddParameter("ProfileId", ProfileId);
            command.AddParameter("DigitalCardId", DigitalCardId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[ProfileHasCards] WHERE ProfileId = @ProfileId AND DigitalCardId = @DigitalCardId;";
            command.AddParameter("ProfileId", ProfileId);
            command.AddParameter("DigitalCardId", DigitalCardId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}