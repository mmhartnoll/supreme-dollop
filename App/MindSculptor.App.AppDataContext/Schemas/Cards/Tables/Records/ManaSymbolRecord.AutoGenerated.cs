using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class ManaSymbolRecord : DatabaseRecord<ManaSymbolRecord>
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

        private ManaSymbolRecord(DatabaseContext dataContext, ManaSymbolsTable manaSymbolsTable, Guid id, string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity) : base(dataContext, manaSymbolsTable)
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

        internal static ManaSymbolRecord Create(DatabaseContext dataContext, ManaSymbolsTable manaSymbolsTable, Guid id, string type, string code, int convertedManaCost, bool hasWhiteIdentity, bool hasBlueIdentity, bool hasBlackIdentity, bool hasRedIdentity, bool hasGreenIdentity, bool hasColorlessIdentity)
        {
            return new ManaSymbolRecord(dataContext, manaSymbolsTable, id, type, code, convertedManaCost, hasWhiteIdentity, hasBlueIdentity, hasBlackIdentity, hasRedIdentity, hasGreenIdentity, hasColorlessIdentity);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
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
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
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
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[ManaSymbols] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[ManaSymbols] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}