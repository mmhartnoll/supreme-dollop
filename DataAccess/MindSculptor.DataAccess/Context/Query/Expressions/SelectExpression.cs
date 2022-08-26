using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindSculptor.DataAccess.Context.Query.Expressions
{
    public class SelectExpression : ExpressionComponent
    {
        private readonly IDatabaseContextQueryable queryable;
        private readonly LogicalExpression logicalExpression;
        private readonly List<OrderByClause> orderByClauses;

        private SelectExpression(IDatabaseContextQueryable queryable)
        {
            this.queryable = queryable;
            logicalExpression = new LogicalAndExpression();
            orderByClauses = new List<OrderByClause>();
        }

        public static SelectExpression Create(IDatabaseContextQueryable queryable)
            => new SelectExpression(queryable);

        public void AddLogicalClause(BooleanValueExpression expression)
            => logicalExpression.AddExpression(expression);

        public void AddOrdering(FieldExpression fieldExpression, SortDirection sortDirection)
            => orderByClauses.Add(OrderByClause.Create(fieldExpression, sortDirection));

        protected override string GetExpressionString()
        {
            var queryString = new StringBuilder();
            queryString.Append("SELECT * FROM ");
            queryString.Append(queryable.IdentifierString);

            var logicalExpression = this.logicalExpression.ToString();
            if (logicalExpression != string.Empty)
            {
                queryString.Append(" WHERE ");
                queryString.Append(logicalExpression);
            }

            if (orderByClauses.Any())
            {
                queryString.Append(" ORDER BY ");
                queryString.Append(string.Join(
                    ", ",
                    orderByClauses.Select(orderByClause => string.Join(
                        " ",
                        orderByClause.FieldName,
                        orderByClause.SortDirection switch
                        {
                            SortDirection.Ascending => "ASC",
                            SortDirection.Descending => "DESC",
                            _ => throw new NotSupportedException($"'{nameof(SortDirection)}' value of '{orderByClause.SortDirection}' is not supported.")
                        }))));
            }

            queryString.Append(";");
            return queryString.ToString();
        }

        internal override void ResolveParameters(ParameterProvider parameterProvider)
            => logicalExpression.ResolveParameters(parameterProvider);

        private class OrderByClause
        {
            public string FieldName { get; }
            public SortDirection SortDirection { get; }

            private OrderByClause(FieldExpression fieldExpression, SortDirection sortDirection)
            {
                FieldName = fieldExpression.FieldName;
                SortDirection = sortDirection;
            }

            public static OrderByClause Create(FieldExpression fieldExpression, SortDirection sortDirection)
                => new OrderByClause(fieldExpression, sortDirection);
        }
    }
}
