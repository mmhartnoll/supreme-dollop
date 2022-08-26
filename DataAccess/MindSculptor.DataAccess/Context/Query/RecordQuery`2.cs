using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.DataAccess.Context.Query
{
    public class RecordQuery<TRecord, TExpression> : IEnumerable<TRecord>, IAsyncEnumerable<TRecord>
        where TRecord : DatabaseRecord
        where TExpression : DatabaseRecordExpression, new()
    {
        private readonly DatabaseContext context;
        private readonly Func<DbDataReader, TRecord> mappingFunction;

        internal SelectExpression SelectExpression { get; }

        private RecordQuery(DatabaseContext context, DatabaseSchemaEntity queryableEntity, Func<DbDataReader, TRecord> mappingFunction)
        {
            this.context = context;
            this.mappingFunction = mappingFunction;
            SelectExpression = SelectExpression.Create(queryableEntity);
        }

        internal static RecordQuery<TRecord, TExpression> Create(DatabaseContext dataContext, DatabaseSchemaEntity queryableEntity, Func<DbDataReader, TRecord> mappingFunction)
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

        private void PrepareCommand(DbCommand command)
        {
            var parameterProvider = new ParameterProvider();
            SelectExpression.ResolveParameters(parameterProvider);

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
        }

        private IEnumerable<TRecord> EnumerateRecords()
        {
            return context.Enumerate(EnumerateRecords);

            IEnumerable<TRecord> EnumerateRecords(DbCommand command)
            {
                PrepareCommand(command);
                using var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                    yield return mappingFunction.Invoke(dataReader);
            }
        }

        private async IAsyncEnumerable<TRecord> StreamRecordsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var stream = context.StreamAsync(StreamRecordsAsync, cancellationToken);
            await foreach (var record in stream.ConfigureAwait(false))
                yield return record;

            async IAsyncEnumerable<TRecord> StreamRecordsAsync(DbCommand command, CancellationToken cancellationToken = default)
            {
                PrepareCommand(command);
                using var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                while (await dataReader.ReadAsync().ConfigureAwait(false))
                    yield return mappingFunction.Invoke(dataReader);
            }
        }

        public IEnumerator<TRecord> GetEnumerator()
            => EnumerateRecords().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => EnumerateRecords().GetEnumerator();

        public IAsyncEnumerator<TRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => StreamRecordsAsync().GetAsyncEnumerator(cancellationToken);
    }
}
