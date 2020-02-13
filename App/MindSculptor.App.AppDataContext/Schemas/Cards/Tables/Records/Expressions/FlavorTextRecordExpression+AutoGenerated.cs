using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FlavorTextRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression FacePrintingId { get; }
        public TextFieldExpression Value { get; }
        public ParentRecordExpression<FacePrintingRecord> FacePrintingRecord { get; }

        public FlavorTextRecordExpression()
        {
            FacePrintingId = IdFieldExpression.Create("FacePrintingId");
            Value = TextFieldExpression.Create("Value");
            FacePrintingRecord = ParentRecordExpression<FacePrintingRecord>.Create(GetFacePrintingRecordExpression);
        }

        private BooleanValueExpression GetFacePrintingRecordExpression(FacePrintingRecord facePrintingRecord)
        {
            return FacePrintingId == facePrintingRecord.Id;
        }
    }
}