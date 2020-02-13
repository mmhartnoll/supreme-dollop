using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FacesTable : DataContextTable<FaceRecord, FaceRecordExpression>
    {
        private FacesTable(DataContext dataContext) : base(dataContext, "Cards", "Faces")
        {
        }

        internal static FacesTable Create(DataContext dataContext)
        {
            return new FacesTable(dataContext);
        }

        public FaceRecord NewRecord(BaseRecord baseRecord, string name, bool isPrimaryFace)
        {
            var newRecord = FaceRecord.Create(DataContext, Guid.NewGuid(), baseRecord.Id, name, isPrimaryFace);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Faces] ( Id, BaseId, Name, IsPrimaryFace ) VALUES ( @Id, @BaseId, @Name, @IsPrimaryFace );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("BaseId", newRecord.BaseId);
                command.AddParameter("Name", newRecord.Name);
                command.AddParameter("IsPrimaryFace", newRecord.IsPrimaryFace);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FaceRecord> NewRecordAsync(BaseRecord baseRecord, string name, bool isPrimaryFace)
        {
            var newRecord = FaceRecord.Create(DataContext, Guid.NewGuid(), baseRecord.Id, name, isPrimaryFace);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Faces] ( Id, BaseId, Name, IsPrimaryFace ) VALUES ( @Id, @BaseId, @Name, @IsPrimaryFace );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("BaseId", newRecord.BaseId);
                command.AddParameter("Name", newRecord.Name);
                command.AddParameter("IsPrimaryFace", newRecord.IsPrimaryFace);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FaceRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var baseId = (Guid)dbDataReader["BaseId"];
            var name = (string)dbDataReader["Name"];
            var isPrimaryFace = (bool)dbDataReader["IsPrimaryFace"];
            return FaceRecord.Create(DataContext, id, baseId, name, isPrimaryFace);
        }
    }
}