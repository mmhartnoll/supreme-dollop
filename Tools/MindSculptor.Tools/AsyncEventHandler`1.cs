using System.Threading.Tasks;

namespace MindSculptor.Tools
{
    public delegate Task AsyncEventHandler<TEventArgs>(NullableReference<object> sender, TEventArgs e);

    public static partial class AsyncEventHandlerExtensions
    {
        public static async Task InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> eventHandler, NullableReference<object> sender, TEventArgs e)
        {
            foreach (AsyncEventHandler<TEventArgs> invocation in eventHandler.GetInvocationList())
                await invocation.Invoke(sender, e).ConfigureAwait(false);
        }
    }
}
