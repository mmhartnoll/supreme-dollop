using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class LoyaltyTable : DatabaseTable<LoyaltyRecord, LoyaltyRecordExpression>
    {
        private LoyaltyTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "Loyalty")
        {
        }

        internal static LoyaltyTable Create(DatabaseContext databaseContext)
        {
            return new LoyaltyTable(databaseContext);
        }

        public LoyaltyRecord NewRecord(Guid faceId, int baseLoyalty, string loyaltyFormat)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceId, baseLoyalty, loyaltyFormat));
        }

        public async Task<LoyaltyRecord> NewRecordAsync(Guid faceId, int baseLoyalty, string loyaltyFormat, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, baseLoyalty, loyaltyFormat, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public LoyaltyRecord NewRecord(FaceRecord faceRecord, int baseLoyalty, string loyaltyFormat)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceRecord.Id, baseLoyalty, loyaltyFormat));
        }

        public async Task<LoyaltyRecord> NewRecordAsync(FaceRecord faceRecord, int baseLoyalty, string loyaltyFormat, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, baseLoyalty, loyaltyFormat, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private LoyaltyRecord NewRecord(DbCommand command, Guid faceId, int baseLoyalty, string loyaltyFormat)
        {
            var newRecord = LoyaltyRecord.Create(DatabaseContext, this, faceId, baseLoyalty, loyaltyFormat);
            command.CommandText = "INSERT INTO [Cards].[Loyalty] ( FaceId, BaseLoyalty, LoyaltyFormat ) VALUES ( @FaceId, @BaseLoyalty, @LoyaltyFormat );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("BaseLoyalty", newRecord.BaseLoyalty);
            command.AddParameter("LoyaltyFormat", newRecord.LoyaltyFormat);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<LoyaltyRecord> NewRecordAsync(DbCommand command, Guid faceId, int baseLoyalty, string loyaltyFormat, CancellationToken cancellationToken)
        {
            var newRecord = LoyaltyRecord.Create(DatabaseContext, this, faceId, baseLoyalty, loyaltyFormat);
            command.CommandText = "INSERT INTO [Cards].[Loyalty] ( FaceId, BaseLoyalty, LoyaltyFormat ) VALUES ( @FaceId, @BaseLoyalty, @LoyaltyFormat );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("BaseLoyalty", newRecord.BaseLoyalty);
            command.AddParameter("LoyaltyFormat", newRecord.LoyaltyFormat);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override LoyaltyRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var baseLoyalty = Convert.ToInt32(dbDataReader["BaseLoyalty"]);
            var loyaltyFormat = (string)dbDataReader["LoyaltyFormat"];
            return LoyaltyRecord.Create(DatabaseContext, this, faceId, baseLoyalty, loyaltyFormat);
        }
    }
}