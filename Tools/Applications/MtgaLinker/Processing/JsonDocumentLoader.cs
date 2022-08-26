using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mindsculptor.Tools.Applications.MtgaLinker.Processing
{
    internal static class JsonDocumentLoader
    {
        public static JsonDocument LoadJsonDocument(string regexPattern)
        {
            var potentialFilePaths = new List<string>();
            foreach (var filePath in Directory.GetFiles(DataFileDirectory))
            {
                var filename = Path.GetFileName(filePath);
                var match = Regex.Match(filename, regexPattern);
                if (match.Success)
                    potentialFilePaths.Add(filePath);
            }
            if (!potentialFilePaths.Any())
                throw new Exception();

            var latestFilePath = potentialFilePaths.OrderByDescending(filename => File.GetCreationTimeUtc(filename)).First();
            var fileData = File.ReadAllText(latestFilePath);
            return JsonDocument.Parse(fileData);
        }

        private const string DataFileDirectory = @"C:\Program Files (x86)\Wizards of the Coast\MTGA\MTGA_Data\Downloads\Data";
    }
}
