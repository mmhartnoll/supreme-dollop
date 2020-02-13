using MindSculptor.DataAccess.DataContext.Query;
using MindSculptor.DataAccess.DataContext.Query.Expressions;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Fields;
using MindSculptor.DataAccess.DataContext.Query.Expressions.Logical;
using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.DataAccess.DataContext
{
    public abstract class DataContextTable<TRecord, TExpression> : DataContextSchemaEntity, IEnumerable<TRecord>, IAsyncEnumerable<TRecord>
        where TRecord : DataContextRecord
        where TExpression : DataContextRecordExpression, new()
    {
        public DataContext DataContext { get; }

        protected DataContextTable(DataContext dataContext, string schemaName, string name)
            : base(schemaName, name)
        {
            DataContext = dataContext;
        }

        public RecordQuery<TRecord, TExpression> QueryWhere(Func<TExpression, BooleanValueExpression> expression)
            => NewRecordQuery().QueryWhere(expression);

        public RecordQuery<TRecord, TExpression> OrderBy(Func<TExpression, FieldExpression> expression, SortDirection sortDirection = SortDirection.Default)
            => NewRecordQuery().OrderBy(expression, sortDirection);

        protected RecordQuery<TRecord, TExpression> NewRecordQuery()
            => RecordQuery<TRecord, TExpression>.Create(DataContext, this, MapRecordFromDataReader);

        protected abstract TRecord MapRecordFromDataReader(DbDataReader dataReader);

        public IEnumerator<TRecord> GetEnumerator()
            => NewRecordQuery().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IAsyncEnumerator<TRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => NewRecordQuery().GetAsyncEnumerator(cancellationToken);
    }
}
