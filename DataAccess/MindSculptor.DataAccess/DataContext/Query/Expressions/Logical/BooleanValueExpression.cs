namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Logical
{
    public abstract class BooleanValueExpression : ExpressionComponent
    {
        protected BooleanValueExpression() { }

        public static BooleanValueExpression operator &(BooleanValueExpression leftExpression, BooleanValueExpression rightExpression)
        {
            var expression = leftExpression as LogicalAndExpression ?? LogicalAndExpression.Create(leftExpression);
            if (rightExpression is LogicalAndExpression andExpression)
                foreach (var childExpression in andExpression.Expressions)
                    expression.AddExpression(childExpression);
            else
                expression.AddExpression(rightExpression);
            return expression;
        }

        public static BooleanValueExpression operator |(BooleanValueExpression leftExpression, BooleanValueExpression rightExpression)
        {
            var expression = leftExpression as LogicalOrExpression ?? LogicalOrExpression.Create(leftExpression);
            if (rightExpression is LogicalOrExpression orExpression)
                foreach (var childExpression in orExpression.Expressions)
                    expression.AddExpression(childExpression);
            else
                expression.AddExpression(rightExpression);
            return expression;
        }

        public static BooleanValueExpression operator !(BooleanValueExpression expression)
            => expression.Negate();

        internal abstract BooleanValueExpression Negate();

        
        /* The following operator overrides ensure that the && and || operators do not 'short-cut' the & and | operators
         * -- x && y is evaluated as T.false(x) ? x : T.&(x, y)
         * -- x || y is evaulated as T.true(x)  ? x : T.|(x, y) 
         */
        public static bool operator true(BooleanValueExpression expression) => false;
        public static bool operator false(BooleanValueExpression expression) => false;
    }
}
