using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class PlayerRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression MtgaUserId { get; }
        public TextFieldExpression Name { get; }
        public IntegerFieldExpression NameId { get; }

        public PlayerRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            MtgaUserId = TextFieldExpression.Create("MtgaUserId");
            Name = TextFieldExpression.Create("Name");
            NameId = IntegerFieldExpression.Create("NameId");
        }
    }
}