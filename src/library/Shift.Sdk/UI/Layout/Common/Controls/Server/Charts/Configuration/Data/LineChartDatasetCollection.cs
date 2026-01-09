using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class LineChartDatasetCollection : BaseChartDatasetCollection<LineChartDatasetConfig>
    {
        protected override LineChartDatasetConfig CreateDatasetInstance(string id) => new LineChartDatasetConfig(id);
    }
}
