using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions
{
    public class ManaSymbolRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression Type { get; }
        public TextFieldExpression Code { get; }
        public IntegerFieldExpression ConvertedManaCost { get; }
        public BooleanFieldExpression HasWhiteIdentity { get; }
        public BooleanFieldExpression HasBlueIdentity { get; }
        public BooleanFieldExpression HasBlackIdentity { get; }
        public BooleanFieldExpression HasRedIdentity { get; }
        public BooleanFieldExpression HasGreenIdentity { get; }
        public BooleanFieldExpression HasColorlessIdentity { get; }

        public ManaSymbolRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            Type = TextFieldExpression.Create("Type");
            Code = TextFieldExpression.Create("Code");
            ConvertedManaCost = IntegerFieldExpression.Create("ConvertedManaCost");
            HasWhiteIdentity = BooleanFieldExpression.Create("HasWhiteIdentity");
            HasBlueIdentity = BooleanFieldExpression.Create("HasBlueIdentity");
            HasBlackIdentity = BooleanFieldExpression.Create("HasBlackIdentity");
            HasRedIdentity = BooleanFieldExpression.Create("HasRedIdentity");
            HasGreenIdentity = BooleanFieldExpression.Create("HasGreenIdentity");
            HasColorlessIdentity = BooleanFieldExpression.Create("HasColorlessIdentity");
        }
    }
}