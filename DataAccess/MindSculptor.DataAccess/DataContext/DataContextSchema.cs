namespace MindSculptor.DataAccess.DataContext
{
    public abstract class DataContextSchema
    {
        protected DataContext DataContext { get; }

        protected DataContextSchema(DataContext dataContext)
            => DataContext = dataContext;
    }
}
