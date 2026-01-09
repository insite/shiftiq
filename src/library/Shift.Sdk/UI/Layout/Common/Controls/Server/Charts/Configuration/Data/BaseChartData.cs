using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public interface IChartData
    {
        bool IsEmpty { get; }

        void Clear();
        IChartDataset CreateDataset(string id);
        IReadOnlyList<IChartDataset> GetDatasets();
        IChartDataset GetDataset(int index);
        IChartDataset GetDataset(string id);
    }
}
