using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Values;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Fields
{
    public class IntegerFieldExpression : FieldExpression
    {
        private IntegerFieldExpression(string fieldName)
            : base(fieldName) { }

        public static IntegerFieldExpression Create(string fieldName)
            => new IntegerFieldExpression(fieldName);

        public BooleanValueExpression In(params int[] values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<int>.Create(values));

        public BooleanValueExpression In(IEnumerable<int> values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<int>.Create(values));

        public static BooleanValueExpression operator ==(IntegerFieldExpression expression, int? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.Equals, 
                    expression, 
                    ParameterizedValueExpression<int>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNull, 
                    expression);

        public static BooleanValueExpression operator !=(IntegerFieldExpression expression, int? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.DoesNotEqual, 
                    expression, 
                    ParameterizedValueExpression<int>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNotNull, 
                    expression);

        public static BooleanValueExpression operator >(IntegerFieldExpression expression, int value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.GreaterThan, 
                expression, 
                ParameterizedValueExpression<int>.Create(value));

        public static BooleanValueExpression operator <(IntegerFieldExpression expression, int value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.LessThan, 
                expression, 
                ParameterizedValueExpression<int>.Create(value));

        public static BooleanValueExpression operator >=(IntegerFieldExpression expression, int value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.GreaterThanOrEqual, 
                expression, 
                ParameterizedValueExpression<int>.Create(value));

        public static BooleanValueExpression operator <=(IntegerFieldExpression expression, int value)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.LessThanOrEqual, 
                expression, 
                ParameterizedValueExpression<int>.Create(value));

        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
