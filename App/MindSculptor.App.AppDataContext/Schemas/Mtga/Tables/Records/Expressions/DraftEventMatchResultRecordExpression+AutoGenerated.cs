using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftEventMatchResultRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression DraftEventMatchId { get; }
        public BooleanFieldExpression MatchWon { get; }
        public ParentRecordExpression<DraftEventMatchRecord> DraftEventMatchRecord { get; }

        public DraftEventMatchResultRecordExpression()
        {
            DraftEventMatchId = IdFieldExpression.Create("DraftEventMatchId");
            MatchWon = BooleanFieldExpression.Create("MatchWon");
            DraftEventMatchRecord = ParentRecordExpression<DraftEventMatchRecord>.Create(GetDraftEventMatchRecordExpression);
        }

        private BooleanValueExpression GetDraftEventMatchRecordExpression(DraftEventMatchRecord draftEventMatchRecord)
        {
            return DraftEventMatchId == draftEventMatchRecord.Id;
        }
    }
}