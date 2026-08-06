namespace Shift.Sdk.UI
{
    public interface IChartDataset
    {
        string Label { get; set; }

        IChartDatasetItem NewItem();
    }
}
