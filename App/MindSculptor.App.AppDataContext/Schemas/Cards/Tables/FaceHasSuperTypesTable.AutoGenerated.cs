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
    public class FaceHasSuperTypesTable : DatabaseTable<FaceHasSuperTypeRecord, FaceHasSuperTypeRecordExpression>
    {
        private FaceHasSuperTypesTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "FaceHasSuperTypes")
        {
        }

        internal static FaceHasSuperTypesTable Create(DatabaseContext databaseContext)
        {
            return new FaceHasSuperTypesTable(databaseContext);
        }

        public FaceHasSuperTypeRecord NewRecord(Guid faceId, Guid superTypeId, int ordinal)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceId, superTypeId, ordinal));
        }

        public async Task<FaceHasSuperTypeRecord> NewRecordAsync(Guid faceId, Guid superTypeId, int ordinal, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, superTypeId, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FaceHasSuperTypeRecord NewRecord(FaceRecord faceRecord, SuperTypeRecord superTypeRecord, int ordinal)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceRecord.Id, superTypeRecord.Id, ordinal));
        }

        public async Task<FaceHasSuperTypeRecord> NewRecordAsync(FaceRecord faceRecord, SuperTypeRecord superTypeRecord, int ordinal, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, superTypeRecord.Id, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FaceHasSuperTypeRecord NewRecord(DbCommand command, Guid faceId, Guid superTypeId, int ordinal)
        {
            var newRecord = FaceHasSuperTypeRecord.Create(DatabaseContext, this, faceId, superTypeId, ordinal);
            command.CommandText = "INSERT INTO [Cards].[FaceHasSuperTypes] ( FaceId, SuperTypeId, Ordinal ) VALUES ( @FaceId, @SuperTypeId, @Ordinal );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("SuperTypeId", newRecord.SuperTypeId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FaceHasSuperTypeRecord> NewRecordAsync(DbCommand command, Guid faceId, Guid superTypeId, int ordinal, CancellationToken cancellationToken)
        {
            var newRecord = FaceHasSuperTypeRecord.Create(DatabaseContext, this, faceId, superTypeId, ordinal);
            command.CommandText = "INSERT INTO [Cards].[FaceHasSuperTypes] ( FaceId, SuperTypeId, Ordinal ) VALUES ( @FaceId, @SuperTypeId, @Ordinal );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("SuperTypeId", newRecord.SuperTypeId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FaceHasSuperTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var superTypeId = (Guid)dbDataReader["SuperTypeId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return FaceHasSuperTypeRecord.Create(DatabaseContext, this, faceId, superTypeId, ordinal);
        }
    }
}