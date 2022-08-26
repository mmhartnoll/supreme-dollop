using System;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Logical
{
    public class UnaryBooleanOperationExpression : BooleanValueExpression
    {
        public OperationType Type { get; }
        public ExpressionComponent OperandExpression { get; }

        private UnaryBooleanOperationExpression(OperationType type, ExpressionComponent operandExpression)
        {
            Type = type;
            OperandExpression = operandExpression;
        }

        internal static UnaryBooleanOperationExpression Create(OperationType type, ExpressionComponent operandExpression)
            => new UnaryBooleanOperationExpression(type, operandExpression);

        internal override BooleanValueExpression Negate()
        {
            return Create(NegateOperationType(Type), OperandExpression);

            static OperationType NegateOperationType(OperationType operationType)
                => operationType switch
                {
                    OperationType.IsNull => OperationType.IsNotNull,
                    OperationType.IsNotNull => OperationType.IsNull,
                    _ => throw new NotSupportedException($"{nameof(OperationType)} '{operationType}' is not suppported.")
                };
        }

        protected override string GetExpressionString()
            => Type switch
            {
                OperationType.IsNull => $"{OperandExpression} IS NULL",
                OperationType.IsNotNull => $"{OperandExpression} IS NOT NULL",
                _ => throw new NotSupportedException($"{nameof(OperationType)} '{Type}' is not suppported.")
            };

        internal override void ResolveParameters(ParameterProvider parameterProvider)
            => OperandExpression.ResolveParameters(parameterProvider);

        public enum OperationType
        {
            IsNull,
            IsNotNull
        }
    }
}
