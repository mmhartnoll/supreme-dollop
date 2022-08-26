using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class BaseRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression CardTypeId { get; }
        public ParentRecordExpression<CardTypeRecord> CardTypeRecord { get; }

        public BaseRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            CardTypeId = IdFieldExpression.Create("CardTypeId");
            CardTypeRecord = ParentRecordExpression<CardTypeRecord>.Create(GetCardTypeRecordExpression);
        }

        private BooleanValueExpression GetCardTypeRecordExpression(CardTypeRecord cardTypeRecord)
        {
            return CardTypeId == cardTypeRecord.Id;
        }
    }
}