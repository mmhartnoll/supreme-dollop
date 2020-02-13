using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class BasePrintingRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression SetInclusionId { get; }
        public IdFieldExpression PrintingTypeId { get; }
        public ParentRecordExpression<SetInclusionRecord> SetInclusionRecord { get; }
        public ParentRecordExpression<PrintingTypeRecord> PrintingTypeRecord { get; }

        public BasePrintingRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            SetInclusionId = IdFieldExpression.Create("SetInclusionId");
            PrintingTypeId = IdFieldExpression.Create("PrintingTypeId");
            SetInclusionRecord = ParentRecordExpression<SetInclusionRecord>.Create(GetSetInclusionRecordExpression);
            PrintingTypeRecord = ParentRecordExpression<PrintingTypeRecord>.Create(GetPrintingTypeRecordExpression);
        }

        private BooleanValueExpression GetSetInclusionRecordExpression(SetInclusionRecord setInclusionRecord)
        {
            return SetInclusionId == setInclusionRecord.Id;
        }

        private BooleanValueExpression GetPrintingTypeRecordExpression(PrintingTypeRecord printingTypeRecord)
        {
            return PrintingTypeId == printingTypeRecord.Id;
        }
    }
}