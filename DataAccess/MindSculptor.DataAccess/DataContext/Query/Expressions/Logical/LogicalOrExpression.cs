namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Logical
{
    public class LogicalOrExpression : LogicalExpression
    {
        private LogicalOrExpression() { }

        private LogicalOrExpression(BooleanValueExpression expression)
            : base(expression) { }

        public static LogicalOrExpression Create(BooleanValueExpression expression)
            => new LogicalOrExpression(expression);

        public static LogicalOrExpression CreateEmpty()
            => new LogicalOrExpression();

        internal override BooleanValueExpression Negate()
        {
            var logicalOrExpression = LogicalAndExpression.CreateEmpty();
            foreach (var subExpression in Expressions)
                logicalOrExpression.AddExpression(subExpression.Negate());
            return logicalOrExpression;
        }

        protected override string GetExpressionString()
            => string.Join(" OR ", Expressions);
    }
}
