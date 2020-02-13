using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class LoyaltyRecord : DataContextRecord
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

        private LoyaltyRecord(DataContext dataContext, Guid faceId, int baseLoyalty, string loyaltyFormat) : base(dataContext)
        {
            FaceId = faceId;
            _baseLoyalty = baseLoyalty;
            _loyaltyFormat = loyaltyFormat;
        }

        internal static LoyaltyRecord Create(DataContext dataContext, Guid faceId, int baseLoyalty, string loyaltyFormat)
        {
            return new LoyaltyRecord(dataContext, faceId, baseLoyalty, loyaltyFormat);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[Loyalty] SET BaseLoyalty = @BaseLoyalty, LoyaltyFormat = @LoyaltyFormat WHERE FaceId = @FaceId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("BaseLoyalty", BaseLoyalty);
                    command.AddParameter("LoyaltyFormat", LoyaltyFormat);
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
                    command.CommandText = "UPDATE [Cards].[Loyalty] SET BaseLoyalty = @BaseLoyalty, LoyaltyFormat = @LoyaltyFormat WHERE FaceId = @FaceId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("BaseLoyalty", BaseLoyalty);
                    command.AddParameter("LoyaltyFormat", LoyaltyFormat);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[Loyalty] WHERE FaceId = @FaceId;";
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
                command.CommandText = "DELETE FROM [Cards].[Loyalty] WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}