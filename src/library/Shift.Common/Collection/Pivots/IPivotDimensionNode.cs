using System.Collections.Generic;

namespace Shift.Common
{
    public interface IPivotDimensionNode : IEnumerable<IPivotDimensionNode>
    {
        PivotTable Table { get; }

        IPivotDimensionNode Parent { get; }

        IPivotDimensionNode this[string unit] { get; }

        IPivotDimensionNode this[int index] { get; }

        string Dimension { get; }

        string Unit { get; }

        bool IsIndex { get; }

        bool IsRoot { get; }

        IPivotDimensionNode[] GetIndexes();
        IPivotCell GetCell(IPivotDimensionNode index);
        IPivotCell GetCell(MultiKey<string> key);
        IPivotCell[] GetCells();

        MultiKey<string> GetKey();
    }
}
