using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using MindSculptor.Tools.CodeGeneration.Declarations;
using MindSculptor.Tools.CodeGeneration.Directives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration
{
    public abstract class GeneratedCodeFile
    {
        private readonly List<UsingDirective> usingDirectives = new List<UsingDirective>();
        private readonly List<NamespaceDeclaration> namespaceDeclarations = new List<NamespaceDeclaration>();

        public string FolderPath { get; }
        public string FileName { get; }
        public string FilePath => Path.Combine(FolderPath, FileName);

        protected GeneratedCodeFile(string folderPath, string fileName)
        {
            FolderPath = folderPath;
            FileName = fileName;
        }

        protected void AddUsingDirective(UsingDirective usingDirective)
            => usingDirectives.Add(usingDirective);

        protected void AddNamespace(NamespaceDeclaration namespaceDeclaration)
            => namespaceDeclarations.Add(namespaceDeclaration);

        protected CompilationUnitSyntax GetCompilationUnitSyntax()
        {
            var memberDeclarationSyntaxes = namespaceDeclarations
                .Select<NamespaceDeclaration, MemberDeclarationSyntax>(x => x)
                .ToArray();
            var usingDirectiveSyntaxes = usingDirectives
                .OrderBy(usingDirective => usingDirective.Namespace)
                .Select<UsingDirective, UsingDirectiveSyntax>(x => x)
                .ToArray();
            return SyntaxFactory.CompilationUnit()
                .AddUsings(usingDirectiveSyntaxes)
                .AddMembers(memberDeclarationSyntaxes);
        }

        public void CreateOrUpdateFile()
        {
            var workspace = new AdhocWorkspace();
            var options = workspace.Options;

            var newFileText = Formatter.Format(GetCompilationUnitSyntax(), workspace, options)
                .ToFullString();

            if (File.Exists(FilePath))
            {
                var existingFileText = File.ReadAllText(FilePath);
                if (newFileText != existingFileText)
                {
                    Console.WriteLine($"Updating file: {FilePath}");
                    File.WriteAllText(FilePath, newFileText);
                }
            }
            else
            {
                Console.WriteLine($"Creating file: {FilePath}");
                if (!Directory.Exists(FolderPath)) 
                    Directory.CreateDirectory(FolderPath);
                File.WriteAllText(FilePath, newFileText);
            }
        }
    }
}
