using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ActiveDraftEventEntryRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression DraftEventEntryId { get; }
        public IdFieldExpression DraftEventId { get; }
        public IdFieldExpression ProfileId { get; }
        public ParentRecordExpression<DraftEventEntryRecord> DraftEventEntryRecord { get; }
        public ParentRecordExpression<DraftEventRecord> DraftEventRecord { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }

        public ActiveDraftEventEntryRecordExpression()
        {
            DraftEventEntryId = IdFieldExpression.Create("DraftEventEntryId");
            DraftEventId = IdFieldExpression.Create("DraftEventId");
            ProfileId = IdFieldExpression.Create("ProfileId");
            DraftEventEntryRecord = ParentRecordExpression<DraftEventEntryRecord>.Create(GetDraftEventEntryRecordExpression);
            DraftEventRecord = ParentRecordExpression<DraftEventRecord>.Create(GetDraftEventRecordExpression);
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
        }

        private BooleanValueExpression GetDraftEventEntryRecordExpression(DraftEventEntryRecord draftEventEntryRecord)
        {
            return DraftEventEntryId == draftEventEntryRecord.Id && DraftEventId == draftEventEntryRecord.DraftEventId && ProfileId == draftEventEntryRecord.ProfileId;
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