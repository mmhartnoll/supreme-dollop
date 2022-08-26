using MindSculptor.DataAccess.Context.Events;

namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseRecord<TRecord> : DatabaseRecord
        where TRecord : DatabaseRecord
    {
        protected DatabaseTable<TRecord> Table { get; }

        public DatabaseRecord(DatabaseContext context, DatabaseTable<TRecord> table) : base(context)
            => Table = table;

        protected void OnRecordChanged(RecordChangedAction action, TRecord record)
            => Table.OnRecordChanged(action, record);
    }
}
