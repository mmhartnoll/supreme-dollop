using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasCastingCostRecord : DatabaseRecord<FaceHasCastingCostRecord>
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

        private FaceHasCastingCostRecord(DatabaseContext dataContext, FaceHasCastingCostTable faceHasCastingCostTable, Guid faceId, Guid manaSymbolId, int ordinal, int count) : base(dataContext, faceHasCastingCostTable)
        {
            FaceId = faceId;
            ManaSymbolId = manaSymbolId;
            _ordinal = ordinal;
            _count = count;
        }

        internal static FaceHasCastingCostRecord Create(DatabaseContext dataContext, FaceHasCastingCostTable faceHasCastingCostTable, Guid faceId, Guid manaSymbolId, int ordinal, int count)
        {
            return new FaceHasCastingCostRecord(dataContext, faceHasCastingCostTable, faceId, manaSymbolId, ordinal, count);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FaceHasCastingCost] SET Ordinal = @Ordinal, Count = @Count WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("ManaSymbolId", ManaSymbolId);
                command.AddParameter("Ordinal", Ordinal);
                command.AddParameter("Count", Count);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FaceHasCastingCost] SET Ordinal = @Ordinal, Count = @Count WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("ManaSymbolId", ManaSymbolId);
                command.AddParameter("Ordinal", Ordinal);
                command.AddParameter("Count", Count);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasCastingCost] WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("ManaSymbolId", ManaSymbolId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasCastingCost] WHERE FaceId = @FaceId AND ManaSymbolId = @ManaSymbolId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("ManaSymbolId", ManaSymbolId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}