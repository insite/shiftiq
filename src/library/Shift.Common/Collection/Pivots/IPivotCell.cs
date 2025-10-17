namespace Shift.Common
{
    public interface IPivotCell
    {
        PivotTable Table { get; }

        MultiKey<string> RowKey { get; }

        MultiKey<string> ColumnKey { get; }

        int? Value { get; set; }

        IPivotDimensionNode GetRow();
        IPivotDimensionNode GetColumn();
    }
}
