using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class FaceRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression BaseId { get; }
        public TextFieldExpression Name { get; }
        public BooleanFieldExpression IsPrimaryFace { get; }
        public ParentRecordExpression<BaseRecord> BaseRecord { get; }

        public FaceRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            BaseId = IdFieldExpression.Create("BaseId");
            Name = TextFieldExpression.Create("Name");
            IsPrimaryFace = BooleanFieldExpression.Create("IsPrimaryFace");
            BaseRecord = ParentRecordExpression<BaseRecord>.Create(GetBaseRecordExpression);
        }

        private BooleanValueExpression GetBaseRecordExpression(BaseRecord baseRecord)
        {
            return BaseId == baseRecord.Id;
        }
    }
}