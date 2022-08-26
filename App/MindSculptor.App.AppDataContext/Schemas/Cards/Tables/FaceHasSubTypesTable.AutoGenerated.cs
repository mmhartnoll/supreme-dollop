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
    public class FaceHasSubTypesTable : DatabaseTable<FaceHasSubTypeRecord, FaceHasSubTypeRecordExpression>
    {
        private FaceHasSubTypesTable(DatabaseContext dataContext) : base(dataContext, "Cards", "FaceHasSubTypes")
        {
        }

        internal static FaceHasSubTypesTable Create(DatabaseContext dataContext)
        {
            return new FaceHasSubTypesTable(dataContext);
        }

        public FaceHasSubTypeRecord NewRecord(Guid faceId, Guid subTypeId, int ordinal)
        {
            return Context.Execute(command => NewRecord(command, faceId, subTypeId, ordinal));
        }

        public async Task<FaceHasSubTypeRecord> NewRecordAsync(Guid faceId, Guid subTypeId, int ordinal, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, subTypeId, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FaceHasSubTypeRecord NewRecord(FaceRecord faceRecord, SubTypeRecord subTypeRecord, int ordinal)
        {
            return Context.Execute(command => NewRecord(command, faceRecord.Id, subTypeRecord.Id, ordinal));
        }

        public async Task<FaceHasSubTypeRecord> NewRecordAsync(FaceRecord faceRecord, SubTypeRecord subTypeRecord, int ordinal, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, subTypeRecord.Id, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FaceHasSubTypeRecord NewRecord(DbCommand command, Guid faceId, Guid subTypeId, int ordinal)
        {
            var newRecord = FaceHasSubTypeRecord.Create(Context, this, faceId, subTypeId, ordinal);
            command.CommandText = "INSERT INTO [Cards].[FaceHasSubTypes] ( FaceId, SubTypeId, Ordinal ) VALUES ( @FaceId, @SubTypeId, @Ordinal );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("SubTypeId", newRecord.SubTypeId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FaceHasSubTypeRecord> NewRecordAsync(DbCommand command, Guid faceId, Guid subTypeId, int ordinal, CancellationToken cancellationToken)
        {
            var newRecord = FaceHasSubTypeRecord.Create(Context, this, faceId, subTypeId, ordinal);
            command.CommandText = "INSERT INTO [Cards].[FaceHasSubTypes] ( FaceId, SubTypeId, Ordinal ) VALUES ( @FaceId, @SubTypeId, @Ordinal );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("SubTypeId", newRecord.SubTypeId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FaceHasSubTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var subTypeId = (Guid)dbDataReader["SubTypeId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return FaceHasSubTypeRecord.Create(Context, this, faceId, subTypeId, ordinal);
        }
    }
}