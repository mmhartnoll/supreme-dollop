using System;

namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Values
{
    public class ParameterizedValueExpression<TValue> : ExpressionComponent
        where TValue : notnull
    {
        private string? parameterName = null;

        public string ParameterName => parameterName ?? throw new InvalidOperationException($"The value of '{nameof(ParameterName)}' has not yet been set. Please ensure that the '{nameof(ResolveParameters)}' method has been called first.");
        public TValue ParameterValue { get; }

        private ParameterizedValueExpression(TValue parameterValue)
            => ParameterValue = parameterValue;

        internal static ParameterizedValueExpression<TValue> Create(TValue parameterValue)
            => new ParameterizedValueExpression<TValue>(parameterValue);

        protected override string GetExpressionString()
            => $"@{ParameterName}";

        internal override void ResolveParameters(ParameterProvider parameterProvider)
        {
            parameterName = parameterProvider.CreateParameter(ParameterValue);
        }
    }
}
