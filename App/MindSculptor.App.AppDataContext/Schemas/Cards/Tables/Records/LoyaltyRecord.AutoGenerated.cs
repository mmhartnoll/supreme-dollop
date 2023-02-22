using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class LoyaltyRecord : DatabaseRecord<LoyaltyRecord>
    {
        private int _baseLoyalty;
        private string _loyaltyFormat;

        public Guid FaceId { get; }

        public int BaseLoyalty
        {
            get => _baseLoyalty;
            set
            {
                IsModified |= _baseLoyalty != value;
                _baseLoyalty = value;
            }
        }

        public string LoyaltyFormat
        {
            get => _loyaltyFormat;
            set
            {
                IsModified |= _loyaltyFormat != value;
                _loyaltyFormat = value;
            }
        }

        private LoyaltyRecord(DatabaseContext databaseContext, LoyaltyTable loyaltyTable, Guid faceId, int baseLoyalty, string loyaltyFormat) : base(databaseContext, loyaltyTable)
        {
            FaceId = faceId;
            _baseLoyalty = baseLoyalty;
            _loyaltyFormat = loyaltyFormat;
        }

        internal static LoyaltyRecord Create(DatabaseContext databaseContext, LoyaltyTable loyaltyTable, Guid faceId, int baseLoyalty, string loyaltyFormat)
        {
            return new LoyaltyRecord(databaseContext, loyaltyTable, faceId, baseLoyalty, loyaltyFormat);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Loyalty] SET BaseLoyalty = @BaseLoyalty, LoyaltyFormat = @LoyaltyFormat WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("BaseLoyalty", BaseLoyalty);
                command.AddParameter("LoyaltyFormat", LoyaltyFormat);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Loyalty] SET BaseLoyalty = @BaseLoyalty, LoyaltyFormat = @LoyaltyFormat WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("BaseLoyalty", BaseLoyalty);
                command.AddParameter("LoyaltyFormat", LoyaltyFormat);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[Loyalty] WHERE FaceId = @FaceId;";
            command.AddParameter("FaceId", FaceId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[Loyalty] WHERE FaceId = @FaceId;";
            command.AddParameter("FaceId", FaceId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}