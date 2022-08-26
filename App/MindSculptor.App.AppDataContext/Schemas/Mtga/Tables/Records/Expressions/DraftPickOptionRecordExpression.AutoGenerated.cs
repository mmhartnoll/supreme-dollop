using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftPickOptionRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression EventId { get; }
        public IdFieldExpression EventEntryId { get; }
        public IdFieldExpression DigitalCardId { get; }
        public IntegerFieldExpression PackNumber { get; }
        public IntegerFieldExpression PickNumber { get; }
        public IntegerFieldExpression Ordinal { get; }
        public ParentRecordExpression<DraftEventRecord> DraftEventRecord { get; }
        public ParentRecordExpression<EventEntryRecord> EventEntryRecord { get; }
        public ParentRecordExpression<DigitalCardRecord> DigitalCardRecord { get; }

        public DraftPickOptionRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            EventId = IdFieldExpression.Create("EventId");
            EventEntryId = IdFieldExpression.Create("EventEntryId");
            DigitalCardId = IdFieldExpression.Create("DigitalCardId");
            PackNumber = IntegerFieldExpression.Create("PackNumber");
            PickNumber = IntegerFieldExpression.Create("PickNumber");
            Ordinal = IntegerFieldExpression.Create("Ordinal");
            DraftEventRecord = ParentRecordExpression<DraftEventRecord>.Create(GetDraftEventRecordExpression);
            EventEntryRecord = ParentRecordExpression<EventEntryRecord>.Create(GetEventEntryRecordExpression);
            DigitalCardRecord = ParentRecordExpression<DigitalCardRecord>.Create(GetDigitalCardRecordExpression);
        }

        private BooleanValueExpression GetDraftEventRecordExpression(DraftEventRecord draftEventRecord)
        {
            return EventId == draftEventRecord.EventId;
        }

        private BooleanValueExpression GetEventEntryRecordExpression(EventEntryRecord eventEntryRecord)
        {
            return EventEntryId == eventEntryRecord.Id;
        }

        private BooleanValueExpression GetDigitalCardRecordExpression(DigitalCardRecord digitalCardRecord)
        {
            return DigitalCardId == digitalCardRecord.Id;
        }
    }
}