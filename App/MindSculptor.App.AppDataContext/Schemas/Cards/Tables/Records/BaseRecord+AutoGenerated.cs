using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class BaseRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid CardTypeId { get; }

        private BaseRecord(DataContext dataContext, Guid id, Guid cardTypeId) : base(dataContext)
        {
            Id = id;
            CardTypeId = cardTypeId;
        }

        internal static BaseRecord Create(DataContext dataContext, Guid id, Guid cardTypeId)
        {
            return new BaseRecord(dataContext, id, cardTypeId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[Bases] SET CardTypeId = @CardTypeId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("CardTypeId", CardTypeId);
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
                    command.CommandText = "UPDATE [Cards].[Bases] SET CardTypeId = @CardTypeId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("CardTypeId", CardTypeId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[Bases] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Cards].[Bases] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}