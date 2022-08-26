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
    public class OracleTextTable : DatabaseTable<OracleTextRecord, OracleTextRecordExpression>
    {
        private OracleTextTable(DatabaseContext dataContext) : base(dataContext, "Cards", "OracleText")
        {
        }

        internal static OracleTextTable Create(DatabaseContext dataContext)
        {
            return new OracleTextTable(dataContext);
        }

        public OracleTextRecord NewRecord(Guid faceId, string value)
        {
            return Context.Execute(command => NewRecord(command, faceId, value));
        }

        public async Task<OracleTextRecord> NewRecordAsync(Guid faceId, string value, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public OracleTextRecord NewRecord(FaceRecord faceRecord, string value)
        {
            return Context.Execute(command => NewRecord(command, faceRecord.Id, value));
        }

        public async Task<OracleTextRecord> NewRecordAsync(FaceRecord faceRecord, string value, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private OracleTextRecord NewRecord(DbCommand command, Guid faceId, string value)
        {
            var newRecord = OracleTextRecord.Create(Context, this, faceId, value);
            command.CommandText = "INSERT INTO [Cards].[OracleText] ( FaceId, Value ) VALUES ( @FaceId, @Value );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("Value", newRecord.Value);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<OracleTextRecord> NewRecordAsync(DbCommand command, Guid faceId, string value, CancellationToken cancellationToken)
        {
            var newRecord = OracleTextRecord.Create(Context, this, faceId, value);
            command.CommandText = "INSERT INTO [Cards].[OracleText] ( FaceId, Value ) VALUES ( @FaceId, @Value );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("Value", newRecord.Value);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override OracleTextRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var value = (string)dbDataReader["Value"];
            return OracleTextRecord.Create(Context, this, faceId, value);
        }
    }
}