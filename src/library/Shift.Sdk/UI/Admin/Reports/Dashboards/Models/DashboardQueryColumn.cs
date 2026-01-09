using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardQueryColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Sort { get; set; }

        public DashboardQueryColumnLink Link { get; set; }
    }
}