using System.Threading.Tasks;

namespace MindSculptor.DataAccess.DataContext
{
    public abstract class DataContextRecord
    {
        public DataContext DataContext { get; }
        public bool IsModified { get; protected set; }

        public DataContextRecord(DataContext dataContext)
            => DataContext = dataContext;

        public abstract void UpdateRecord();
        public abstract Task UpdateRecordAsync();

        public abstract void DeleteRecord();
        public abstract Task DeleteRecordAsync();
    }
}
