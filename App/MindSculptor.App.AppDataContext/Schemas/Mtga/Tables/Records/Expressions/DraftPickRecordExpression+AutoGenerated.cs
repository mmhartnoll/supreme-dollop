using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftPickRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression DraftEventEntryId { get; }
        public IdFieldExpression DigitalCardId { get; }
        public IntegerFieldExpression PackNumber { get; }
        public IntegerFieldExpression PickNumber { get; }
        public IntegerFieldExpression Ordinal { get; }
        public BooleanFieldExpression IsPick { get; }
        public ParentRecordExpression<DraftEventEntryRecord> DraftEventEntryRecord { get; }
        public ParentRecordExpression<CardRecord> CardRecord { get; }

        public DraftPickRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            DraftEventEntryId = IdFieldExpression.Create("DraftEventEntryId");
            DigitalCardId = IdFieldExpression.Create("DigitalCardId");
            PackNumber = IntegerFieldExpression.Create("PackNumber");
            PickNumber = IntegerFieldExpression.Create("PickNumber");
            Ordinal = IntegerFieldExpression.Create("Ordinal");
            IsPick = BooleanFieldExpression.Create("IsPick");
            DraftEventEntryRecord = ParentRecordExpression<DraftEventEntryRecord>.Create(GetDraftEventEntryRecordExpression);
            CardRecord = ParentRecordExpression<CardRecord>.Create(GetCardRecordExpression);
        }

        private BooleanValueExpression GetDraftEventEntryRecordExpression(DraftEventEntryRecord draftEventEntryRecord)
        {
            return DraftEventEntryId == draftEventEntryRecord.Id;
        }

        private BooleanValueExpression GetCardRecordExpression(CardRecord cardRecord)
        {
            return DigitalCardId == cardRecord.Id;
        }
    }
}