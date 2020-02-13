using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class BoosterRecord : DataContextRecord
    {
        public Guid SetId { get; }
        public int MtgaBoosterId { get; }

        private BoosterRecord(DataContext dataContext, Guid setId, int mtgaBoosterId) : base(dataContext)
        {
            SetId = setId;
            MtgaBoosterId = mtgaBoosterId;
        }

        internal static BoosterRecord Create(DataContext dataContext, Guid setId, int mtgaBoosterId)
        {
            return new BoosterRecord(dataContext, setId, mtgaBoosterId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[Boosters] SET MtgaBoosterId = @MtgaBoosterId WHERE SetId = @SetId;";
                    command.AddParameter("SetId", SetId);
                    command.AddParameter("MtgaBoosterId", MtgaBoosterId);
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
                    command.CommandText = "UPDATE [Mtga].[Boosters] SET MtgaBoosterId = @MtgaBoosterId WHERE SetId = @SetId;";
                    command.AddParameter("SetId", SetId);
                    command.AddParameter("MtgaBoosterId", MtgaBoosterId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[Boosters] WHERE SetId = @SetId;";
                command.AddParameter("SetId", SetId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[Boosters] WHERE SetId = @SetId;";
                command.AddParameter("SetId", SetId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}