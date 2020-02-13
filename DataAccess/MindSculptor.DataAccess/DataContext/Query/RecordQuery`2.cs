using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace MindSculptor.DataAccess.DataContext.Query
{
    public class RecordQuery<TRecord, TExpression> : IEnumerable<TRecord>, IAsyncEnumerable<TRecord>
        where TRecord : DataContextRecord
        where TExpression : DataContextRecordExpression, new()
    {
        private readonly DataContext dataContext;
        private readonly Func<DbDataReader, TRecord> mappingFunction;

        internal SelectExpression SelectExpression { get; }

        private RecordQuery(DataContext dataContext, DataContextSchemaEntity queryableEntity, Func<DbDataReader, TRecord> mappingFunction)
        {
            this.dataContext = dataContext;
            this.mappingFunction = mappingFunction;
            SelectExpression = SelectExpression.Create(queryableEntity);
        }

        internal static RecordQuery<TRecord, TExpression> Create(DataContext dataContext, DataContextSchemaEntity queryableEntity, Func<DbDataReader, TRecord> mappingFunction)
            => new RecordQuery<TRecord, TExpression>(dataContext, queryableEntity, mappingFunction);

        public RecordQuery<TRecord, TExpression> QueryWhere(Func<TExpression, BooleanValueExpression> expression)
        {
            SelectExpression.AddLogicalClause(expression.Invoke(new TExpression()));
            return this;
        }

        public RecordQuery<TRecord, TExpression> OrderBy(Func<TExpression, FieldExpression> expression, SortDirection sortDirection = SortDirection.Default)
        {
            SelectExpression.AddOrdering(expression.Invoke(new TExpression()), sortDirection);
            return this;
        }

        private DbCommand GetQueryCommand()
        {
            var parameterProvider = ParameterProvider.Create();
            SelectExpression.ResolveParameters(parameterProvider);

            var command = dataContext.Connection.CreateCommand();
            if (dataContext.HasTransaction)
                command.Transaction = dataContext.Transaction;
            command.CommandText = SelectExpression.ExpressionString;
            var parameters = parameterProvider.Parameters.Select(
                parameterKeyValue =>
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterKeyValue.Key;
                    parameter.Value = parameterKeyValue.Value ?? DBNull.Value;
                    return parameter;
                });
            foreach (var parameter in parameters)
                command.Parameters.Add(parameter);
            return command;
        }

        private IEnumerable<TRecord> EnumerateRecords()
        {
            using (var command = GetQueryCommand())
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    yield return mappingFunction.Invoke(dataReader);
        }

        private async IAsyncEnumerable<TRecord> StreamRecordsAsync()
        {
            using (var command = GetQueryCommand())
            using (var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                while (await dataReader.ReadAsync().ConfigureAwait(false))
                    yield return mappingFunction.Invoke(dataReader);
        }

        public IEnumerator<TRecord> GetEnumerator()
            => EnumerateRecords().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => EnumerateRecords().GetEnumerator();

        public IAsyncEnumerator<TRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => StreamRecordsAsync().GetAsyncEnumerator(cancellationToken);
    }
}
