using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class ArtistRecord : DatabaseRecord<ArtistRecord>
    {
        public Guid Id { get; }
        public string Name { get; }

        private ArtistRecord(DatabaseContext dataContext, ArtistsTable artistsTable, Guid id, string name) : base(dataContext, artistsTable)
        {
            Id = id;
            Name = name;
        }

        internal static ArtistRecord Create(DatabaseContext dataContext, ArtistsTable artistsTable, Guid id, string name)
        {
            return new ArtistRecord(dataContext, artistsTable, id, name);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Artists] SET Name = @Name WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Name", Name);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Artists] SET Name = @Name WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Name", Name);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[Artists] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[Artists] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}