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
    public class ArtistsTable : DatabaseTable<ArtistRecord, ArtistRecordExpression>
    {
        private ArtistsTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "Artists")
        {
        }

        internal static ArtistsTable Create(DatabaseContext databaseContext)
        {
            return new ArtistsTable(databaseContext);
        }

        public ArtistRecord NewRecord(string name)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), name));
        }

        public async Task<ArtistRecord> NewRecordAsync(string name, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), name, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ArtistRecord NewRecord(DbCommand command, Guid id, string name)
        {
            var newRecord = ArtistRecord.Create(DatabaseContext, this, id, name);
            command.CommandText = "INSERT INTO [Cards].[Artists] ( Id, Name ) VALUES ( @Id, @Name );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Name", newRecord.Name);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ArtistRecord> NewRecordAsync(DbCommand command, Guid id, string name, CancellationToken cancellationToken)
        {
            var newRecord = ArtistRecord.Create(DatabaseContext, this, id, name);
            command.CommandText = "INSERT INTO [Cards].[Artists] ( Id, Name ) VALUES ( @Id, @Name );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Name", newRecord.Name);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override ArtistRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var name = (string)dbDataReader["Name"];
            return ArtistRecord.Create(DatabaseContext, this, id, name);
        }
    }
}