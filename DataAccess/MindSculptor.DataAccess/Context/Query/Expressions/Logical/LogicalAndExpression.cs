namespace MindSculptor.DataAccess.Context.Query.Expressions.Logical
{
    public class LogicalAndExpression : LogicalExpression
    {
        public LogicalAndExpression() { }

        public LogicalAndExpression(BooleanValueExpression expression)
            : base(expression) { }

        internal override BooleanValueExpression Negate()
        {
            var logicalOrExpression = new LogicalOrExpression();
            foreach (var subExpression in Expressions)
                logicalOrExpression.AddExpression(subExpression.Negate());
            return logicalOrExpression;
        }

        protected override string GetExpressionString()
            => string.Join(" AND ", Expressions);
    }
}
