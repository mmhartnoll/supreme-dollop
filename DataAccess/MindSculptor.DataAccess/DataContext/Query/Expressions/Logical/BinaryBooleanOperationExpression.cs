using System;

namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Logical
{
    public class BinaryBooleanOperationExpression : BooleanValueExpression
    {
        public OperationType Type { get; }
        public ExpressionComponent LeftOperandExpression { get; }
        public ExpressionComponent RightOperandExpression { get; }

        private BinaryBooleanOperationExpression(OperationType type, ExpressionComponent leftOperandExpression, ExpressionComponent rightOperandExpression)
        {
            Type = type;
            LeftOperandExpression = leftOperandExpression;
            RightOperandExpression = rightOperandExpression;
        }

        internal static BinaryBooleanOperationExpression Create(OperationType type, ExpressionComponent leftOperandExpression, ExpressionComponent rightOperandExpression)
            => new BinaryBooleanOperationExpression(type, leftOperandExpression, rightOperandExpression);

        internal override BooleanValueExpression Negate()
        {
            return Create(NegateOperationType(Type), LeftOperandExpression, RightOperandExpression);

            static OperationType NegateOperationType(OperationType operationType)
                => operationType switch
                {
                    OperationType.Equals => OperationType.DoesNotEqual,
                    OperationType.DoesNotEqual => OperationType.Equals,
                    OperationType.GreaterThan => OperationType.LessThanOrEqual,
                    OperationType.LessThan => OperationType.GreaterThanOrEqual,
                    OperationType.GreaterThanOrEqual => OperationType.LessThan,
                    OperationType.LessThanOrEqual => OperationType.GreaterThan,
                    OperationType.In => OperationType.NotIn,
                    OperationType.NotIn => OperationType.In,
                    _ => throw new NotSupportedException($"{nameof(OperationType)} '{operationType}' is not suppported.")
                };
        }


        protected override string GetExpressionString()
            => Type switch
            {
                OperationType.Equals => $"{LeftOperandExpression} = {RightOperandExpression}",
                OperationType.DoesNotEqual => $"{LeftOperandExpression} <> {RightOperandExpression}",
                OperationType.GreaterThan => $"{LeftOperandExpression} > {RightOperandExpression}",
                OperationType.LessThan => $"{LeftOperandExpression} < {RightOperandExpression}",
                OperationType.GreaterThanOrEqual => $"{LeftOperandExpression} >= {RightOperandExpression}",
                OperationType.LessThanOrEqual => $"{LeftOperandExpression} <= {RightOperandExpression}",
                OperationType.In => $"{LeftOperandExpression} IN ({RightOperandExpression})",
                OperationType.NotIn => $"{LeftOperandExpression} NOT IN ({RightOperandExpression})",
                _ => throw new NotSupportedException($"{nameof(OperationType)} '{Type}' is not suppported.")
            };

        internal override void ResolveParameters(ParameterProvider parameterProvider)
        {
            LeftOperandExpression.ResolveParameters(parameterProvider);
            RightOperandExpression.ResolveParameters(parameterProvider);
        }

        public enum OperationType
        {
            Equals,
            DoesNotEqual,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            In,
            NotIn
        }
    }
}
