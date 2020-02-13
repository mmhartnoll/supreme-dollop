using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class OpponentRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression ScreenName { get; }
        public IntegerFieldExpression UserId { get; }

        public OpponentRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            ScreenName = TextFieldExpression.Create("ScreenName");
            UserId = IntegerFieldExpression.Create("UserId");
        }
    }
}