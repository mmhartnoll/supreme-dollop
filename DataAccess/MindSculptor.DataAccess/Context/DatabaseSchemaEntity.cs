using MindSculptor.DataAccess.Context.Query.Expressions;

namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseSchemaEntity : IDatabaseContextQueryable
    {
        public string SchemaName { get; }
        public string Name { get; }

        public string IdentifierString => $"[{SchemaName}].[{Name}]";

        protected DatabaseSchemaEntity(string schemaName, string name)
        {
            SchemaName = schemaName;
            Name = name;
        }
    }
}
