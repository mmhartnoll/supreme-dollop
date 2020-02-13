using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class CardTypeRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Value { get; }

        public CardTypeRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Value = TextFieldExpression.Create("Value");
        }
    }
}