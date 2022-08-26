namespace MindSculptor.DataAccess.Context.Query.Expressions
{
    public abstract class ExpressionComponent
    {
        public string ExpressionString => GetExpressionString();

        public override string ToString() => GetExpressionString();

        protected abstract string GetExpressionString();

        internal abstract void ResolveParameters(ParameterProvider parameterProvider);
    }
}
