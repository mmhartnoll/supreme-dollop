using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class LoyaltyRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public IntegerFieldExpression BaseLoyalty { get; }
        public TextFieldExpression LoyaltyFormat { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }

        public LoyaltyRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            BaseLoyalty = IntegerFieldExpression.Create("BaseLoyalty");
            LoyaltyFormat = TextFieldExpression.Create("LoyaltyFormat");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }
    }
}