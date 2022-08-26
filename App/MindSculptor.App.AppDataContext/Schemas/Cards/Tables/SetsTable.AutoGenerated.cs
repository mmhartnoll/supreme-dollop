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
    public class SetsTable : DatabaseTable<SetRecord, SetRecordExpression>
    {
        private SetsTable(DatabaseContext dataContext) : base(dataContext, "Cards", "Sets")
        {
        }

        internal static SetsTable Create(DatabaseContext dataContext)
        {
            return new SetsTable(dataContext);
        }

        public SetRecord NewRecord(string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), name, code, codeExtension, releaseYear, releaseMonth, releaseDay));
        }

        public async Task<SetRecord> NewRecordAsync(string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), name, code, codeExtension, releaseYear, releaseMonth, releaseDay, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private SetRecord NewRecord(DbCommand command, Guid id, string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay)
        {
            var newRecord = SetRecord.Create(Context, this, id, name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
            command.CommandText = "INSERT INTO [Cards].[Sets] ( Id, Name, Code, CodeExtension, ReleaseYear, ReleaseMonth, ReleaseDay ) VALUES ( @Id, @Name, @Code, @CodeExtension, @ReleaseYear, @ReleaseMonth, @ReleaseDay );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Name", newRecord.Name);
            command.AddParameter("Code", newRecord.Code);
            command.AddParameter("CodeExtension", newRecord.CodeExtension);
            command.AddParameter("ReleaseYear", newRecord.ReleaseYear);
            command.AddParameter("ReleaseMonth", newRecord.ReleaseMonth);
            command.AddParameter("ReleaseDay", newRecord.ReleaseDay);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<SetRecord> NewRecordAsync(DbCommand command, Guid id, string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay, CancellationToken cancellationToken)
        {
            var newRecord = SetRecord.Create(Context, this, id, name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
            command.CommandText = "INSERT INTO [Cards].[Sets] ( Id, Name, Code, CodeExtension, ReleaseYear, ReleaseMonth, ReleaseDay ) VALUES ( @Id, @Name, @Code, @CodeExtension, @ReleaseYear, @ReleaseMonth, @ReleaseDay );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Name", newRecord.Name);
            command.AddParameter("Code", newRecord.Code);
            command.AddParameter("CodeExtension", newRecord.CodeExtension);
            command.AddParameter("ReleaseYear", newRecord.ReleaseYear);
            command.AddParameter("ReleaseMonth", newRecord.ReleaseMonth);
            command.AddParameter("ReleaseDay", newRecord.ReleaseDay);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
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
            return SetRecord.Create(Context, this, id, name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
        }
    }
}