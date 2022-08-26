namespace MindSculptor.Tools.Data
{
    public class FormattedInteger
    {
        public int Value { get; }
        public string Format { get; }

        private FormattedInteger(int value, string format)
        {
            Value = value;
            Format = format;
        }

        public static FormattedInteger CreateFormatted(int value, string format)
            => new FormattedInteger(value, format);

        public static FormattedInteger CreateUnformatted(int value)
            => new FormattedInteger(value, "$");
    }
}
