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
    public class FacesTable : DatabaseTable<FaceRecord, FaceRecordExpression>
    {
        private FacesTable(DatabaseContext dataContext) : base(dataContext, "Cards", "Faces")
        {
        }

        internal static FacesTable Create(DatabaseContext dataContext)
        {
            return new FacesTable(dataContext);
        }

        public FaceRecord NewRecord(Guid baseId, string name, bool isPrimaryFace)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), baseId, name, isPrimaryFace));
        }

        public async Task<FaceRecord> NewRecordAsync(Guid baseId, string name, bool isPrimaryFace, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), baseId, name, isPrimaryFace, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FaceRecord NewRecord(BaseRecord baseRecord, string name, bool isPrimaryFace)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), baseRecord.Id, name, isPrimaryFace));
        }

        public async Task<FaceRecord> NewRecordAsync(BaseRecord baseRecord, string name, bool isPrimaryFace, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), baseRecord.Id, name, isPrimaryFace, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FaceRecord NewRecord(DbCommand command, Guid id, Guid baseId, string name, bool isPrimaryFace)
        {
            var newRecord = FaceRecord.Create(Context, this, id, baseId, name, isPrimaryFace);
            command.CommandText = "INSERT INTO [Cards].[Faces] ( Id, BaseId, Name, IsPrimaryFace ) VALUES ( @Id, @BaseId, @Name, @IsPrimaryFace );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("BaseId", newRecord.BaseId);
            command.AddParameter("Name", newRecord.Name);
            command.AddParameter("IsPrimaryFace", newRecord.IsPrimaryFace);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FaceRecord> NewRecordAsync(DbCommand command, Guid id, Guid baseId, string name, bool isPrimaryFace, CancellationToken cancellationToken)
        {
            var newRecord = FaceRecord.Create(Context, this, id, baseId, name, isPrimaryFace);
            command.CommandText = "INSERT INTO [Cards].[Faces] ( Id, BaseId, Name, IsPrimaryFace ) VALUES ( @Id, @BaseId, @Name, @IsPrimaryFace );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("BaseId", newRecord.BaseId);
            command.AddParameter("Name", newRecord.Name);
            command.AddParameter("IsPrimaryFace", newRecord.IsPrimaryFace);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FaceRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var baseId = (Guid)dbDataReader["BaseId"];
            var name = (string)dbDataReader["Name"];
            var isPrimaryFace = (bool)dbDataReader["IsPrimaryFace"];
            return FaceRecord.Create(Context, this, id, baseId, name, isPrimaryFace);
        }
    }
}