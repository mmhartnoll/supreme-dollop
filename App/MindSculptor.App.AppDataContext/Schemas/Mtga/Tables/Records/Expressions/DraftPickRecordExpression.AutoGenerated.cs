using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftPickRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression EventEntryId { get; }
        public IntegerFieldExpression PackNumber { get; }
        public IntegerFieldExpression PickNumber { get; }
        public BooleanFieldExpression IsFifthCopy { get; }
        public ParentRecordExpression<EventEntryRecord> EventEntryRecord { get; }

        public DraftPickRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            EventEntryId = IdFieldExpression.Create("EventEntryId");
            PackNumber = IntegerFieldExpression.Create("PackNumber");
            PickNumber = IntegerFieldExpression.Create("PickNumber");
            IsFifthCopy = BooleanFieldExpression.Create("IsFifthCopy");
            EventEntryRecord = ParentRecordExpression<EventEntryRecord>.Create(GetEventEntryRecordExpression);
        }

        private BooleanValueExpression GetEventEntryRecordExpression(EventEntryRecord eventEntryRecord)
        {
            return EventEntryId == eventEntryRecord.Id;
        }
    }
}