using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FaceHasMainTypeRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public IdFieldExpression MainTypeId { get; }
        public IntegerFieldExpression Ordinal { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }
        public ParentRecordExpression<MainTypeRecord> MainTypeRecord { get; }

        public FaceHasMainTypeRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            MainTypeId = IdFieldExpression.Create("MainTypeId");
            Ordinal = IntegerFieldExpression.Create("Ordinal");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
            MainTypeRecord = ParentRecordExpression<MainTypeRecord>.Create(GetMainTypeRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }

        private BooleanValueExpression GetMainTypeRecordExpression(MainTypeRecord mainTypeRecord)
        {
            return MainTypeId == mainTypeRecord.Id;
        }
    }
}