using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class ManaSymbolsTable : DataContextTable<ManaSymbolRecord, ManaSymbolRecordExpression>
    {
        private ManaSymbolsTable(DataContext dataContext) : base(dataContext, "Cards", "ManaSymbols")
        {
        }

        internal static ManaSymbolsTable Create(DataContext dataContext)
        {
            return new ManaSymbolsTable(dataContext);
        }

        public ManaSymbolRecord NewRecord(string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity)
        {
            var newRecord = ManaSymbolRecord.Create(DataContext, Guid.NewGuid(), type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[ManaSymbols] ( Id, Type, Code, ConvertedManaCost, HasWhiteIdentity, HasBlueIdentity, HasBlackIdentity, HasRedIdentity, HasGreenIdentity, HasColorlessIdentity ) VALUES ( @Id, @Type, @Code, @ConvertedManaCost, @HasWhiteIdentity, @HasBlueIdentity, @HasBlackIdentity, @HasRedIdentity, @HasGreenIdentity, @HasColorlessIdentity );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Type", newRecord.Type);
                command.AddParameter("Code", newRecord.Code);
                command.AddParameter("ConvertedManaCost", newRecord.ConvertedManaCost);
                command.AddParameter("HasWhiteIdentity", newRecord.HasWhiteIdentity);
                command.AddParameter("HasBlueIdentity", newRecord.HasBlueIdentity);
                command.AddParameter("HasBlackIdentity", newRecord.HasBlackIdentity);
                command.AddParameter("HasRedIdentity", newRecord.HasRedIdentity);
                command.AddParameter("HasGreenIdentity", newRecord.HasGreenIdentity);
                command.AddParameter("HasColorlessIdentity", newRecord.HasColorlessIdentity);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<ManaSymbolRecord> NewRecordAsync(string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity)
        {
            var newRecord = ManaSymbolRecord.Create(DataContext, Guid.NewGuid(), type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[ManaSymbols] ( Id, Type, Code, ConvertedManaCost, HasWhiteIdentity, HasBlueIdentity, HasBlackIdentity, HasRedIdentity, HasGreenIdentity, HasColorlessIdentity ) VALUES ( @Id, @Type, @Code, @ConvertedManaCost, @HasWhiteIdentity, @HasBlueIdentity, @HasBlackIdentity, @HasRedIdentity, @HasGreenIdentity, @HasColorlessIdentity );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Type", newRecord.Type);
                command.AddParameter("Code", newRecord.Code);
                command.AddParameter("ConvertedManaCost", newRecord.ConvertedManaCost);
                command.AddParameter("HasWhiteIdentity", newRecord.HasWhiteIdentity);
                command.AddParameter("HasBlueIdentity", newRecord.HasBlueIdentity);
                command.AddParameter("HasBlackIdentity", newRecord.HasBlackIdentity);
                command.AddParameter("HasRedIdentity", newRecord.HasRedIdentity);
                command.AddParameter("HasGreenIdentity", newRecord.HasGreenIdentity);
                command.AddParameter("HasColorlessIdentity", newRecord.HasColorlessIdentity);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override ManaSymbolRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var type = (string)dbDataReader["Type"];
            var code = (string)dbDataReader["Code"];
            var convertedManaCost = Convert.ToInt32(dbDataReader["ConvertedManaCost"]);
            var hasWhiteIdentity = (bool)dbDataReader["HasWhiteIdentity"];
            var hasBlueIdentity = (bool)dbDataReader["HasBlueIdentity"];
            var hasBlackIdentity = (bool)dbDataReader["HasBlackIdentity"];
            var hasRedIdentity = (bool)dbDataReader["HasRedIdentity"];
            var hasGreenIdentity = (bool)dbDataReader["HasGreenIdentity"];
            var hasColorlessIdentity = (bool)dbDataReader["HasColorlessIdentity"];
            return ManaSymbolRecord.Create(DataContext, id, type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
        }
    }
}