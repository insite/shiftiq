using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardModel
    {
        public string Error { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public List<DashboardPanel> Panels { get; set; }
    }
}