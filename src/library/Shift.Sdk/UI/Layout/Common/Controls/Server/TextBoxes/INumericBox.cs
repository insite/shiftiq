using System;

namespace Shift.Sdk.UI
{
    public interface INumericBox
    {
        event EventHandler ValueChanged;

        int? ValueAsInt { get; set; }
        decimal? ValueAsDecimal { get; set; }
        string ValueAsText { get; set; }
        bool Visible { get; set; }
        bool Enabled { get; set; }
    }
}
