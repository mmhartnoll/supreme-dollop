using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ProfilesTable : DatabaseTable<ProfileRecord, ProfileRecordExpression>
    {
        private ProfilesTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "Profiles")
        {
        }

        internal static ProfilesTable Create(DatabaseContext dataContext)
        {
            return new ProfilesTable(dataContext);
        }

        public ProfileRecord NewRecord(Guid id)
        {
            return Context.Execute(command => NewRecord(command, id));
        }

        public async Task<ProfileRecord> NewRecordAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, id, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public ProfileRecord NewRecord(PlayerRecord playerRecord)
        {
            return Context.Execute(command => NewRecord(command, playerRecord.Id));
        }

        public async Task<ProfileRecord> NewRecordAsync(PlayerRecord playerRecord, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, playerRecord.Id, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ProfileRecord NewRecord(DbCommand command, Guid id)
        {
            var newRecord = ProfileRecord.Create(Context, this, id);
            command.CommandText = "INSERT INTO [Mtga].[Profiles] ( Id ) VALUES ( @Id );";
            command.AddParameter("Id", newRecord.Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ProfileRecord> NewRecordAsync(DbCommand command, Guid id, CancellationToken cancellationToken)
        {
            var newRecord = ProfileRecord.Create(Context, this, id);
            command.CommandText = "INSERT INTO [Mtga].[Profiles] ( Id ) VALUES ( @Id );";
            command.AddParameter("Id", newRecord.Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override ProfileRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            return ProfileRecord.Create(Context, this, id);
        }
    }
}