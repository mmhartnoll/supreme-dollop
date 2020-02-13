using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.Applications.SchemaTool.Extensions
{
    public static class RecordDefinitionExtensions
    {
        public static IEnumerable<RecordDefinition> OrderByDependencies(this IEnumerable<RecordDefinition> source)
        {
            var sortedRecordDefinitions = new List<RecordDefinition>();
            var unsortedRecordDefinitions = source.ToList();

            while (unsortedRecordDefinitions.Any())
            {
                var nextNonDependentRecordDefinition = GetNextNonDependentRecordDefinition(unsortedRecordDefinitions);
                unsortedRecordDefinitions.Remove(nextNonDependentRecordDefinition);
                sortedRecordDefinitions.Add(nextNonDependentRecordDefinition);
            }

            return sortedRecordDefinitions.Enumerate();

            static RecordDefinition GetNextNonDependentRecordDefinition(IEnumerable<RecordDefinition> source)
                => source.FirstOrDefault(recordDefinition => !recordDefinition.DependsOn(source)) ?? 
                    throw new Exception("Cyclical foreign key relationship detected.");
        }

        public static bool DependsOn(this RecordDefinition source, RecordDefinition recordDefinition)
            => source.ForeignKeys.Any(foreignKeyDefinition => foreignKeyDefinition.ReferencedKey.RecordDefinition.TableName == recordDefinition.TableName);

        public static bool DependsOn(this RecordDefinition source, IEnumerable<RecordDefinition> recordDefinitions)
            => recordDefinitions.Any(recordDefinition => source.DependsOn(recordDefinition));
    }
}
