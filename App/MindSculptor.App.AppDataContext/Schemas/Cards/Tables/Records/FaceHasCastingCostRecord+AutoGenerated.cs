using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasCastingCostRecord : DataContextRecord
    {
        private int _ordinal;
        private int _count;

        public Guid FaceId { get; }
        public Guid ManaSymbolId { get; }

        public int Ordinal
        {
            get => _ordinal;
            set
            {
                IsModified |= _ordinal != value;
                _ordinal = value;
            }
        }

        public int Count
        {
            get => _count;
            set
            {
                IsModified |= _count != value;
                _count = value;
            }
        }

        private FaceHasCastingCostRecord(DataContext dataContext, Guid faceId, Guid manaSymbolId, int ordinal, int count) : base(dataContext)
        {
            FaceId = faceId;
            ManaSymbolId = manaSymbolId;
            _ordinal = ordinal;
            _count = count;
        }

        internal static FaceHasCastingCostRecord Create(DataContext dataContext, Guid faceId, Guid manaSymbolId, int ordinal, int count)
        {
            return new FaceHasCastingCostRecord(dataContext, faceId, manaSymbolId, ordinal, count);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FaceHasCastingCost] SET Ordinal = @Ordinal, Count = @Count WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("ManaSymbolId", ManaSymbolId);
                    command.AddParameter("Ordinal", Ordinal);
                    command.AddParameter("Count", Count);
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
                    command.CommandText = "UPDATE [Cards].[FaceHasCastingCost] SET Ordinal = @Ordinal, Count = @Count WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("ManaSymbolId", ManaSymbolId);
                    command.AddParameter("Ordinal", Ordinal);
                    command.AddParameter("Count", Count);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FaceHasCastingCost] WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("ManaSymbolId", ManaSymbolId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FaceHasCastingCost] WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("ManaSymbolId", ManaSymbolId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}