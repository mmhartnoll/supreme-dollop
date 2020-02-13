using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class OpponentRecord : DataContextRecord
    {
        public Guid Id { get; }
        public string ScreenName { get; }
        public int UserId { get; }

        private OpponentRecord(DataContext dataContext, Guid id, string screenName, int userId) : base(dataContext)
        {
            Id = id;
            ScreenName = screenName;
            UserId = userId;
        }

        internal static OpponentRecord Create(DataContext dataContext, Guid id, string screenName, int userId)
        {
            return new OpponentRecord(dataContext, id, screenName, userId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[Opponents] SET ScreenName = @ScreenName, UserId = @UserId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("ScreenName", ScreenName);
                    command.AddParameter("UserId", UserId);
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
                    command.CommandText = "UPDATE [Mtga].[Opponents] SET ScreenName = @ScreenName, UserId = @UserId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("ScreenName", ScreenName);
                    command.AddParameter("UserId", UserId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[Opponents] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[Opponents] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}