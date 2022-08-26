using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class EventMatchResultRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression EventMatchId { get; }
        public BooleanFieldExpression MatchWon { get; }
        public ParentRecordExpression<EventMatchRecord> EventMatchRecord { get; }

        public EventMatchResultRecordExpression()
        {
            EventMatchId = IdFieldExpression.Create("EventMatchId");
            MatchWon = BooleanFieldExpression.Create("MatchWon");
            EventMatchRecord = ParentRecordExpression<EventMatchRecord>.Create(GetEventMatchRecordExpression);
        }

        private BooleanValueExpression GetEventMatchRecordExpression(EventMatchRecord eventMatchRecord)
        {
            return EventMatchId == eventMatchRecord.Id;
        }
    }
}