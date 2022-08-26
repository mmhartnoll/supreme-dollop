using MindSculptor.App.AppDataModel;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.Tools.Applications.SchemaTool.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindSculptor.Tools.Applications.SchemaTool
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
            Console.ReadLine();
        }

        private void Run()
        {
            var dataModel = new MindSculptorDataModel();
            var recordDefinitions = dataModel.Schemata
                .SelectMany(schemaDefinition => schemaDefinition.Records)
                .OrderByDependencies();
            foreach (var recordDefinition in recordDefinitions)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"CREATE TABLE [{recordDefinition.Schema.Name}].[{recordDefinition.TableName}] (");

                var fieldLines = recordDefinition.Fields
                    .Select(fieldDefinition => new FieldLine(fieldDefinition));
                var nameSectionPadding = fieldLines
                    .Select(fieldLine => fieldLine.NameSection.Length)
                    .Max();
                var typeSectionPadding = fieldLines
                    .Select(fieldLine => fieldLine.TypeSection.Length)
                    .Max();
                foreach (var fieldLine in fieldLines)
                {
                    var formattedFieldLine = fieldLine.GetFormattedString(nameSectionPadding, typeSectionPadding);
                    stringBuilder.AppendLine(formattedFieldLine);
                }

                var formattedPrimaryKeyFields = GetFormattedKeyFieldList(recordDefinition.PrimaryKey.Fields);
                stringBuilder.AppendLine($"    PRIMARY KEY CLUSTERED ({formattedPrimaryKeyFields}),");

                foreach (var uniqueKeyDefinition in recordDefinition.UniqueKeys)
                {
                    var formattedUniqueKeyFields = GetFormattedKeyFieldList(uniqueKeyDefinition.Fields);
                    stringBuilder.AppendLine($"    UNIQUE NONCLUSTERED ({formattedUniqueKeyFields}),");
                }

                foreach (var foreignKeyDefinition in recordDefinition.ForeignKeys)
                {
                    var formattedForeignKeyFields = string.Join(", ", foreignKeyDefinition.Fields.Select(
                        fieldDefinition => $"[{fieldDefinition.Field.Name}]"));
                    var formattedReferenceFields = string.Join(", ", foreignKeyDefinition.ReferencedKey.Fields.Select(
                        fieldDefinition => $"[{fieldDefinition.Field.Name}]"));
                    var formattedReferenceIdentifier = $"[{foreignKeyDefinition.ReferencedKey.RecordDefinition.Schema.Name}].[{foreignKeyDefinition.ReferencedKey.RecordDefinition.TableName}]";
                    
                    stringBuilder.AppendLine($"    FOREIGN KEY ({formattedForeignKeyFields}) REFERENCES {formattedReferenceIdentifier} ({formattedReferenceFields}),");
                }

                stringBuilder.Remove(stringBuilder.Length - 3, 1);
                stringBuilder.AppendLine(");");

                Console.WriteLine(stringBuilder.ToString());
            }

            string GetFormattedKeyFieldList(IEnumerable<FieldReference> keyFields)
            {
                return string.Join(
                    ", ",
                    keyFields.Select(fieldDefinition => string.Join(
                        " ",
                        $"{fieldDefinition.Field.Name}",
                        GetIndexSortDirectionString(fieldDefinition.SortDirection))));

                string GetIndexSortDirectionString(IndexSortDirection indexSortDirection) => indexSortDirection switch
                    {
                        IndexSortDirection.Ascending  => "ASC",
                        IndexSortDirection.Descending => "DESC",
                        _ => throw new NotSupportedException($"{nameof(IndexSortDirection)} value '{indexSortDirection}' is not supported.")
                    };
            }
        }

        private class FieldLine
        {
            public string NameSection { get; }
            public string TypeSection { get; }
            public string NullableSection { get; }

            public FieldLine(Field fieldDefinition)
            {
                NameSection = $"[{fieldDefinition.Name}]";
                NullableSection = fieldDefinition.IsNullable ? "NULL" : "NOT NULL";

                switch (fieldDefinition)
                {
                    case IdField idField:
                        TypeSection = "UNIQUEIDENTIFIER";
                        break;
                    case TextField textField:
                        var maxLength = textField.HasMaximumLength ?
                            textField.MaximumLength.ToString() : "MAX";
                        TypeSection = $"NVARCHAR ({maxLength})";
                        break;
                    case IntegerField intField:
                        TypeSection = "INT";
                        break;
                    case DecimalField decimalField:
                        TypeSection = $"DECIMAL({decimalField.Precision}, {decimalField.Scale})";
                        break;
                    case BooleanField booleanField:
                        TypeSection = "BIT";
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            public string GetFormattedString(int nameSectionPadding, int typeSectionPadding)
            {
                var paddedNameSection = NameSection.PadRight(nameSectionPadding);
                var paddedTypeSection = TypeSection.PadRight(typeSectionPadding);
                return $"{string.Join(" ", "   ", paddedNameSection, paddedTypeSection, NullableSection)},";
            }
        }
    }
}
