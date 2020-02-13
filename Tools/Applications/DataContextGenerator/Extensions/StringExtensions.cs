namespace MindSculptor.Tools.Applications.DataContextGenerator.Extensions
{
    public static class StringExtensions
    {
        public static string FormatAsVariableName(this string input)
        {
            if (input == input.ToLowerInvariant() || input == input.ToUpperInvariant())
                return $"{input}";
            return $"{input.Substring(0, 1).ToLowerInvariant()}{input.Substring(1)}";
        }

        public static string FormatAsVariableName(this string input, string prefix)
            => $"_{input.FormatAsVariableName()}";
    }
}
