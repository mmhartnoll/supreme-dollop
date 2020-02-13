using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Values;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.DataContext.Query.Expressions.Fields
{
    public class TextFieldExpression : FieldExpression
    {
        private TextFieldExpression(string fieldName)
            : base(fieldName) { }

        public static TextFieldExpression Create(string fieldName)
            => new TextFieldExpression(fieldName);

        public BooleanValueExpression In(params string[] values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In, 
                this, 
                ParameterizedListExpression<string>.Create(values));

        public BooleanValueExpression In(IEnumerable<string> values)
            => BinaryBooleanOperationExpression.Create(
                BinaryBooleanOperationExpression.OperationType.In,
                this,
                ParameterizedListExpression<string>.Create(values));

        public static BooleanValueExpression operator ==(TextFieldExpression expression, string? value)
            => value == null ?
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNull, 
                    expression) as BooleanValueExpression :
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.Equals, 
                    expression, 
                    ParameterizedValueExpression<string>.Create(value));

        public static BooleanValueExpression operator !=(TextFieldExpression expression, string? value)
            => value == null ?
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNotNull, 
                    expression) as BooleanValueExpression :
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.DoesNotEqual, 
                    expression, 
                    ParameterizedValueExpression<string>.Create(value));

        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
