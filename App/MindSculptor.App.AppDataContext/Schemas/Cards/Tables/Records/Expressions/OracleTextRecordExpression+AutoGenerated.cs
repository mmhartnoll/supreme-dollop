using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class OracleTextRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public TextFieldExpression Value { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }

        public OracleTextRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            Value = TextFieldExpression.Create("Value");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }
    }
}