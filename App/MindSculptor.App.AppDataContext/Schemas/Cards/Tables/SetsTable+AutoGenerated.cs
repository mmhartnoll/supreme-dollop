using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class SetsTable : DataContextTable<SetRecord, SetRecordExpression>
    {
        private SetsTable(DataContext dataContext) : base(dataContext, "Cards", "Sets")
        {
        }

        internal static SetsTable Create(DataContext dataContext)
        {
            return new SetsTable(dataContext);
        }

        public SetRecord NewRecord(string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay)
        {
            var newRecord = SetRecord.Create(DataContext, Guid.NewGuid(), name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Sets] ( Id, Name, Code, CodeExtension, ReleaseYear, ReleaseMonth, ReleaseDay ) VALUES ( @Id, @Name, @Code, @CodeExtension, @ReleaseYear, @ReleaseMonth, @ReleaseDay );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Name", newRecord.Name);
                command.AddParameter("Code", newRecord.Code);
                command.AddParameter("CodeExtension", newRecord.CodeExtension);
                command.AddParameter("ReleaseYear", newRecord.ReleaseYear);
                command.AddParameter("ReleaseMonth", newRecord.ReleaseMonth);
                command.AddParameter("ReleaseDay", newRecord.ReleaseDay);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<SetRecord> NewRecordAsync(string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay)
        {
            var newRecord = SetRecord.Create(DataContext, Guid.NewGuid(), name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Sets] ( Id, Name, Code, CodeExtension, ReleaseYear, ReleaseMonth, ReleaseDay ) VALUES ( @Id, @Name, @Code, @CodeExtension, @ReleaseYear, @ReleaseMonth, @ReleaseDay );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Name", newRecord.Name);
                command.AddParameter("Code", newRecord.Code);
                command.AddParameter("CodeExtension", newRecord.CodeExtension);
                command.AddParameter("ReleaseYear", newRecord.ReleaseYear);
                command.AddParameter("ReleaseMonth", newRecord.ReleaseMonth);
                command.AddParameter("ReleaseDay", newRecord.ReleaseDay);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override SetRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var name = (string)dbDataReader["Name"];
            var code = (string)dbDataReader["Code"];
            var codeExtension = dbDataReader["CodeExtension"] == DBNull.Value ? null : (string?)dbDataReader["CodeExtension"];
            var releaseYear = Convert.ToInt32(dbDataReader["ReleaseYear"]);
            var releaseMonth = Convert.ToInt32(dbDataReader["ReleaseMonth"]);
            var releaseDay = dbDataReader["ReleaseDay"] == DBNull.Value ? null : (int?)Convert.ToInt32(dbDataReader["ReleaseDay"]);
            return SetRecord.Create(DataContext, id, name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
        }
    }
}