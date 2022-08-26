using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class SetInclusionRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression SetId { get; }
        public IdFieldExpression SubsetTypeId { get; }
        public IdFieldExpression BaseId { get; }
        public IdFieldExpression RarityTypeId { get; }
        public IntegerFieldExpression LogicalOrdinal { get; }
        public IntegerFieldExpression CollectorsNumber { get; }
        public ParentRecordExpression<SetRecord> SetRecord { get; }
        public ParentRecordExpression<SubsetTypeRecord> SubsetTypeRecord { get; }
        public ParentRecordExpression<BaseRecord> BaseRecord { get; }
        public ParentRecordExpression<RarityTypeRecord> RarityTypeRecord { get; }

        public SetInclusionRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            SetId = IdFieldExpression.Create("SetId");
            SubsetTypeId = IdFieldExpression.Create("SubsetTypeId");
            BaseId = IdFieldExpression.Create("BaseId");
            RarityTypeId = IdFieldExpression.Create("RarityTypeId");
            LogicalOrdinal = IntegerFieldExpression.Create("LogicalOrdinal");
            CollectorsNumber = IntegerFieldExpression.Create("CollectorsNumber");
            SetRecord = ParentRecordExpression<SetRecord>.Create(GetSetRecordExpression);
            SubsetTypeRecord = ParentRecordExpression<SubsetTypeRecord>.Create(GetSubsetTypeRecordExpression);
            BaseRecord = ParentRecordExpression<BaseRecord>.Create(GetBaseRecordExpression);
            RarityTypeRecord = ParentRecordExpression<RarityTypeRecord>.Create(GetRarityTypeRecordExpression);
        }

        private BooleanValueExpression GetSetRecordExpression(SetRecord setRecord)
        {
            return SetId == setRecord.Id;
        }

        private BooleanValueExpression GetSubsetTypeRecordExpression(SubsetTypeRecord subsetTypeRecord)
        {
            return SubsetTypeId == subsetTypeRecord.Id;
        }

        private BooleanValueExpression GetBaseRecordExpression(BaseRecord baseRecord)
        {
            return BaseId == baseRecord.Id;
        }

        private BooleanValueExpression GetRarityTypeRecordExpression(RarityTypeRecord rarityTypeRecord)
        {
            return RarityTypeId == rarityTypeRecord.Id;
        }
    }
}