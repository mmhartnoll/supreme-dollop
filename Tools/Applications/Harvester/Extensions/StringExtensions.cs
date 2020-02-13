using System.Text.RegularExpressions;

namespace MindSculptor.Tools.Applications.Harvester.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveLineBreaks(this string input)
            => input.Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

        public static string FormatManaSymbols(this string input)
        {
            var output = Regex.Replace(input, "<img .*?name=(.*?)&amp;type=symbol.*?>", "{$1}");
            output = Regex.Replace(output, "{tap}", "{T}");
            output = Regex.Replace(output, "{snow}", "{S}");
            return output;
        }

        public static string ConvertTextBlocksToNewLines(this string input)
            => Regex.Replace(input, "<div.*?>(.*?)</div>", "$1\n");

        public static string RemoveItalicsTags(this string input)
            => Regex.Replace(input, "<i>(.*?)</i>", "$1");
    }
}
