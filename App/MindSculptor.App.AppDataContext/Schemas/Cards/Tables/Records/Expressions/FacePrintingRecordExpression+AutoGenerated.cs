using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FacePrintingRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression BasePrintingId { get; }
        public IdFieldExpression FaceId { get; }
        public IdFieldExpression ImageId { get; }
        public ParentRecordExpression<BasePrintingRecord> BasePrintingRecord { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }

        public FacePrintingRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            BasePrintingId = IdFieldExpression.Create("BasePrintingId");
            FaceId = IdFieldExpression.Create("FaceId");
            ImageId = IdFieldExpression.Create("ImageId");
            BasePrintingRecord = ParentRecordExpression<BasePrintingRecord>.Create(GetBasePrintingRecordExpression);
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
        }

        private BooleanValueExpression GetBasePrintingRecordExpression(BasePrintingRecord basePrintingRecord)
        {
            return BasePrintingId == basePrintingRecord.Id;
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }
    }
}