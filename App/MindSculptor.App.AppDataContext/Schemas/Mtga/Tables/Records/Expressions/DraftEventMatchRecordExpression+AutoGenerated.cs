using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftEventMatchRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression DraftEventEntryId { get; }
        public IdFieldExpression OpponentId { get; }
        public ParentRecordExpression<DraftEventEntryRecord> DraftEventEntryRecord { get; }
        public ParentRecordExpression<OpponentRecord> OpponentRecord { get; }

        public DraftEventMatchRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            DraftEventEntryId = IdFieldExpression.Create("DraftEventEntryId");
            OpponentId = IdFieldExpression.Create("OpponentId");
            DraftEventEntryRecord = ParentRecordExpression<DraftEventEntryRecord>.Create(GetDraftEventEntryRecordExpression);
            OpponentRecord = ParentRecordExpression<OpponentRecord>.Create(GetOpponentRecordExpression);
        }

        private BooleanValueExpression GetDraftEventEntryRecordExpression(DraftEventEntryRecord draftEventEntryRecord)
        {
            return DraftEventEntryId == draftEventEntryRecord.Id;
        }

        private BooleanValueExpression GetOpponentRecordExpression(OpponentRecord opponentRecord)
        {
            return OpponentId == opponentRecord.Id;
        }
    }
}