using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FaceHasCastingCostRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FaceId { get; }
        public IdFieldExpression ManaSymbolId { get; }
        public IntegerFieldExpression Ordinal { get; }
        public IntegerFieldExpression Count { get; }
        public ParentRecordExpression<FaceRecord> FaceRecord { get; }
        public ParentRecordExpression<ManaSymbolRecord> ManaSymbolRecord { get; }

        public FaceHasCastingCostRecordExpression()
        {
            FaceId = IdFieldExpression.Create("FaceId");
            ManaSymbolId = IdFieldExpression.Create("ManaSymbolId");
            Ordinal = IntegerFieldExpression.Create("Ordinal");
            Count = IntegerFieldExpression.Create("Count");
            FaceRecord = ParentRecordExpression<FaceRecord>.Create(GetFaceRecordExpression);
            ManaSymbolRecord = ParentRecordExpression<ManaSymbolRecord>.Create(GetManaSymbolRecordExpression);
        }

        private BooleanValueExpression GetFaceRecordExpression(FaceRecord faceRecord)
        {
            return FaceId == faceRecord.Id;
        }

        private BooleanValueExpression GetManaSymbolRecordExpression(ManaSymbolRecord manaSymbolRecord)
        {
            return ManaSymbolId == manaSymbolRecord.Id;
        }
    }
}