using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class PieChartDatasetCollection : BaseChartDatasetCollection<PieChartDatasetConfig>
    {
        protected override PieChartDatasetConfig CreateDatasetInstance(string id) => new PieChartDatasetConfig(id);
    }
}
