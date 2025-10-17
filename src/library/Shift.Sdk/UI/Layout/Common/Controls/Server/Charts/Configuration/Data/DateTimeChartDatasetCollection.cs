using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DateTimeChartDatasetCollection : BaseChartDatasetCollection<DateTimeChartDatasetConfig>
    {
        protected override DateTimeChartDatasetConfig CreateDatasetInstance(string id) => new DateTimeChartDatasetConfig(id);
    }
}
