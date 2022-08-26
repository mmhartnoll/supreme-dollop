using MindSculptor.DataAccess.Context.Query;
using MindSculptor.DataAccess.Context.Query.Expressions;
using MindSculptor.DataAccess.Context.Query.Expressions.Fields;
using MindSculptor.DataAccess.Context.Query.Expressions.Logical;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;

namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseTable<TRecord, TExpression> : DatabaseTable<TRecord>, IEnumerable<TRecord>, IAsyncEnumerable<TRecord>
        where TRecord : DatabaseRecord
        where TExpression : DatabaseRecordExpression, new()
    {
        protected DatabaseContext Context { get; }

        protected DatabaseTable(DatabaseContext dataContext, string schemaName, string name) : base(schemaName, name)
            => Context = dataContext;

        public RecordQuery<TRecord, TExpression> QueryWhere(Func<TExpression, BooleanValueExpression> expression)
            => NewRecordQuery().QueryWhere(expression);

        public RecordQuery<TRecord, TExpression> OrderBy(Func<TExpression, FieldExpression> expression, SortDirection sortDirection = SortDirection.Default)
            => NewRecordQuery().OrderBy(expression, sortDirection);

        protected RecordQuery<TRecord, TExpression> NewRecordQuery()
            => RecordQuery<TRecord, TExpression>.Create(Context, this, MapRecordFromDataReader);

        protected abstract TRecord MapRecordFromDataReader(DbDataReader dataReader);

        public IEnumerator<TRecord> GetEnumerator()
            => NewRecordQuery().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IAsyncEnumerator<TRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => NewRecordQuery().GetAsyncEnumerator(cancellationToken);
    }
}
