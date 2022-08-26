using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseRecord
    {
        public DatabaseContext Context { get; }
        public bool IsModified { get; protected set; }

        public DatabaseRecord(DatabaseContext context)
            => Context = context;

        public void UpdateRecord() => Context.Execute(UpdateRecord);
        public async Task UpdateRecordAsync(CancellationToken cancellationToken = default) 
            => await Context.ExecuteAsync(UpdateRecordAsync, cancellationToken).ConfigureAwait(false);

        public void DeleteRecord() => Context.Execute(DeleteRecord);
        public async Task DeleteRecordAsync(CancellationToken cancellationToken = default)
            => await Context.ExecuteAsync(DeleteRecordAsync, cancellationToken).ConfigureAwait(false);

        protected abstract void UpdateRecord(DbCommand command);
        protected abstract Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken);

        protected abstract void DeleteRecord(DbCommand command);
        protected abstract Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken);
    }
}
