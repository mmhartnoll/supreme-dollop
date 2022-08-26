using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class EventMatchGameRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression EventMatchId { get; }
        public IntegerFieldExpression GameNumber { get; }
        public BooleanFieldExpression PlayedFirst { get; }
        public BooleanFieldExpression GameWon { get; }
        public ParentRecordExpression<EventMatchRecord> EventMatchRecord { get; }

        public EventMatchGameRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            EventMatchId = IdFieldExpression.Create("EventMatchId");
            GameNumber = IntegerFieldExpression.Create("GameNumber");
            PlayedFirst = BooleanFieldExpression.Create("PlayedFirst");
            GameWon = BooleanFieldExpression.Create("GameWon");
            EventMatchRecord = ParentRecordExpression<EventMatchRecord>.Create(GetEventMatchRecordExpression);
        }

        private BooleanValueExpression GetEventMatchRecordExpression(EventMatchRecord eventMatchRecord)
        {
            return EventMatchId == eventMatchRecord.Id;
        }
    }
}