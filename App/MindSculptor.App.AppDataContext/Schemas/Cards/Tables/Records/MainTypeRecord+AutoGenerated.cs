using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class MainTypeRecord : DataContextRecord
    {
        public Guid Id { get; }
        public string Value { get; }

        private MainTypeRecord(DataContext dataContext, Guid id, string value) : base(dataContext)
        {
            Id = id;
            Value = value;
        }

        internal static MainTypeRecord Create(DataContext dataContext, Guid id, string value)
        {
            return new MainTypeRecord(dataContext, id, value);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[MainTypes] SET Value = @Value WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Value", Value);
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
                    command.CommandText = "UPDATE [Cards].[MainTypes] SET Value = @Value WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Value", Value);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[MainTypes] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Cards].[MainTypes] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}