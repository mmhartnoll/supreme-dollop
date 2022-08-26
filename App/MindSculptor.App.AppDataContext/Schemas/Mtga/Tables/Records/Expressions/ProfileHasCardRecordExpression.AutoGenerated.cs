using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ProfileHasCardRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression ProfileId { get; }
        public IdFieldExpression DigitalCardId { get; }
        public IntegerFieldExpression Count { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }
        public ParentRecordExpression<DigitalCardRecord> DigitalCardRecord { get; }

        public ProfileHasCardRecordExpression()
        {
            ProfileId = IdFieldExpression.Create("ProfileId");
            DigitalCardId = IdFieldExpression.Create("DigitalCardId");
            Count = IntegerFieldExpression.Create("Count");
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
            DigitalCardRecord = ParentRecordExpression<DigitalCardRecord>.Create(GetDigitalCardRecordExpression);
        }

        private BooleanValueExpression GetProfileRecordExpression(ProfileRecord profileRecord)
        {
            return ProfileId == profileRecord.Id;
        }

        private BooleanValueExpression GetDigitalCardRecordExpression(DigitalCardRecord digitalCardRecord)
        {
            return DigitalCardId == digitalCardRecord.Id;
        }
    }
}