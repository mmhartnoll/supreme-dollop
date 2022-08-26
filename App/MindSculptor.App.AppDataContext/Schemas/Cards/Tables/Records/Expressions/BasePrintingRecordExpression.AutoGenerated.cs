using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class BasePrintingRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression SetInclusionId { get; }
        public IdFieldExpression PrintingTypeId { get; }
        public IdFieldExpression ArtistId { get; }
        public ParentRecordExpression<SetInclusionRecord> SetInclusionRecord { get; }
        public ParentRecordExpression<PrintingTypeRecord> PrintingTypeRecord { get; }
        public ParentRecordExpression<ArtistRecord> ArtistRecord { get; }

        public BasePrintingRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            SetInclusionId = IdFieldExpression.Create("SetInclusionId");
            PrintingTypeId = IdFieldExpression.Create("PrintingTypeId");
            ArtistId = IdFieldExpression.Create("ArtistId");
            SetInclusionRecord = ParentRecordExpression<SetInclusionRecord>.Create(GetSetInclusionRecordExpression);
            PrintingTypeRecord = ParentRecordExpression<PrintingTypeRecord>.Create(GetPrintingTypeRecordExpression);
            ArtistRecord = ParentRecordExpression<ArtistRecord>.Create(GetArtistRecordExpression);
        }

        private BooleanValueExpression GetSetInclusionRecordExpression(SetInclusionRecord setInclusionRecord)
        {
            return SetInclusionId == setInclusionRecord.Id;
        }

        private BooleanValueExpression GetPrintingTypeRecordExpression(PrintingTypeRecord printingTypeRecord)
        {
            return PrintingTypeId == printingTypeRecord.Id;
        }

        private BooleanValueExpression GetArtistRecordExpression(ArtistRecord artistRecord)
        {
            return ArtistId == artistRecord.Id;
        }
    }
}