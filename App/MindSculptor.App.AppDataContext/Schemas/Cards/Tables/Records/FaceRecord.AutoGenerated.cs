using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceRecord : DatabaseRecord<FaceRecord>
    {
        public Guid Id { get; }
        public Guid BaseId { get; }
        public string Name { get; }
        public bool IsPrimaryFace { get; }

        private FaceRecord(DatabaseContext dataContext, FacesTable facesTable, Guid id, Guid baseId, string name, bool isPrimaryFace) : base(dataContext, facesTable)
        {
            Id = id;
            BaseId = baseId;
            Name = name;
            IsPrimaryFace = isPrimaryFace;
        }

        internal static FaceRecord Create(DatabaseContext dataContext, FacesTable facesTable, Guid id, Guid baseId, string name, bool isPrimaryFace)
        {
            return new FaceRecord(dataContext, facesTable, id, baseId, name, isPrimaryFace);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Faces] SET BaseId = @BaseId, Name = @Name, IsPrimaryFace = @IsPrimaryFace WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("BaseId", BaseId);
                command.AddParameter("Name", Name);
                command.AddParameter("IsPrimaryFace", IsPrimaryFace);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Faces] SET BaseId = @BaseId, Name = @Name, IsPrimaryFace = @IsPrimaryFace WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("BaseId", BaseId);
                command.AddParameter("Name", Name);
                command.AddParameter("IsPrimaryFace", IsPrimaryFace);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[Faces] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[Faces] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}