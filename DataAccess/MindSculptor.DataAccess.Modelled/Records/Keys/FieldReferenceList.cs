using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public class FieldReferenceList : IEnumerable<FieldReference>
    {
        private readonly IEnumerable<FieldReference> fieldList;

        internal FieldReferenceList(IEnumerable<FieldReference> fieldList)
            => this.fieldList = fieldList;

        public static implicit operator FieldReferenceList(Field field)
            => new FieldReferenceList(new FieldReference(field, IndexSortDirection.Default).ToEnumerable());

        public static implicit operator FieldReferenceList((Field Field, IndexSortDirection SortDirection) fieldSortingPair)
            => new FieldReferenceList(new FieldReference(fieldSortingPair.Field, fieldSortingPair.SortDirection).ToEnumerable());

        public IEnumerator<FieldReference> GetEnumerator()
            => fieldList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => fieldList.GetEnumerator();
    }
}
