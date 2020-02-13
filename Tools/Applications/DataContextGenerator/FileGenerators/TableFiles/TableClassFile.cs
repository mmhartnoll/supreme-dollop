﻿using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Constructors;
using MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods;
using MindSculptor.Tools.CodeGeneration;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System.Collections.Generic;
using System.IO;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles
{
    internal class TableClassFile : GeneratedCodeFile
    {
        private TableClassFile(RecordDefinition recordDefinition, string rootFolderPath, string rootNamespace) 
            : base(Path.Combine(rootFolderPath, "Schemas", recordDefinition.Schema.Name, "Tables"), $"{recordDefinition.TableName}Table+AutoGenerated.cs")
        {
            var referencedSchemaNames = new HashSet<string>();
            referencedSchemaNames.Add(recordDefinition.Schema.Name);

            foreach (var foreignKeyDefinition in recordDefinition.ForeignKeys)
            {
                var schemaName = foreignKeyDefinition.ReferencedKey.RecordDefinition.Schema.Name;
                if (!referencedSchemaNames.Contains(schemaName))
                    referencedSchemaNames.Add(schemaName);
            }

            foreach (var schemaName in referencedSchemaNames)
                AddUsingDirective($"MindSculptor.App.AppDataContext.Schemas.{schemaName}.Tables.Records");

            AddUsingDirective($"MindSculptor.App.AppDataContext.Schemas.{recordDefinition.Schema.Name}.Tables.Records.Expressions");
            AddUsingDirective("MindSculptor.DataAccess.DataContext");
            AddUsingDirective("MindSculptor.Tools.Extensions");
            AddUsingDirective("System");
            AddUsingDirective("System.Data.Common");
            AddUsingDirective("System.Threading.Tasks");

            AddNamespace(TablesNamespaceDeclaration.Create(recordDefinition, rootNamespace));
        }

        public static TableClassFile Create(RecordDefinition recordDefinition, string rootFolderPath, string rootNamespace)
            => new TableClassFile(recordDefinition, rootFolderPath, rootNamespace);

        private class TablesNamespaceDeclaration : NamespaceDeclaration
        {
            private TablesNamespaceDeclaration(RecordDefinition recordDefinition, string rootNamespace) 
                    : base($"{rootNamespace}.Schemas.{recordDefinition.Schema.Name}.Tables")
                => AddClass(TableClassDeclaration.Create(recordDefinition));

            public static TablesNamespaceDeclaration Create(RecordDefinition recordDefinition, string rootNamespace)
                => new TablesNamespaceDeclaration(recordDefinition, rootNamespace);

            private class TableClassDeclaration : ClassDeclaration
            {
                private TableClassDeclaration(RecordDefinition recordDefinition) 
                    : base($"{recordDefinition.TableName}Table", $"DataContextTable<{recordDefinition.RecordName}, {recordDefinition.RecordName}Expression>", MemberAccessModifiers.Public)
                {
                    AddConstructor(TableConstructorDeclaration.Create(recordDefinition));
                    AddMethod(CreateTableClassMethodDeclaration.Create(recordDefinition));

                    AddMethod(NewRecordMethodDeclaration.Create(recordDefinition));
                    AddMethod(NewRecordAsyncMethodDeclaration.Create(recordDefinition));
                    AddMethod(MapRecordFromDataReaderMethodDeclaration.Create(recordDefinition));
                }

                public static TableClassDeclaration Create(RecordDefinition recordDefinition)
                    => new TableClassDeclaration(recordDefinition);
            }
        }
    }
}
