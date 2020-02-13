using MindSculptor.DataAccess.DataContext.Query.Expressions;

namespace MindSculptor.DataAccess.DataContext
{
    public abstract class DataContextSchemaEntity : IDataContextQueryable
    {
        public string SchemaName { get; }
        public string Name { get; }

        public string IdentifierString => $"[{SchemaName}].[{Name}]";

        protected DataContextSchemaEntity(string schemaName, string name)
        {
            SchemaName = schemaName;
            Name = name;
        }
    }
}
