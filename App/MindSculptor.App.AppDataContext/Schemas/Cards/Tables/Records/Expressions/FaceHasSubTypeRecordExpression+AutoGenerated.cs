using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FaceHasSubTypeRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public IdFieldExpression SubTypeId { get; }
        public IntegerFieldExpression Ordinal { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }
        public ParentRecordExpression<SubTypeRecord> SubTypeRecord { get; }

        public FaceHasSubTypeRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            SubTypeId = IdFieldExpression.Create("SubTypeId");
            Ordinal = IntegerFieldExpression.Create("Ordinal");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
            SubTypeRecord = ParentRecordExpression<SubTypeRecord>.Create(GetSubTypeRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }

        private BooleanValueExpression GetSubTypeRecordExpression(SubTypeRecord subTypeRecord)
        {
            return SubTypeId == subTypeRecord.Id;
        }
    }
}