using System;
using System.Collections.Generic;

namespace Mindsculptor.Tools.Applications.MtgaLinker.Processing
{
    internal class EnumsLookup
    {
        private static readonly Lazy<IDictionary<string, IDictionary<int, string>>> enumsLookupLoader;

        static EnumsLookup()
            => enumsLookupLoader = new Lazy<IDictionary<string, IDictionary<int, string>>>(LoadEnumsLookup);

        public static string GetValue(string enumName, int id)
            => enumsLookupLoader.Value[enumName][id];

        public static bool TryGetValue(string enumName, int id, out string value)
        {
            if (enumsLookupLoader.Value.TryGetValue(enumName, out var enumLookup))
                return enumLookup.TryGetValue(id, out value!);
            value = default!;
            return false;
        }

        private static IDictionary<string, IDictionary<int, string>> LoadEnumsLookup()
        {
            var enumsLookup = new Dictionary<string, IDictionary<int, string>>();
            var enumsDocument = JsonDocumentLoader.LoadJsonDocument(EnumsFileRegexPattern);
            foreach (var enumElement in enumsDocument.RootElement.EnumerateArray())
            {
                var valueLookup = new Dictionary<int, string>();
                var enumName = enumElement.GetProperty("name").GetString();
                var valueElements = enumElement.GetProperty("values");
                foreach (var valueElement in valueElements.EnumerateArray())
                {
                    var id = valueElement.GetProperty("id").GetInt32();
                    var textId = valueElement.GetProperty("text").GetInt32();
                    var text = LocaleStringsLookup.GetValue(textId);
                    valueLookup.Add(id, text);
                }
                enumsLookup.Add(enumName, valueLookup);
            }
            return enumsLookup;
        }

        private const string EnumsFileRegexPattern = @"^data_enums_[\da-f]{32}.mtga$";
    }
}
