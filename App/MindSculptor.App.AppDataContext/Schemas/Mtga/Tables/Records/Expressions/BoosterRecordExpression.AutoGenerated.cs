using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class BoosterRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression SetId { get; }
        public IntegerFieldExpression MtgaBoosterId { get; }
        public ParentRecordExpression<SetRecord> SetRecord { get; }

        public BoosterRecordExpression()
        {
            SetId = IdFieldExpression.Create("SetId");
            MtgaBoosterId = IntegerFieldExpression.Create("MtgaBoosterId");
            SetRecord = ParentRecordExpression<SetRecord>.Create(GetSetRecordExpression);
        }

        private BooleanValueExpression GetSetRecordExpression(SetRecord setRecord)
        {
            return SetId == setRecord.Id;
        }
    }
}