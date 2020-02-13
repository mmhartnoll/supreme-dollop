using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftEventRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid SetId { get; }
        public string DraftType { get; }

        private DraftEventRecord(DataContext dataContext, Guid id, Guid setId, string draftType) : base(dataContext)
        {
            Id = id;
            SetId = setId;
            DraftType = draftType;
        }

        internal static DraftEventRecord Create(DataContext dataContext, Guid id, Guid setId, string draftType)
        {
            return new DraftEventRecord(dataContext, id, setId, draftType);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[DraftEvents] SET SetId = @SetId, DraftType = @DraftType WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("SetId", SetId);
                    command.AddParameter("DraftType", DraftType);
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
                    command.CommandText = "UPDATE [Mtga].[DraftEvents] SET SetId = @SetId, DraftType = @DraftType WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("SetId", SetId);
                    command.AddParameter("DraftType", DraftType);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[DraftEvents] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[DraftEvents] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}