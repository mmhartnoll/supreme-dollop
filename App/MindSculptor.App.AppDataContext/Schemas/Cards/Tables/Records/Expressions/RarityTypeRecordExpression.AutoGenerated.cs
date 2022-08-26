using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class RarityTypeRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Value { get; }

        public RarityTypeRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Value = TextFieldExpression.Create("Value");
        }
    }
}