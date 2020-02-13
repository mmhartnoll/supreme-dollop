using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;

namespace MindSculptor.App.AppDataContext.Schemas.Accounts.Tables.Records.Expressions
{
    public class AccountRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public TextFieldExpression EmailAddress { get; }

        public AccountRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            EmailAddress = TextFieldExpression.Create("EmailAddress");
        }
    }
}