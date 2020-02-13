using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftEventEntryRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression DraftEventId { get; }
        public IdFieldExpression ProfileId { get; }
        public ParentRecordExpression<DraftEventRecord> DraftEventRecord { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }

        public DraftEventEntryRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            DraftEventId = IdFieldExpression.Create("DraftEventId");
            ProfileId = IdFieldExpression.Create("ProfileId");
            DraftEventRecord = ParentRecordExpression<DraftEventRecord>.Create(GetDraftEventRecordExpression);
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
        }

        private BooleanValueExpression GetDraftEventRecordExpression(DraftEventRecord draftEventRecord)
        {
            return DraftEventId == draftEventRecord.Id;
        }

        private BooleanValueExpression GetProfileRecordExpression(ProfileRecord profileRecord)
        {
            return ProfileId == profileRecord.Id;
        }
    }
}