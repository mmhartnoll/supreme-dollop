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
    public class ManaSymbolsTable : DatabaseTable<ManaSymbolRecord, ManaSymbolRecordExpression>
    {
        private ManaSymbolsTable(DatabaseContext dataContext) : base(dataContext, "Cards", "ManaSymbols")
        {
        }

        internal static ManaSymbolsTable Create(DatabaseContext dataContext)
        {
            return new ManaSymbolsTable(dataContext);
        }

        public ManaSymbolRecord NewRecord(string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity));
        }

        public async Task<ManaSymbolRecord> NewRecordAsync(string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ManaSymbolRecord NewRecord(DbCommand command, Guid id, string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity)
        {
            var newRecord = ManaSymbolRecord.Create(Context, this, id, type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
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
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ManaSymbolRecord> NewRecordAsync(DbCommand command, Guid id, string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity, CancellationToken cancellationToken)
        {
            var newRecord = ManaSymbolRecord.Create(Context, this, id, type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
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
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
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
            return ManaSymbolRecord.Create(Context, this, id, type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
        }
    }
}