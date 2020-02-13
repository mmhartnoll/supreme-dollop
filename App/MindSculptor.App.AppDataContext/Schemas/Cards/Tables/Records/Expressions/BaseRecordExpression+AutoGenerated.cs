using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class BaseRecordExpression : DataContextRecordExpression
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