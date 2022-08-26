using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using MindSculptor.DataAccess.Context.Query.Expressions.Relational;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions
{
    public class ProfileRecordExpression : DatabaseRecordExpression
    {
        public IdFieldExpression Id { get; }
        public ParentRecordExpression<PlayerRecord> PlayerRecord { get; }

        public ProfileRecordExpression()
        {
            Id = IdFieldExpression.Create("Id");
            PlayerRecord = ParentRecordExpression<PlayerRecord>.Create(GetPlayerRecordExpression);
        }

        private BooleanValueExpression GetPlayerRecordExpression(PlayerRecord playerRecord)
        {
            return Id == playerRecord.Id;
        }
    }
}