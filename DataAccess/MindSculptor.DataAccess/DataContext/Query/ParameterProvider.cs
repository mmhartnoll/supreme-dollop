using MindSculptor.Tools.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.DataAccess.DataContext.Query
{
    public class ParameterProvider
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        private int parameterIndex;

        public IEnumerable<KeyValuePair<string, object>> Parameters => parameters.Enumerate();

        private ParameterProvider() { }

        public static ParameterProvider Create()
            => new ParameterProvider();

        public string CreateParameter(object value)
        {
            try
            {
                semaphore.Wait();
                var parameterName = $"P{parameterIndex++}";
                parameters.Add(parameterName, value);
                return parameterName;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<string> CreateParameterAsync(object value)
        {
            try
            {
                await semaphore.WaitAsync()
                    .ConfigureAwait(false);
                var parameterName = $"P{parameterIndex++}";
                parameters.Add(parameterName, value);
                return parameterName;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
