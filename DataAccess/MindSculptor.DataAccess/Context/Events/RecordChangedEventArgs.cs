namespace MindSculptor.DataAccess.Context.Events
{
    public class RecordChangedEventArgs<TRecord>
        where TRecord : DatabaseRecord
    {
        public RecordChangedAction Action { get; }
        public TRecord ChangedRecord { get; }

        private RecordChangedEventArgs(RecordChangedAction action, TRecord changedRecord)
        {
            Action = action;
            ChangedRecord = changedRecord;
        }

        public static RecordChangedEventArgs<TRecord> Create(RecordChangedAction action, TRecord changedRecord)
            => new RecordChangedEventArgs<TRecord>(action, changedRecord);
    }
}
