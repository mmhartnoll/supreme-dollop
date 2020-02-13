namespace MindSculptor.Tools.Applications.Harvester
{
    internal class Progress
    {
        public string Title { get; }
        public int Current { get; }
        public int Total { get; }
        public string Detail { get; }

        private Progress(string title, int current, int total, string? detail = null)
        {
            Title = title;
            Current = current;
            Total = total;
            Detail = detail ?? string.Empty;
        }

        public static Progress Create(string title, int current, int total, string? detail = null)
            => new Progress(title, current, total, detail);
    }
}
