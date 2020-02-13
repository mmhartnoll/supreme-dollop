using System;

namespace MindSculptor.Tools
{
    public class VerifiedResult<T>
    {
        private readonly T value;

        public bool Success { get; }
        public T Value => Success ? value : throw new InvalidOperationException("Value is undefined. Please check the value of 'Success' before checking this property.");

        private VerifiedResult(bool success, T value)
        {
            Success = success;
            this.value = value;
        }

        public static VerifiedResult<T> Successful(T value)
            => new VerifiedResult<T>(true, value);

        public static VerifiedResult<T> Failure
            => new VerifiedResult<T>(false, default!);
    }
}
