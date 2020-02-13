using MindSculptor.App.AppDataContext;
using MindSculptor.App.AppDataModel;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.Applications.DataExporter.Extensions;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Applications.DataExporter
{
    class Program
    {
        static async Task Main(string[] args)
            => await new Program().RunAsync().ConfigureAwait(false);

        private async Task RunAsync()
        {
            using var filestream = new FileStream(ExportFilePath, FileMode.Create);
            using var streamWriter = new StreamWriter(filestream);
            using var dbConnection = new SqlConnection(DBConnectionString);

            var dataModel = new MindSculptorDataModel();
            var orderedRecordDefinitions = dataModel.Schemata
                .SelectMany(schema => schema.Records)
                .OrderByDependencies();

            try
            {
                await dbConnection.OpenAsync();
                var dataContext = AppDataContext.Create(dbConnection);

                foreach (var recordDefinition in orderedRecordDefinitions)
                {
                    var schema = dataContext.GetType().GetProperty(recordDefinition.Schema.Name)?.GetValue(dataContext)!;
                    var records = schema.GetType().GetProperty(recordDefinition.TableName)?.GetValue(schema) as IAsyncEnumerable<DataContextRecord>;

                    var fieldNames = recordDefinition.Fields
                        .Select(field => field.Name);

                    await using (var enumerator = records!.GetAsyncEnumerator())
                        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                        {
                            await streamWriter.WriteLineAsync($"INSERT INTO [{recordDefinition.Schema.Name}].[{recordDefinition.TableName}] ({string.Join(", ", fieldNames)}) VALUES")
                                .ConfigureAwait(false);
                            await streamWriter.WriteAsync($"\t{GetValuesLine(dataContext, recordDefinition, enumerator.Current)}")
                                .ConfigureAwait(false);

                            for (int i = 1; i < 1000 && await enumerator.MoveNextAsync().ConfigureAwait(false); i++)
                                await streamWriter.WriteAsync($",\n\t{GetValuesLine(dataContext, recordDefinition, enumerator.Current)}")
                                    .ConfigureAwait(false);

                            await streamWriter.WriteLineAsync($";\n").ConfigureAwait(false);
                        }
                }
            }
            finally
            {
                await dbConnection.CloseAsync();
            }

            static string GetValuesLine(AppDataContext dataContext, RecordDefinition recordDefinition, DataContextRecord record)
            {
                var values = recordDefinition.Fields.Select(fieldDefinition =>
                {
                    var value = record.GetType().GetProperty(fieldDefinition.Name)?.GetValue(record);
                    if (fieldDefinition.IsNullable && value == null)
                        return $"null";
                    if (value == null)
                        throw new Exception("Unexpected null value.");
                    switch (fieldDefinition)
                    {
                        case TextField _:
                            return $"'{(value as string)!.Replace("'", "''")}'";
                        case IntegerField _:
                            return value.ToString();
                        case BooleanField _:
                            return (bool)value ? "1" : "0";
                        default:
                            return $"'{value}'";
                    }
                });
                return $"({string.Join(", ", values)})";
            }
        }

        private const string ExportFilePath = @"C:\Users\mmhar\source\repos\supreme-dollop\Database\DataExport.sql";
        private const string DBConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MindSculptorApp002;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}
