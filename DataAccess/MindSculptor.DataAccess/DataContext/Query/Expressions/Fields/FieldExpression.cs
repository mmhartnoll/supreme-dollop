namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Fields
{
    public abstract class FieldExpression : ExpressionComponent
    {
        public string FieldName { get; }

        protected FieldExpression(string fieldName)
            => FieldName = fieldName;

        protected override string GetExpressionString()
            => FieldName;

        internal sealed override void ResolveParameters(ParameterProvider parameterProvider) { }
    }
}
