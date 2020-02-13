using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class CardRecord : DataContextRecord
    {
        public Guid Id { get; }
        public int MtgaCardId { get; }

        private CardRecord(DataContext dataContext, Guid id, int mtgaCardId) : base(dataContext)
        {
            Id = id;
            MtgaCardId = mtgaCardId;
        }

        internal static CardRecord Create(DataContext dataContext, Guid id, int mtgaCardId)
        {
            return new CardRecord(dataContext, id, mtgaCardId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[Cards] SET MtgaCardId = @MtgaCardId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("MtgaCardId", MtgaCardId);
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
                    command.CommandText = "UPDATE [Mtga].[Cards] SET MtgaCardId = @MtgaCardId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("MtgaCardId", MtgaCardId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[Cards] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[Cards] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}