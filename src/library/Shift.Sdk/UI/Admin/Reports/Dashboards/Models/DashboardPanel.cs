using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardPanel
    {
        public int Size { get; set; }
        public List<DashboardWidget> Widgets { get; set; }
    }
}