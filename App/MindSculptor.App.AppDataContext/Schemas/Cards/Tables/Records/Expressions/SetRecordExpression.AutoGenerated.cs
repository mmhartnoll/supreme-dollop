using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class SetRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Name { get; }
        public TextFieldExpression Code { get; }
        public TextFieldExpression CodeExtension { get; }
        public IntegerFieldExpression ReleaseYear { get; }
        public IntegerFieldExpression ReleaseMonth { get; }
        public IntegerFieldExpression ReleaseDay { get; }

        public SetRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Name = TextFieldExpression.Create("Name");
            Code = TextFieldExpression.Create("Code");
            CodeExtension = TextFieldExpression.Create("CodeExtension");
            ReleaseYear = IntegerFieldExpression.Create("ReleaseYear");
            ReleaseMonth = IntegerFieldExpression.Create("ReleaseMonth");
            ReleaseDay = IntegerFieldExpression.Create("ReleaseDay");
        }
    }
}