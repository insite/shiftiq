using System.Drawing;

namespace Shift.Sdk.UI
{
    public interface IChartDatasetColoredItem : IChartDatasetLabeledItem
    {
        Color BackgroundColor { get; set; }
        Color BorderColor { get; set; }
    }
}
