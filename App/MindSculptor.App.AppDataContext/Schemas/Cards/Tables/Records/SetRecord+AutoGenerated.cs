using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SetRecord : DataContextRecord
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Code { get; }
        public string? CodeExtension { get; }
        public int ReleaseYear { get; }
        public int ReleaseMonth { get; }
        public int? ReleaseDay { get; }

        private SetRecord(DataContext dataContext, Guid id, string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay) : base(dataContext)
        {
            Id = id;
            Name = name;
            Code = code;
            CodeExtension = codeExtension;
            ReleaseYear = releaseYear;
            ReleaseMonth = releaseMonth;
            ReleaseDay = releaseDay;
        }

        internal static SetRecord Create(DataContext dataContext, Guid id, string name, string code, string? codeExtension, int releaseYear, int releaseMonth, int? releaseDay)
        {
            return new SetRecord(dataContext, id, name, code, codeExtension, releaseYear, releaseMonth, releaseDay);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[Sets] SET Name = @Name, Code = @Code, CodeExtension = @CodeExtension, ReleaseYear = @ReleaseYear, ReleaseMonth = @ReleaseMonth, ReleaseDay = @ReleaseDay WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Name", Name);
                    command.AddParameter("Code", Code);
                    command.AddParameter("CodeExtension", CodeExtension);
                    command.AddParameter("ReleaseYear", ReleaseYear);
                    command.AddParameter("ReleaseMonth", ReleaseMonth);
                    command.AddParameter("ReleaseDay", ReleaseDay);
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
                    command.CommandText = "UPDATE [Cards].[Sets] SET Name = @Name, Code = @Code, CodeExtension = @CodeExtension, ReleaseYear = @ReleaseYear, ReleaseMonth = @ReleaseMonth, ReleaseDay = @ReleaseDay WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Name", Name);
                    command.AddParameter("Code", Code);
                    command.AddParameter("CodeExtension", CodeExtension);
                    command.AddParameter("ReleaseYear", ReleaseYear);
                    command.AddParameter("ReleaseMonth", ReleaseMonth);
                    command.AddParameter("ReleaseDay", ReleaseDay);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[Sets] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Cards].[Sets] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}