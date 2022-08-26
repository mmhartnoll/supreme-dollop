using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FaceHasMainTypesTable : DatabaseTable<FaceHasMainTypeRecord, FaceHasMainTypeRecordExpression>
    {
        private FaceHasMainTypesTable(DatabaseContext dataContext) : base(dataContext, "Cards", "FaceHasMainTypes")
        {
        }

        internal static FaceHasMainTypesTable Create(DatabaseContext dataContext)
        {
            return new FaceHasMainTypesTable(dataContext);
        }

        public FaceHasMainTypeRecord NewRecord(Guid faceId, Guid mainTypeId, int ordinal)
        {
            return Context.Execute(command => NewRecord(command, faceId, mainTypeId, ordinal));
        }

        public async Task<FaceHasMainTypeRecord> NewRecordAsync(Guid faceId, Guid mainTypeId, int ordinal, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, mainTypeId, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FaceHasMainTypeRecord NewRecord(FaceRecord faceRecord, MainTypeRecord mainTypeRecord, int ordinal)
        {
            return Context.Execute(command => NewRecord(command, faceRecord.Id, mainTypeRecord.Id, ordinal));
        }

        public async Task<FaceHasMainTypeRecord> NewRecordAsync(FaceRecord faceRecord, MainTypeRecord mainTypeRecord, int ordinal, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, mainTypeRecord.Id, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FaceHasMainTypeRecord NewRecord(DbCommand command, Guid faceId, Guid mainTypeId, int ordinal)
        {
            var newRecord = FaceHasMainTypeRecord.Create(Context, this, faceId, mainTypeId, ordinal);
            command.CommandText = "INSERT INTO [Cards].[FaceHasMainTypes] ( FaceId, MainTypeId, Ordinal ) VALUES ( @FaceId, @MainTypeId, @Ordinal );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("MainTypeId", newRecord.MainTypeId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FaceHasMainTypeRecord> NewRecordAsync(DbCommand command, Guid faceId, Guid mainTypeId, int ordinal, CancellationToken cancellationToken)
        {
            var newRecord = FaceHasMainTypeRecord.Create(Context, this, faceId, mainTypeId, ordinal);
            command.CommandText = "INSERT INTO [Cards].[FaceHasMainTypes] ( FaceId, MainTypeId, Ordinal ) VALUES ( @FaceId, @MainTypeId, @Ordinal );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("MainTypeId", newRecord.MainTypeId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FaceHasMainTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var mainTypeId = (Guid)dbDataReader["MainTypeId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return FaceHasMainTypeRecord.Create(Context, this, faceId, mainTypeId, ordinal);
        }
    }
}