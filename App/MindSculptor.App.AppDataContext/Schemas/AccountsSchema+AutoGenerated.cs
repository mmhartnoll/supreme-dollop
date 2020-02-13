using MindSculptor.App.AppDataContext.Schemas.Accounts.Tables;
using MindSculptor.DataAccess.DataContext;
using System;

namespace MindSculptor.App.AppDataContext.Schemas
{
    public class AccountsSchema : DataContextSchema
    {
        private Lazy<AccountsTable> accountsTableLoader;

        public AccountsTable Accounts => accountsTableLoader.Value;

        private AccountsSchema(DataContext dataContext) : base(dataContext)
        {
            accountsTableLoader = new Lazy<AccountsTable>(() => AccountsTable.Create(DataContext));
        }

        internal static AccountsSchema Create(DataContext dataContext)
        {
            return new AccountsSchema(dataContext);
        }
    }
}