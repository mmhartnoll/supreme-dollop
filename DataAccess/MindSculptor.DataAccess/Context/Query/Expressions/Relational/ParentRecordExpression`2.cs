using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using System;

namespace MindSculptor.DataAccess.Context.Query.Expressions.Relational
{
    public class ParentRecordExpression<TRecord>
        where TRecord : DatabaseRecord
    {
        private readonly Func<TRecord, BooleanValueExpression> expressionFunction;

        private ParentRecordExpression(Func<TRecord, BooleanValueExpression> expressionFunction)
            => this.expressionFunction = expressionFunction;

        public static ParentRecordExpression<TRecord> Create(Func<TRecord, BooleanValueExpression> expression)
            => new ParentRecordExpression<TRecord>(expression);

        public static BooleanValueExpression operator ==(ParentRecordExpression<TRecord> expression, TRecord record)
            => expression.expressionFunction.Invoke(record);

        public static BooleanValueExpression operator !=(ParentRecordExpression<TRecord> expression, TRecord record)
            => expression.expressionFunction.Invoke(record).Negate();

        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
