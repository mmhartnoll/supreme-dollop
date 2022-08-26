using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class OracleTextRecordExpression : DatabaseRecordExpression
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