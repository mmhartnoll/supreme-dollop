using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class EventMatchRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression EventEntryId { get; }
        public IdFieldExpression OpponentId { get; }
        public ParentRecordExpression<EventEntryRecord> EventEntryRecord { get; }
        public ParentRecordExpression<PlayerRecord> PlayerRecord { get; }

        public EventMatchRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            EventEntryId = IdFieldExpression.Create("EventEntryId");
            OpponentId = IdFieldExpression.Create("OpponentId");
            EventEntryRecord = ParentRecordExpression<EventEntryRecord>.Create(GetEventEntryRecordExpression);
            PlayerRecord = ParentRecordExpression<PlayerRecord>.Create(GetPlayerRecordExpression);
        }

        private BooleanValueExpression GetEventEntryRecordExpression(EventEntryRecord eventEntryRecord)
        {
            return EventEntryId == eventEntryRecord.Id;
        }

        private BooleanValueExpression GetPlayerRecordExpression(PlayerRecord playerRecord)
        {
            return OpponentId == playerRecord.Id;
        }
    }
}