using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class ManaSymbolRecord : DataContextRecord
    {
        public Guid Id { get; }
        public string Type { get; }
        public string Code { get; }
        public int ConvertedManaCost { get; }
        public bool HasWhiteIdentity { get; }
        public bool HasBlueIdentity { get; }
        public bool HasBlackIdentity { get; }
        public bool HasRedIdentity { get; }
        public bool HasGreenIdentity { get; }
        public bool HasColorlessIdentity { get; }

        private ManaSymbolRecord(DataContext dataContext, Guid id, string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity) : base(dataContext)
        {
            Id = id;
            Type = type;
            Code = code;
            ConvertedManaCost = convertedManaCost;
            HasWhiteIdentity = hasWhiteIdentity;
            HasBlueIdentity = hasBlueIdentity;
            HasBlackIdentity = hasBlackIdentity;
            HasRedIdentity = hasRedIdentity;
            HasGreenIdentity = hasGreenIdentity;
            HasColorlessIdentity = hasColorlessIdentity;
        }

        internal static ManaSymbolRecord Create(DataContext dataContext, Guid id, string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity)
        {
            return new ManaSymbolRecord(dataContext, id, type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[ManaSymbols] SET Type = @Type, Code = @Code, ConvertedManaCost = @ConvertedManaCost, HasWhiteIdentity = @HasWhiteIdentity, HasBlueIdentity = @HasBlueIdentity, HasBlackIdentity = @HasBlackIdentity, HasRedIdentity = @HasRedIdentity, HasGreenIdentity = @HasGreenIdentity, HasColorlessIdentity = @HasColorlessIdentity WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Type", Type);
                    command.AddParameter("Code", Code);
                    command.AddParameter("ConvertedManaCost", ConvertedManaCost);
                    command.AddParameter("HasWhiteIdentity", HasWhiteIdentity);
                    command.AddParameter("HasBlueIdentity", HasBlueIdentity);
                    command.AddParameter("HasBlackIdentity", HasBlackIdentity);
                    command.AddParameter("HasRedIdentity", HasRedIdentity);
                    command.AddParameter("HasGreenIdentity", HasGreenIdentity);
                    command.AddParameter("HasColorlessIdentity", HasColorlessIdentity);
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
                    command.CommandText = "UPDATE [Cards].[ManaSymbols] SET Type = @Type, Code = @Code, ConvertedManaCost = @ConvertedManaCost, HasWhiteIdentity = @HasWhiteIdentity, HasBlueIdentity = @HasBlueIdentity, HasBlackIdentity = @HasBlackIdentity, HasRedIdentity = @HasRedIdentity, HasGreenIdentity = @HasGreenIdentity, HasColorlessIdentity = @HasColorlessIdentity WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("Type", Type);
                    command.AddParameter("Code", Code);
                    command.AddParameter("ConvertedManaCost", ConvertedManaCost);
                    command.AddParameter("HasWhiteIdentity", HasWhiteIdentity);
                    command.AddParameter("HasBlueIdentity", HasBlueIdentity);
                    command.AddParameter("HasBlackIdentity", HasBlackIdentity);
                    command.AddParameter("HasRedIdentity", HasRedIdentity);
                    command.AddParameter("HasGreenIdentity", HasGreenIdentity);
                    command.AddParameter("HasColorlessIdentity", HasColorlessIdentity);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[ManaSymbols] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Cards].[ManaSymbols] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}