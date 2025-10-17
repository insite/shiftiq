using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class BarChartDatasetCollection : BaseChartDatasetCollection<BarChartDatasetConfig>
    {
        protected override BarChartDatasetConfig CreateDatasetInstance(string id) => new BarChartDatasetConfig(id);
    }
}