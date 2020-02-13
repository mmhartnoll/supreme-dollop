using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftPickRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid DraftEventEntryId { get; }
        public Guid DigitalCardId { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public int Ordinal { get; }
        public bool IsPick { get; }

        private DraftPickRecord(DataContext dataContext, Guid id, Guid draftEventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal, bool isPick) : base(dataContext)
        {
            Id = id;
            DraftEventEntryId = draftEventEntryId;
            DigitalCardId = digitalCardId;
            PackNumber = packNumber;
            PickNumber = pickNumber;
            Ordinal = ordinal;
            IsPick = isPick;
        }

        internal static DraftPickRecord Create(DataContext dataContext, Guid id, Guid draftEventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal, bool isPick)
        {
            return new DraftPickRecord(dataContext, id, draftEventEntryId, digitalCardId, packNumber, pickNumber, ordinal, isPick);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[DraftPicks] SET DraftEventEntryId = @DraftEventEntryId, DigitalCardId = @DigitalCardId, PackNumber = @PackNumber, PickNumber = @PickNumber, Ordinal = @Ordinal, IsPick = @IsPick WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                    command.AddParameter("DigitalCardId", DigitalCardId);
                    command.AddParameter("PackNumber", PackNumber);
                    command.AddParameter("PickNumber", PickNumber);
                    command.AddParameter("Ordinal", Ordinal);
                    command.AddParameter("IsPick", IsPick);
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
                    command.CommandText = "UPDATE [Mtga].[DraftPicks] SET DraftEventEntryId = @DraftEventEntryId, DigitalCardId = @DigitalCardId, PackNumber = @PackNumber, PickNumber = @PickNumber, Ordinal = @Ordinal, IsPick = @IsPick WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                    command.AddParameter("DigitalCardId", DigitalCardId);
                    command.AddParameter("PackNumber", PackNumber);
                    command.AddParameter("PickNumber", PickNumber);
                    command.AddParameter("Ordinal", Ordinal);
                    command.AddParameter("IsPick", IsPick);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[DraftPicks] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[DraftPicks] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}