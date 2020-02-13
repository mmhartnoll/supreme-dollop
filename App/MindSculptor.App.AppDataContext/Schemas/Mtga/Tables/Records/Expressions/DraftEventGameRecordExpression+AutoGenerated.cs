using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftEventGameRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression DraftEventMatchId { get; }
        public IntegerFieldExpression GameNumber { get; }
        public BooleanFieldExpression PlayedFirst { get; }
        public BooleanFieldExpression GameWon { get; }
        public ParentRecordExpression<DraftEventMatchRecord> DraftEventMatchRecord { get; }

        public DraftEventGameRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            DraftEventMatchId = IdFieldExpression.Create("DraftEventMatchId");
            GameNumber = IntegerFieldExpression.Create("GameNumber");
            PlayedFirst = BooleanFieldExpression.Create("PlayedFirst");
            GameWon = BooleanFieldExpression.Create("GameWon");
            DraftEventMatchRecord = ParentRecordExpression<DraftEventMatchRecord>.Create(GetDraftEventMatchRecordExpression);
        }

        private BooleanValueExpression GetDraftEventMatchRecordExpression(DraftEventMatchRecord draftEventMatchRecord)
        {
            return DraftEventMatchId == draftEventMatchRecord.Id;
        }
    }
}