namespace MindSculptor.Tools
{
    public static class NullableReference
    {
        public static NullableReference<T> FromValue<T>(T value) where T : class
            => NullableReference<T>.FromValue(value);
    }
}
