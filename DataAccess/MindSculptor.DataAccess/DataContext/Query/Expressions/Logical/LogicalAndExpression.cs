namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Logical
{
    public class LogicalAndExpression : LogicalExpression
    {
        private LogicalAndExpression() { }

        private LogicalAndExpression(BooleanValueExpression expression)
            : base(expression) { }

        public static LogicalAndExpression Create(BooleanValueExpression expression)
            => new LogicalAndExpression(expression);

        public static LogicalAndExpression CreateEmpty()
            => new LogicalAndExpression();

        internal override BooleanValueExpression Negate()
        {
            var logicalOrExpression = LogicalOrExpression.CreateEmpty();
            foreach (var subExpression in Expressions)
                logicalOrExpression.AddExpression(subExpression.Negate());
            return logicalOrExpression;
        }

        protected override string GetExpressionString()
            => string.Join(" AND ", Expressions);
    }
}
