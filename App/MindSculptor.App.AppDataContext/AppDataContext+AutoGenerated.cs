using MindSculptor.App.AppDataContext.Schemas;
using MindSculptor.DataAccess.DataContext;
using System;
using System.Data.Common;

namespace MindSculptor.App.AppDataContext
{
    public class AppDataContext : DataContext
    {
        private Lazy<AccountsSchema> accountsSchemaLoader;
        private Lazy<CardsSchema> cardsSchemaLoader;
        private Lazy<MtgaSchema> mtgaSchemaLoader;

        public AccountsSchema Accounts => accountsSchemaLoader.Value;

        public CardsSchema Cards => cardsSchemaLoader.Value;

        public MtgaSchema Mtga => mtgaSchemaLoader.Value;

        private AppDataContext(DbConnection dbConnection, DbTransaction? dbTransaction) : base(dbConnection, dbTransaction)
        {
            accountsSchemaLoader = new Lazy<AccountsSchema>(() => AccountsSchema.Create(this));
            cardsSchemaLoader = new Lazy<CardsSchema>(() => CardsSchema.Create(this));
            mtgaSchemaLoader = new Lazy<MtgaSchema>(() => MtgaSchema.Create(this));
        }

        public static AppDataContext Create(DbConnection dbConnection)
        {
            return new AppDataContext(dbConnection, null);
        }

        public static AppDataContext Create(DbConnection dbConnection, DbTransaction dbTransaction)
        {
            return new AppDataContext(dbConnection, dbTransaction);
        }
    }
}