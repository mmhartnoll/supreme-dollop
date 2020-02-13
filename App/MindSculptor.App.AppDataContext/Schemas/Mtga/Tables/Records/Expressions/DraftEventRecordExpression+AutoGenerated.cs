using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class DraftEventRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression SetId { get; }
        public TextFieldExpression DraftType { get; }
        public ParentRecordExpression<SetRecord> SetRecord { get; }

        public DraftEventRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            SetId = IdFieldExpression.Create("SetId");
            DraftType = TextFieldExpression.Create("DraftType");
            SetRecord = ParentRecordExpression<SetRecord>.Create(GetSetRecordExpression);
        }

        private BooleanValueExpression GetSetRecordExpression(SetRecord setRecord)
        {
            return SetId == setRecord.Id;
        }
    }
}