namespace MindSculptor.DataAccess.DataContext.Query.Expressions
{
    public abstract class DataContextRecordExpression
    {
        protected internal ParameterProvider ParameterProvider { get; }

        public DataContextRecordExpression()
            => ParameterProvider = ParameterProvider.Create();
    }
}
