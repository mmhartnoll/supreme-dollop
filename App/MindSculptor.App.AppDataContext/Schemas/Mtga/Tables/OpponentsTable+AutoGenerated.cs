using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class OpponentsTable : DataContextTable<OpponentRecord, OpponentRecordExpression>
    {
        private OpponentsTable(DataContext dataContext) : base(dataContext, "Mtga", "Opponents")
        {
        }

        internal static OpponentsTable Create(DataContext dataContext)
        {
            return new OpponentsTable(dataContext);
        }

        public OpponentRecord NewRecord(string screenName, int userId)
        {
            var newRecord = OpponentRecord.Create(DataContext, Guid.NewGuid(), screenName, userId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Opponents] ( Id, ScreenName, UserId ) VALUES ( @Id, @ScreenName, @UserId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("ScreenName", newRecord.ScreenName);
                command.AddParameter("UserId", newRecord.UserId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<OpponentRecord> NewRecordAsync(string screenName, int userId)
        {
            var newRecord = OpponentRecord.Create(DataContext, Guid.NewGuid(), screenName, userId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Opponents] ( Id, ScreenName, UserId ) VALUES ( @Id, @ScreenName, @UserId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("ScreenName", newRecord.ScreenName);
                command.AddParameter("UserId", newRecord.UserId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override OpponentRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var screenName = (string)dbDataReader["ScreenName"];
            var userId = Convert.ToInt32(dbDataReader["UserId"]);
            return OpponentRecord.Create(DataContext, id, screenName, userId);
        }
    }
}