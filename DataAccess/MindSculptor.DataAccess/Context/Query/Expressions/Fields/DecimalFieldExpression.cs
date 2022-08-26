using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Values;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Fields
{
    public class DecimalFieldExpression : FieldExpression
    {
        public DecimalFieldExpression(string fieldName)
            : base(fieldName) { }

        public static DecimalFieldExpression Create(string fieldName)
            => new DecimalFieldExpression(fieldName);

        public BooleanValueExpression In(params decimal[] values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<decimal>.Create(values));

        public BooleanValueExpression In(IEnumerable<decimal> values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<decimal>.Create(values));

        public static BooleanValueExpression operator ==(DecimalFieldExpression expression, decimal? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.Equals,
                    expression,
                    ParameterizedValueExpression<decimal>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNull,
                    expression);

        public static BooleanValueExpression operator !=(DecimalFieldExpression expression, decimal? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.DoesNotEqual,
                    expression,
                    ParameterizedValueExpression<decimal>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNotNull,
                    expression);

        public static BooleanValueExpression operator >(DecimalFieldExpression expression, decimal value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.GreaterThan,
                expression,
                ParameterizedValueExpression<decimal>.Create(value));

        public static BooleanValueExpression operator <(DecimalFieldExpression expression, decimal value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.LessThan,
                expression,
                ParameterizedValueExpression<decimal>.Create(value));

        public static BooleanValueExpression operator >=(DecimalFieldExpression expression, decimal value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.GreaterThanOrEqual,
                expression,
                ParameterizedValueExpression<decimal>.Create(value));

        public static BooleanValueExpression operator <=(DecimalFieldExpression expression, decimal value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.LessThanOrEqual,
                expression,
                ParameterizedValueExpression<decimal>.Create(value));

        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
