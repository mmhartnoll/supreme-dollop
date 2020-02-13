using MindSculptor.DataAccess.Modelled.Records.Fields;

namespace MindSculptor.DataAccess.Modelled.Records.Keys
{
    public class FieldReference
    {
        public Field Field { get; }
        public IndexSortDirection SortDirection { get; }

        internal FieldReference(Field field, IndexSortDirection sortDirection)
        {
            Field = field;
            SortDirection = sortDirection;
        }

        public static implicit operator FieldReference(Field field)
            => new FieldReference(field, IndexSortDirection.Default);

        public static implicit operator FieldReference((Field Field, IndexSortDirection SortDirection) fieldSortingPair)
            => new FieldReference(fieldSortingPair.Field, fieldSortingPair.SortDirection);

        public static FieldReferenceList List(params FieldReference[] fieldReferences)
            => new FieldReferenceList(fieldReferences);
    }
}
