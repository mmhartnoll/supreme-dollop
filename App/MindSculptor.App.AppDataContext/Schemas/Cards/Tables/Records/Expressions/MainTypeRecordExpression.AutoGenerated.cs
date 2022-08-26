using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class MainTypeRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Value { get; }

        public MainTypeRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Value = TextFieldExpression.Create("Value");
        }
    }
}