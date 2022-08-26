using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class PrintingTypeRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Value { get; }

        public PrintingTypeRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Value = TextFieldExpression.Create("Value");
        }
    }
}