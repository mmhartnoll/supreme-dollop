using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ProfileHasCardsTable : DataContextTable<ProfileHasCardRecord, ProfileHasCardRecordExpression>
    {
        private ProfileHasCardsTable(DataContext dataContext) : base(dataContext, "Mtga", "ProfileHasCards")
        {
        }

        internal static ProfileHasCardsTable Create(DataContext dataContext)
        {
            return new ProfileHasCardsTable(dataContext);
        }

        public ProfileHasCardRecord NewRecord(ProfileRecord profileRecord, CardRecord cardRecord, int count)
        {
            var newRecord = ProfileHasCardRecord.Create(DataContext, profileRecord.Id, cardRecord.Id, count);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ProfileHasCards] ( ProfileId, BasePrintingId, Count ) VALUES ( @ProfileId, @BasePrintingId, @Count );";
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.AddParameter("BasePrintingId", newRecord.BasePrintingId);
                command.AddParameter("Count", newRecord.Count);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<ProfileHasCardRecord> NewRecordAsync(ProfileRecord profileRecord, CardRecord cardRecord, int count)
        {
            var newRecord = ProfileHasCardRecord.Create(DataContext, profileRecord.Id, cardRecord.Id, count);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ProfileHasCards] ( ProfileId, BasePrintingId, Count ) VALUES ( @ProfileId, @BasePrintingId, @Count );";
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.AddParameter("BasePrintingId", newRecord.BasePrintingId);
                command.AddParameter("Count", newRecord.Count);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override ProfileHasCardRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var profileId = (Guid)dbDataReader["ProfileId"];
            var basePrintingId = (Guid)dbDataReader["BasePrintingId"];
            var count = Convert.ToInt32(dbDataReader["Count"]);
            return ProfileHasCardRecord.Create(DataContext, profileId, basePrintingId, count);
        }
    }
}