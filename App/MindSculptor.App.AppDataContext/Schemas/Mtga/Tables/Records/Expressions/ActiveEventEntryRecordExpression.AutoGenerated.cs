using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ActiveEventEntryRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression EventEntryId { get; }
        public IdFieldExpression EventId { get; }
        public IdFieldExpression ProfileId { get; }
        public ParentRecordExpression<EventEntryRecord> EventEntryRecord { get; }
        public ParentRecordExpression<EventRecord> EventRecord { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }

        public ActiveEventEntryRecordExpression()
        {
            EventEntryId = IdFieldExpression.Create("EventEntryId");
            EventId = IdFieldExpression.Create("EventId");
            ProfileId = IdFieldExpression.Create("ProfileId");
            EventEntryRecord = ParentRecordExpression<EventEntryRecord>.Create(GetEventEntryRecordExpression);
            EventRecord = ParentRecordExpression<EventRecord>.Create(GetEventRecordExpression);
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
        }

        private BooleanValueExpression GetEventEntryRecordExpression(EventEntryRecord eventEntryRecord)
        {
            return EventEntryId == eventEntryRecord.Id && EventId == eventEntryRecord.EventId && ProfileId == eventEntryRecord.ProfileId;
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