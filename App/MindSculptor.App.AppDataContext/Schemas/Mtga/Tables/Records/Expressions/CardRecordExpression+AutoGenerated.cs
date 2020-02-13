using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class CardRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IntegerFieldExpression MtgaCardId { get; }
        public ParentRecordExpression<BasePrintingRecord> BasePrintingRecord { get; }

        public CardRecordExpression()
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