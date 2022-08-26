using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftEventRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression EventId { get; }
        public IdFieldExpression SetId { get; }
        public TextFieldExpression DraftType { get; }
        public ParentRecordExpression<EventRecord> EventRecord { get; }
        public ParentRecordExpression<SetRecord> SetRecord { get; }

        public DraftEventRecordExpression()
        {
            EventId = IdFieldExpression.Create("EventId");
            SetId = IdFieldExpression.Create("SetId");
            DraftType = TextFieldExpression.Create("DraftType");
            EventRecord = ParentRecordExpression<EventRecord>.Create(GetEventRecordExpression);
            SetRecord = ParentRecordExpression<SetRecord>.Create(GetSetRecordExpression);
        }

        private BooleanValueExpression GetEventRecordExpression(EventRecord eventRecord)
        {
            return EventId == eventRecord.Id;
        }

        private BooleanValueExpression GetSetRecordExpression(SetRecord setRecord)
        {
            return SetId == setRecord.Id;
        }
    }
}