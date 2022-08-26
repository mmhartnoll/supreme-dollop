using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Values;
using System;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Fields
{
    public class IdFieldExpression : FieldExpression
    {
        private IdFieldExpression(string fieldName)
            : base(fieldName) { }

        public static IdFieldExpression Create(string fieldName)
            => new IdFieldExpression(fieldName);

        public BooleanValueExpression In(params Guid[] values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<Guid>.Create(values));

        public BooleanValueExpression In(IEnumerable<Guid> values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<Guid>.Create(values));

        public static BooleanValueExpression operator ==(IdFieldExpression expression, Guid? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.Equals, 
                    expression, 
                    ParameterizedValueExpression<Guid>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNull, 
                    expression);

        public static BooleanValueExpression operator !=(IdFieldExpression expression, Guid? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.DoesNotEqual, 
                    expression, 
                    ParameterizedValueExpression<Guid>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNotNull, 
                    expression);

        public static BooleanValueExpression operator ==(IdFieldExpression leftExpression, IdFieldExpression rightExpression)
            => BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.Equals,
                    leftExpression,
                    rightExpression);

        public static BooleanValueExpression operator !=(IdFieldExpression leftExpression, IdFieldExpression rightExpression)
            => BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.DoesNotEqual,
                    leftExpression,
                    rightExpression);

        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
