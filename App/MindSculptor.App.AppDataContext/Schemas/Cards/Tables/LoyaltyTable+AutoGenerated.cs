using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class LoyaltyTable : DataContextTable<LoyaltyRecord, LoyaltyRecordExpression>
    {
        private LoyaltyTable(DataContext dataContext) : base(dataContext, "Cards", "Loyalty")
        {
        }

        internal static LoyaltyTable Create(DataContext dataContext)
        {
            return new LoyaltyTable(dataContext);
        }

        public LoyaltyRecord NewRecord(FaceRecord faceRecord, int baseLoyalty, string loyaltyFormat)
        {
            var newRecord = LoyaltyRecord.Create(DataContext, faceRecord.Id, baseLoyalty, loyaltyFormat);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Loyalty] ( FaceId, BaseLoyalty, LoyaltyFormat ) VALUES ( @FaceId, @BaseLoyalty, @LoyaltyFormat );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("BaseLoyalty", newRecord.BaseLoyalty);
                command.AddParameter("LoyaltyFormat", newRecord.LoyaltyFormat);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<LoyaltyRecord> NewRecordAsync(FaceRecord faceRecord, int baseLoyalty, string loyaltyFormat)
        {
            var newRecord = LoyaltyRecord.Create(DataContext, faceRecord.Id, baseLoyalty, loyaltyFormat);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Loyalty] ( FaceId, BaseLoyalty, LoyaltyFormat ) VALUES ( @FaceId, @BaseLoyalty, @LoyaltyFormat );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("BaseLoyalty", newRecord.BaseLoyalty);
                command.AddParameter("LoyaltyFormat", newRecord.LoyaltyFormat);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override LoyaltyRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var baseLoyalty = Convert.ToInt32(dbDataReader["BaseLoyalty"]);
            var loyaltyFormat = (string)dbDataReader["LoyaltyFormat"];
            return LoyaltyRecord.Create(DataContext, faceId, baseLoyalty, loyaltyFormat);
        }
    }
}