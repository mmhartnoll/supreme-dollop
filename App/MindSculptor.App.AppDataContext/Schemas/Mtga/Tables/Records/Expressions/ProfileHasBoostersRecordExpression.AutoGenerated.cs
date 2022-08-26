using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ProfileHasBoostersRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression ProfileId { get; }
        public IdFieldExpression BoosterId { get; }
        public IntegerFieldExpression Count { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }
        public ParentRecordExpression<BoosterRecord> BoosterRecord { get; }

        public ProfileHasBoostersRecordExpression()
        {
            ProfileId = IdFieldExpression.Create("ProfileId");
            BoosterId = IdFieldExpression.Create("BoosterId");
            Count = IntegerFieldExpression.Create("Count");
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
            BoosterRecord = ParentRecordExpression<BoosterRecord>.Create(GetBoosterRecordExpression);
        }

        private BooleanValueExpression GetProfileRecordExpression(ProfileRecord profileRecord)
        {
            return ProfileId == profileRecord.Id;
        }

        private BooleanValueExpression GetBoosterRecordExpression(BoosterRecord boosterRecord)
        {
            return BoosterId == boosterRecord.SetId;
        }
    }
}