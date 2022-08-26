using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DigitalCardRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IntegerFieldExpression MtgaCardId { get; }
        public ParentRecordExpression<BasePrintingRecord> BasePrintingRecord { get; }

        public DigitalCardRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            MtgaCardId = IntegerFieldExpression.Create("MtgaCardId");
            BasePrintingRecord = ParentRecordExpression<BasePrintingRecord>.Create(GetBasePrintingRecordExpression);
        }

        private BooleanValueExpression GetBasePrintingRecordExpression(BasePrintingRecord basePrintingRecord)
        {
            return Id == basePrintingRecord.Id;
        }
    }
}