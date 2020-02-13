using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid BaseId { get; }
        public string Name { get; }
        public bool IsPrimaryFace { get; }

        private FaceRecord(DataContext dataContext, Guid id, Guid baseId, string name, bool isPrimaryFace) : base(dataContext)
        {
            Id = id;
            BaseId = baseId;
            Name = name;
            IsPrimaryFace = isPrimaryFace;
        }

        internal static FaceRecord Create(DataContext dataContext, Guid id, Guid baseId, string name, bool isPrimaryFace)
        {
            return new FaceRecord(dataContext, id, baseId, name, isPrimaryFace);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[Faces] SET BaseId = @BaseId, Name = @Name, IsPrimaryFace = @IsPrimaryFace WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("BaseId", BaseId);
                    command.AddParameter("Name", Name);
                    command.AddParameter("IsPrimaryFace", IsPrimaryFace);
                    command.ExecuteNonQuery();
                }
        }

        public async override Task UpdateRecordAsync()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[Faces] SET BaseId = @BaseId, Name = @Name, IsPrimaryFace = @IsPrimaryFace WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("BaseId", BaseId);
                    command.AddParameter("Name", Name);
                    command.AddParameter("IsPrimaryFace", IsPrimaryFace);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[Faces] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[Faces] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}