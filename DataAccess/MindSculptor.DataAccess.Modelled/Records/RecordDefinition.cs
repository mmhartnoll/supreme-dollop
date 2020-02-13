using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MindSculptor.DataAccess.Modelled.Records
{
    public abstract class RecordDefinition : Container
    {
        private readonly Lazy<IEnumerable<Field>> fieldLoader;
        private readonly Lazy<PrimaryKey> primaryKeyLoader;
        private readonly Lazy<IEnumerable<UniqueKey>> uniqueKeyLoader;
        private readonly Lazy<IEnumerable<ForeignKey>> foreignKeyLoader;

        public abstract Schema Schema { get; }
        public string TableName => GetType().Name;
        public virtual string SingularName => FormatDefaultSingularName(TableName);
        public string RecordName => $"{SingularName}Record";

        public IEnumerable<Field> Fields => fieldLoader.Value;
        public PrimaryKey PrimaryKey => primaryKeyLoader.Value;
        public IEnumerable<UniqueKey> UniqueKeys => uniqueKeyLoader.Value;
        public IEnumerable<ForeignKey> ForeignKeys => foreignKeyLoader.Value;

        protected RecordDefinition()
        {
            fieldLoader = new Lazy<IEnumerable<Field>>(LoadFields);
            primaryKeyLoader = new Lazy<PrimaryKey>(LoadPrimaryKey);
            uniqueKeyLoader = new Lazy<IEnumerable<UniqueKey>>(LoadUniqueKeys);
            foreignKeyLoader = new Lazy<IEnumerable<ForeignKey>>(LoadForeignKeys);
        }

        private IEnumerable<Field> LoadFields() => GetType()
            .GetTypeInfo()
            .GetFields()
            .Where(fieldInfo => fieldInfo.FieldType.IsSubclassOf(typeof(Field)))
            .Select(fieldInfo => fieldInfo.GetValue(this) as Field ?? throw new InvalidCastException($"Failed to load field '{fieldInfo.Name}' for record definition '{TableName}'."))
            .ToList()
            .Enumerate();

        private PrimaryKey LoadPrimaryKey() => GetType()
            .GetTypeInfo()
            .GetFields()
            .Single(fieldInfo => fieldInfo.FieldType == typeof(PrimaryKey))
            .GetValue(this) as PrimaryKey ?? throw new InvalidCastException($"Failed to load primary key for record definition '{TableName}'.");

        private IEnumerable<UniqueKey> LoadUniqueKeys() => GetType()
            .GetTypeInfo()
            .GetFields()
            .Where(fieldInfo => fieldInfo.FieldType == typeof(UniqueKey))
            .Select(fieldInfo => fieldInfo.GetValue(this) as UniqueKey ?? throw new InvalidCastException($"Failed to load unique key '{fieldInfo.Name}' for record definition '{TableName}'."))
            .ToList()
            .Enumerate();

        private IEnumerable<ForeignKey> LoadForeignKeys() => GetType()
            .GetTypeInfo()
            .GetFields()
            .Where(fieldInfo => fieldInfo.FieldType == typeof(ForeignKey))
            .Select(fieldInfo => fieldInfo.GetValue(this) as ForeignKey ?? throw new InvalidCastException($"Failed to load foreign key '{fieldInfo.Name}' for record definition '{TableName}'."))
            .ToList()
            .Enumerate();

        private static string FormatDefaultSingularName(string tableName) => tableName.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) ?
            tableName.Substring(0, tableName.Length - 1) : 
            tableName;
    }
}
