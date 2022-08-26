using MindSculptor.DataAccess.Context.Events;

namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseTable<TRecord> : DatabaseSchemaEntity
        where TRecord : DatabaseRecord
    {
        public event RecordChangedEventHandler<TRecord>? RecordChanged;

        protected DatabaseTable(string schemaName, string name)
            : base(schemaName, name) { }

        protected internal void OnRecordChanged(RecordChangedAction action, TRecord record)
            => RecordChanged?.Invoke(RecordChangedEventArgs<TRecord>.Create(action, record)); 
    }
}
