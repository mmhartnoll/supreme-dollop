using System;
using System.Threading.Tasks;

namespace MindSculptor.Tools
{
    public delegate Task AsyncEventHandler(NullableReference<object> sender, EventArgs eventArgs);

    public static partial class AsyncEventHandlerExtensions
    {
        public static async Task InvokeAsync(this AsyncEventHandler eventHandler, NullableReference<object> sender, EventArgs eventArgs)
        {
            foreach (AsyncEventHandler invocation in eventHandler.GetInvocationList())
                await invocation.Invoke(sender, eventArgs).ConfigureAwait(false);
        }
    }
}
