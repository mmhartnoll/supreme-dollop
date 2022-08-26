namespace MindSculptor.DataAccess.Context.Query.Expressions.Logical
{
    public class LogicalOrExpression : LogicalExpression
    {
        public LogicalOrExpression() { }

        public LogicalOrExpression(BooleanValueExpression expression)
            : base(expression) { }

        internal override BooleanValueExpression Negate()
        {
            var logicalOrExpression = new LogicalAndExpression();
            foreach (var subExpression in Expressions)
                logicalOrExpression.AddExpression(subExpression.Negate());
            return logicalOrExpression;
        }

        protected override string GetExpressionString()
            => string.Join(" OR ", Expressions);
    }
}
