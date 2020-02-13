using MindSculptor.App.AppDataContext.Schemas.Accounts.Tables.Records;
using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ProfileRecordExpression : DataContextRecordExpression
    {
        public IdFieldExpression Id { get; }
        public IdFieldExpression AccountId { get; }
        public TextFieldExpression ScreenName { get; }
        public IntegerFieldExpression UserId { get; }
        public ParentRecordExpression<AccountRecord> AccountRecord { get; }

        public ProfileRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            AccountId = IdFieldExpression.Create("AccountId");
            ScreenName = TextFieldExpression.Create("ScreenName");
            UserId = IntegerFieldExpression.Create("UserId");
            AccountRecord = ParentRecordExpression<AccountRecord>.Create(GetAccountRecordExpression);
        }

        private BooleanValueExpression GetAccountRecordExpression(AccountRecord accountRecord)
        {
            return AccountId == accountRecord.Id;
        }
    }
}