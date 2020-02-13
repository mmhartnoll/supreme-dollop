using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SubsetTypeRecord : DataContextRecord
    {
        public Guid Id { get; }
        public string Value { get; }
        public string CollectorsNumberFormat { get; }

        private SubsetTypeRecord(DataContext dataContext, Guid id, string value, string collectorsNumberFormat) : base(dataContext)
        {
            Id = id;
            Value = value;
            CollectorsNumberFormat = collectorsNumberFormat;
        }

        internal static SubsetTypeRecord Create(DataContext dataContext, Guid id, string value, string collectorsNumberFormat)
        {
            return new SubsetTypeRecord(dataContext, id, value, collectorsNumberFormat);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[SubsetTypes] SET Value = @Value, CollectorsNumberFormat = @CollectorsNumberFormat WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Value", Value);
                    command.AddParameter("CollectorsNumberFormat", CollectorsNumberFormat);
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
                    command.CommandText = "UPDATE [Cards].[SubsetTypes] SET Value = @Value, CollectorsNumberFormat = @CollectorsNumberFormat WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Value", Value);
                    command.AddParameter("CollectorsNumberFormat", CollectorsNumberFormat);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[SubsetTypes] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Cards].[SubsetTypes] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}