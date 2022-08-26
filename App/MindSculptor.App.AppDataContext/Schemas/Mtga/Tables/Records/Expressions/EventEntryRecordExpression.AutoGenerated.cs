using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class EventEntryRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression EventId { get; }
        public IdFieldExpression ProfileId { get; }
        public ParentRecordExpression<EventRecord> EventRecord { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }

        public EventEntryRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            EventId = IdFieldExpression.Create("EventId");
            ProfileId = IdFieldExpression.Create("ProfileId");
            EventRecord = ParentRecordExpression<EventRecord>.Create(GetEventRecordExpression);
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
        }

        private BooleanValueExpression GetEventRecordExpression(EventRecord eventRecord)
        {
            return EventId == eventRecord.Id;
        }

        private BooleanValueExpression GetProfileRecordExpression(ProfileRecord profileRecord)
        {
            return ProfileId == profileRecord.Id;
        }
    }
}