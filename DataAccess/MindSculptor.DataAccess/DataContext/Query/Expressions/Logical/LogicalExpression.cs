using MindSculptor.Tools.Extensions;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Logical
{
    public abstract class LogicalExpression : BooleanValueExpression
    {
        private readonly Queue<BooleanValueExpression> expressions;

        public IEnumerable<BooleanValueExpression> Expressions => expressions.Enumerate();

        protected LogicalExpression()
            => expressions = new Queue<BooleanValueExpression>();

        protected LogicalExpression(BooleanValueExpression expression)
            => (expressions = new Queue<BooleanValueExpression>()).Enqueue(expression);

        public void AddExpression(BooleanValueExpression expression)
            => expressions.Enqueue(expression);

        internal override void ResolveParameters(ParameterProvider parameterProvider)
        {
            foreach (var expression in expressions)
                expression.ResolveParameters(parameterProvider);
        }
    }
}
