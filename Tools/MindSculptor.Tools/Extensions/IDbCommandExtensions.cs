using System;
using System.Data;

namespace MindSculptor.Tools.Extensions
{
    public static class IDbCommandExtensions
    {
        public static void AddParameter(this IDbCommand command, string parameterName, object? value)
        {
            var newParameter = command.CreateParameter();
            newParameter.ParameterName = parameterName;
            newParameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(newParameter);
        }
    }
}
