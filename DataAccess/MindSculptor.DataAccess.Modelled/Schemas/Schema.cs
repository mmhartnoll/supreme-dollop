using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.DataAccess.Modelled.Schemas
{
    public class Schema : Component
    {
        private readonly Lazy<IEnumerable<RecordDefinition>> recordLoader;

        public IEnumerable<RecordDefinition> Records => recordLoader.Value;

        protected Schema(Definition definition)
            : base(definition)
        {
            recordLoader = new Lazy<IEnumerable<RecordDefinition>>(LoadRecords);
        }

        private IEnumerable<RecordDefinition> LoadRecords() => DefiningType.Assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(RecordDefinition)))
            .Select(type => Activator.CreateInstance(type) as RecordDefinition ?? throw new InvalidCastException($"Failed to load record definition '{type.Name}'."))
            .Where(recordDefinition => Equals(recordDefinition.Schema))
            .ToList()
            .Enumerate();

        public static implicit operator Schema(Definition definition)
            => new Schema(definition);

        public new class Definition : Component.Definition
        {

        }
    }
}
