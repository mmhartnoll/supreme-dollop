using System;
using System.Collections.Generic;

namespace Mindsculptor.Tools.Applications.MtgaLinker.Processing
{
    internal static class LocaleStringsLookup
    {
        private static readonly Lazy<IDictionary<int, string>> localeStringsLookupLoader;

        static LocaleStringsLookup()
            => localeStringsLookupLoader = new Lazy<IDictionary<int, string>>(LoadLocaleStringsLookup);

        public static string GetValue(int id) 
            => localeStringsLookupLoader.Value[id];

        public static bool TryGetValue(int id, out string value) 
            => localeStringsLookupLoader.Value.TryGetValue(id, out value!);

        private static IDictionary<int, string> LoadLocaleStringsLookup()
        {
            var localeStringsLookup = new Dictionary<int, string>();
            var localeStringsDocument = JsonDocumentLoader.LoadJsonDocument(LocaleStringsFileRegexPattern);
            foreach (var localeElement in localeStringsDocument.RootElement.EnumerateArray())
            {
                var isoCode = localeElement.GetProperty("isoCode").GetString();
                if (isoCode == "en-US")
                    foreach (var keyElement in localeElement.GetProperty("keys").EnumerateArray())
                    {
                        var id = keyElement.GetProperty("id").GetInt32();
                        var text = keyElement.GetProperty("text").GetString();
                        localeStringsLookup.Add(id, text);
                    }
            }
            return localeStringsLookup;
        }

        private const string LocaleStringsFileRegexPattern = @"^data_loc_[\da-f]{32}.mtga$";
    }
}
