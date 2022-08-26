using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MindSculptor.Tools
{
    public class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> instance;

        public AsyncLazy(Func<Task<T>> factory)
            => instance = new Lazy<Task<T>>(() => Task.Run(factory));

        public TaskAwaiter<T> GetAwaiter() => instance.Value.GetAwaiter();
    }
}
