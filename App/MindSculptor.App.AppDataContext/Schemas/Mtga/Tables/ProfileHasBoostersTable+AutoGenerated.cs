using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ProfileHasBoostersTable : DataContextTable<ProfileHasBoostersRecord, ProfileHasBoostersRecordExpression>
    {
        private ProfileHasBoostersTable(DataContext dataContext) : base(dataContext, "Mtga", "ProfileHasBoosters")
        {
        }

        internal static ProfileHasBoostersTable Create(DataContext dataContext)
        {
            return new ProfileHasBoostersTable(dataContext);
        }

        public ProfileHasBoostersRecord NewRecord(ProfileRecord profileRecord, BoosterRecord boosterRecord, int count)
        {
            var newRecord = ProfileHasBoostersRecord.Create(DataContext, profileRecord.Id, boosterRecord.SetId, count);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ProfileHasBoosters] ( ProfileId, BoosterId, Count ) VALUES ( @ProfileId, @BoosterId, @Count );";
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.AddParameter("BoosterId", newRecord.BoosterId);
                command.AddParameter("Count", newRecord.Count);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<ProfileHasBoostersRecord> NewRecordAsync(ProfileRecord profileRecord, BoosterRecord boosterRecord, int count)
        {
            var newRecord = ProfileHasBoostersRecord.Create(DataContext, profileRecord.Id, boosterRecord.SetId, count);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ProfileHasBoosters] ( ProfileId, BoosterId, Count ) VALUES ( @ProfileId, @BoosterId, @Count );";
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.AddParameter("BoosterId", newRecord.BoosterId);
                command.AddParameter("Count", newRecord.Count);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override ProfileHasBoostersRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var profileId = (Guid)dbDataReader["ProfileId"];
            var boosterId = (Guid)dbDataReader["BoosterId"];
            var count = Convert.ToInt32(dbDataReader["Count"]);
            return ProfileHasBoostersRecord.Create(DataContext, profileId, boosterId, count);
        }
    }
}