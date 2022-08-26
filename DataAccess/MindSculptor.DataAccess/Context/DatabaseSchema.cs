namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseSchema
    {
        protected DatabaseContext Context { get; }

        protected DatabaseSchema(DatabaseContext context)
            => Context = context;
    }
}
