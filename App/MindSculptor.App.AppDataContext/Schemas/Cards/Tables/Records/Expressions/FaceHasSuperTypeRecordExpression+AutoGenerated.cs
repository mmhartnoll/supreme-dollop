using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FaceHasSuperTypeRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public IdFieldExpression SuperTypeId { get; }
        public IntegerFieldExpression Ordinal { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }
        public ParentRecordExpression<SuperTypeRecord> SuperTypeRecord { get; }

        public FaceHasSuperTypeRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            SuperTypeId = IdFieldExpression.Create("SuperTypeId");
            Ordinal = IntegerFieldExpression.Create("Ordinal");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
            SuperTypeRecord = ParentRecordExpression<SuperTypeRecord>.Create(GetSuperTypeRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }

        private BooleanValueExpression GetSuperTypeRecordExpression(SuperTypeRecord superTypeRecord)
        {
            return SuperTypeId == superTypeRecord.Id;
        }
    }
}