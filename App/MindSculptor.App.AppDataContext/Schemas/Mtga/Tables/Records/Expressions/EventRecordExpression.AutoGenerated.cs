using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class EventRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression MtgaEventId { get; }

        public EventRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            MtgaEventId = TextFieldExpression.Create("MtgaEventId");
        }
    }
}