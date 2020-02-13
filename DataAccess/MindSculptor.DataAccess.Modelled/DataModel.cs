using MindSculptor.DataAccess.Modelled.Schemas;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MindSculptor.DataAccess.Modelled
{
    public abstract class DataModel : Container
    {
        private readonly Lazy<IEnumerable<Schema>> schemaLoader;

        public IEnumerable<Schema> Schemata => schemaLoader.Value;

        public DataModel()
        {
            schemaLoader = new Lazy<IEnumerable<Schema>>(LoadSchemas);
        }


        private IEnumerable<Schema> LoadSchemas() => GetType()
            .GetTypeInfo()
            .GetFields()
            .Where(fieldInfo => fieldInfo.FieldType == typeof(Schema) || fieldInfo.FieldType.IsSubclassOf(typeof(Schema)))
            .Select(fieldInfo => fieldInfo.GetValue(this) as Schema ?? throw new InvalidCastException($"Failed to load schema '{fieldInfo.Name}' for data model '{GetType().Name}'."))
            .ToList()
            .Enumerate();
    }
}
