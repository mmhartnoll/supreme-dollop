using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Values;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Fields
{
    public class BooleanFieldExpression : FieldExpression
    {
        private BooleanFieldExpression(string fieldName)
            : base(fieldName) { }

        public static BooleanFieldExpression Create(string fieldName)
            => new BooleanFieldExpression(fieldName);

        public static implicit operator BooleanValueExpression(BooleanFieldExpression expression)
            => expression == true;

        public static BooleanValueExpression operator ==(BooleanFieldExpression expression, bool? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.Equals, 
                    expression, 
                    ParameterizedValueExpression<bool>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNull, 
                    expression);

        public static BooleanValueExpression operator !=(BooleanFieldExpression expression, bool? value)
            => value.HasValue ?
                BinaryBooleanOperationExpression.Create(
                    BinaryBooleanOperationExpression.OperationType.DoesNotEqual, 
                    expression, ParameterizedValueExpression<bool>.Create(value.Value)) as BooleanValueExpression :
                UnaryBooleanOperationExpression.Create(
                    UnaryBooleanOperationExpression.OperationType.IsNotNull, 
                    expression);

        public static BooleanValueExpression operator !(BooleanFieldExpression expression)
            => expression == false;

        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
