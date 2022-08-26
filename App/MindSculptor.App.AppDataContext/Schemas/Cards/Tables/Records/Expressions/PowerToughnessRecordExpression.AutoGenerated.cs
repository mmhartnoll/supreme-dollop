using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class PowerToughnessRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public IntegerFieldExpression BasePower { get; }
        public TextFieldExpression PowerFormat { get; }
        public IntegerFieldExpression BaseToughness { get; }
        public TextFieldExpression ToughnessFormat { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }

        public PowerToughnessRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            BasePower = IntegerFieldExpression.Create("BasePower");
            PowerFormat = TextFieldExpression.Create("PowerFormat");
            BaseToughness = IntegerFieldExpression.Create("BaseToughness");
            ToughnessFormat = TextFieldExpression.Create("ToughnessFormat");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }
    }
}