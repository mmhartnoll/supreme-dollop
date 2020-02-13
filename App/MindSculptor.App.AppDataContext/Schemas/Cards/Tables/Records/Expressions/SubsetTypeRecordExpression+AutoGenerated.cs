using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class SubsetTypeRecordExpression : DataContextRecordExpression
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