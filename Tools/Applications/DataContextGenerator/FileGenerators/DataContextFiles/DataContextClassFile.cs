﻿using MindSculptor.DataAccess.DataContext;
using MindSculptor.DataAccess.Modelled;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Constructors;
using MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Methods;
using MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles.Properties;
using MindSculptor.Tools.CodeGeneration;
using MindSculptor.Tools.CodeGeneration.Declarations;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.DataContextFiles
{
    internal class DataContextClassFile : GeneratedCodeFile
    {
        private DataContextClassFile(DataModel dataModel, string rootFolderPath, string rootNamespace) 
            : base(rootFolderPath, "AppDataContext+AutoGenerated.cs")
        {
            AddUsingDirective("MindSculptor.App.AppDataContext.Schemas");
            AddUsingDirective("MindSculptor.DataAccess.DataContext");
            AddUsingDirective("System");
            AddUsingDirective("System.Data.Common");

            AddNamespace(RootNamespaceDeclaration.Create(dataModel, rootNamespace));
        }

        public static DataContextClassFile Create(DataModel dataModel, string rootFolderPath, string rootNamespace)
            => new DataContextClassFile(dataModel, rootFolderPath, rootNamespace);

        private class RootNamespaceDeclaration : NamespaceDeclaration
        {
            private RootNamespaceDeclaration(DataModel dataModel, string rootNamespace)
                    : base(rootNamespace)
                => AddClass(AppDataContextClassDeclaration.Create(dataModel));

            public static RootNamespaceDeclaration Create(DataModel dataModel, string rootNamespace)
                => new RootNamespaceDeclaration(dataModel, rootNamespace);

            private class AppDataContextClassDeclaration : ClassDeclaration
            {
                public AppDataContextClassDeclaration(DataModel dataModel) 
                    : base("AppDataContext", typeof(DataContext), MemberAccessModifiers.Public)
                {
                    foreach (var schemaDefinition in dataModel.Schemata)
                        AddField(FieldDeclaration.Create(
                            $"Lazy<{schemaDefinition.Name}Schema>",
                            $"{schemaDefinition.Name}SchemaLoader".FormatAsVariableName(), 
                            MemberAccessModifiers.Private));

                    foreach (var schemaDefinition in dataModel.Schemata)
                        AddProperty(SchemaPropertyDeclaration.Create(schemaDefinition));

                    AddConstructor(DataContextConstructorDeclaration.Create(dataModel));
                    AddMethod(CreateDataContextMethodDeclaration.Create(dataModel));
                    AddMethod(CreateDataContextWithTransactionMethodDeclaration.Create(dataModel));
                }

                public static AppDataContextClassDeclaration Create(DataModel dataModel)
                    => new AppDataContextClassDeclaration(dataModel);
            }
        }
    }
}
