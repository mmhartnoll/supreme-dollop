using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ProfileHasCardRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression ProfileId { get; }
        public IdFieldExpression BasePrintingId { get; }
        public IntegerFieldExpression Count { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }
        public ParentRecordExpression<CardRecord> CardRecord { get; }

        public ProfileHasCardRecordExpression()
        {
            ProfileId = IdFieldExpression.Create("ProfileId");
            BasePrintingId = IdFieldExpression.Create("BasePrintingId");
            Count = IntegerFieldExpression.Create("Count");
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
            CardRecord = ParentRecordExpression<CardRecord>.Create(GetCardRecordExpression);
        }

        private BooleanValueExpression GetProfileRecordExpression(ProfileRecord profileRecord)
        {
            return ProfileId == profileRecord.Id;
        }

        private BooleanValueExpression GetCardRecordExpression(CardRecord cardRecord)
        {
            return BasePrintingId == cardRecord.Id;
        }
    }
}