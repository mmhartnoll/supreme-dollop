using MindSculptor.Tools.Extensions;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Logical
{
    public abstract class LogicalExpression : BooleanValueExpression
    {
        private readonly List<BooleanValueExpression> expressions = new List<BooleanValueExpression>();

        public IEnumerable<BooleanValueExpression> Expressions => expressions.Enumerate();

        protected LogicalExpression() { }

        protected LogicalExpression(BooleanValueExpression expression)
            => expressions.Add(expression);

        public void AddExpression(BooleanValueExpression expression)
            => expressions.Add(expression);

        internal override void ResolveParameters(ParameterProvider parameterProvider)
        {
            foreach (var expression in expressions)
                expression.ResolveParameters(parameterProvider);
        }
    }
}
