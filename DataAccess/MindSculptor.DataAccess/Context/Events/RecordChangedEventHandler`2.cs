namespace MindSculptor.DataAccess.Context.Events
{
    public delegate void RecordChangedEventHandler<TRecord>(RecordChangedEventArgs<TRecord> eventArgs)
        where TRecord : DatabaseRecord;
}
