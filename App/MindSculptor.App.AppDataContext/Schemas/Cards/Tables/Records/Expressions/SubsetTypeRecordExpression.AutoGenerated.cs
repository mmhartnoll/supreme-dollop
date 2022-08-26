using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class SubsetTypeRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Value { get; }
        public TextFieldExpression CollectorsNumberFormat { get; }

        public SubsetTypeRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Value = TextFieldExpression.Create("Value");
            CollectorsNumberFormat = TextFieldExpression.Create("CollectorsNumberFormat");
        }
    }
}