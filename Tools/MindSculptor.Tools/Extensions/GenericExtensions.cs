using System;
using System.Linq;

namespace MindSculptor.Tools.Extensions
{
    public static class GenericExtensions
    {
        public static bool In<T>(this T source, params T[] values)
            => values.Contains(source);
    }
}
