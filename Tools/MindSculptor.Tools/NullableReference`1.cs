using MindSculptor.Tools.Exceptions;

namespace MindSculptor.Tools
{
    public struct NullableReference<T>
        where T : class
    {
        private T value;

        public bool HasValue { get; }
        public T Value => HasValue ? value : throw new PropertyUndefinedException(nameof(Value), nameof(HasValue));

        private NullableReference(T? value)
        {
            HasValue = value != null;
            this.value = (HasValue ? value : default)!;
        }

        public static implicit operator NullableReference<T>(T? value)
            => new NullableReference<T>(value);

        public static NullableReference<T> FromValue(T value)
            => new NullableReference<T>(value);
    }
}
