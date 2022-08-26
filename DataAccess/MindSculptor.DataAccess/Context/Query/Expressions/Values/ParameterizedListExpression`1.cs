using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Values
{
    public class ParameterizedListExpression<TValue> : ExpressionComponent
        where TValue : notnull
    {
        private NullableReference<IEnumerable<string>> parameterNames = null;

        public IEnumerable<string> ParameterNames => parameterNames.HasValue ? parameterNames.Value : throw new InvalidOperationException($"The values of '{nameof(ParameterNames)}' have not yet been set. Please ensure that the '{nameof(ResolveParameters)}' method has been called first.");

        public IEnumerable<TValue> ParameterValues { get; }

        public ParameterizedListExpression(params TValue[] parameterValues)
            => ParameterValues = parameterValues.Enumerate();

        public ParameterizedListExpression(IEnumerable<TValue> parameterValues)
            => ParameterValues = parameterValues.Enumerate();

        internal static ParameterizedListExpression<TValue> Create(params TValue[] parameterValues)
            => new ParameterizedListExpression<TValue>(parameterValues);

        internal static ParameterizedListExpression<TValue> Create(IEnumerable<TValue> parameterValues)
            => new ParameterizedListExpression<TValue>(parameterValues);

        protected override string GetExpressionString()
            => string.Join(", ", ParameterNames.Select(parameterName => $"@{parameterName}"));

        internal override void ResolveParameters(ParameterProvider parameterProvider)
            => parameterNames = NullableReference.FromValue(ParameterValues.Select(value => parameterProvider.CreateParameter(value))
                .ToList()
                .Enumerate());
    }
}
