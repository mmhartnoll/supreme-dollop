using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class ArtistRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Name { get; }

        public ArtistRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Name = TextFieldExpression.Create("Name");
        }
    }
}