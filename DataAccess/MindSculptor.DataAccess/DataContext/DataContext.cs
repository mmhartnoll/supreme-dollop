using System;
using System.Data.Common;

namespace MindSculptor.DataAccess.DataContext
{
    public abstract class DataContext
    {
        private readonly DbTransaction? transaction = null;

        public DbConnection Connection { get; }

        public bool HasTransaction => transaction != null;
        public DbTransaction Transaction => transaction ?? 
            throw new InvalidOperationException($"Property '{nameof(Transaction)}' has not been set. Please check the value of '{nameof(HasTransaction)}' before accessing this property.");

        protected DataContext(DbConnection connection)
            => Connection = connection;

        protected DataContext(DbConnection connection, DbTransaction? transaction)
        {
            Connection = connection;
            this.transaction = transaction;
        }
    }
}
