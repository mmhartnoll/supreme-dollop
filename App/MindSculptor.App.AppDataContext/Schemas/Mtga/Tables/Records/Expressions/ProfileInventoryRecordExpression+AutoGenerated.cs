using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ProfileInventoryRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression ProfileId { get; }
        public IntegerFieldExpression MythicRareWildcardCount { get; }
        public IntegerFieldExpression RareWildcardCount { get; }
        public IntegerFieldExpression UncommonWildcardCount { get; }
        public IntegerFieldExpression CommonWildcardCount { get; }
        public IntegerFieldExpression GoldCount { get; }
        public IntegerFieldExpression GemCount { get; }
        public ParentRecordExpression<ProfileRecord> ProfileRecord { get; }

        public ProfileInventoryRecordExpression()
        {
            ProfileId = IdFieldExpression.Create("ProfileId");
            MythicRareWildcardCount = IntegerFieldExpression.Create("MythicRareWildcardCount");
            RareWildcardCount = IntegerFieldExpression.Create("RareWildcardCount");
            UncommonWildcardCount = IntegerFieldExpression.Create("UncommonWildcardCount");
            CommonWildcardCount = IntegerFieldExpression.Create("CommonWildcardCount");
            GoldCount = IntegerFieldExpression.Create("GoldCount");
            GemCount = IntegerFieldExpression.Create("GemCount");
            ProfileRecord = ParentRecordExpression<ProfileRecord>.Create(GetProfileRecordExpression);
        }

        private BooleanValueExpression GetProfileRecordExpression(ProfileRecord profileRecord)
        {
            return ProfileId == profileRecord.Id;
        }
    }
}