using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SetRecord : DatabaseRecord<SetRecord>
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Code { get; }
        public string? CodeExtension { get; }
        public int ReleaseYear { get; }
        public int ReleaseMonth { get; }
        public int? ReleaseDay { get; }

        private SetRecord(DatabaseContext dataContext, SetsTable setsTable, Guid id, string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay) : base(dataContext, setsTable)
        {
            Id = id;
            Name = name;
            Code = code;
            CodeExtension = codeExtension;
            ReleaseYear = releaseYear;
            ReleaseMonth = releaseMonth;
            ReleaseDay = releaseDay;
        }

        internal static SetRecord Create(DatabaseContext dataContext, SetsTable setsTable, Guid id, string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay)
        {
            return new SetRecord(dataContext, setsTable, id, name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Sets] SET Name = @Name, Code = @Code, CodeExtension = @CodeExtension, ReleaseYear = @ReleaseYear, ReleaseMonth = @ReleaseMonth, ReleaseDay = @ReleaseDay WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Name", Name);
                command.AddParameter("Code", Code);
                command.AddParameter("CodeExtension", CodeExtension);
                command.AddParameter("ReleaseYear", ReleaseYear);
                command.AddParameter("ReleaseMonth", ReleaseMonth);
                command.AddParameter("ReleaseDay", ReleaseDay);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Sets] SET Name = @Name, Code = @Code, CodeExtension = @CodeExtension, ReleaseYear = @ReleaseYear, ReleaseMonth = @ReleaseMonth, ReleaseDay = @ReleaseDay WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Name", Name);
                command.AddParameter("Code", Code);
                command.AddParameter("CodeExtension", CodeExtension);
                command.AddParameter("ReleaseYear", ReleaseYear);
                command.AddParameter("ReleaseMonth", ReleaseMonth);
                command.AddParameter("ReleaseDay", ReleaseDay);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[Sets] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[Sets] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}