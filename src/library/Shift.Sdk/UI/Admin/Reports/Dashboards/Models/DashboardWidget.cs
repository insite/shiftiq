using System;

using Shift.Common;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardWidget
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DashboardQuery Query { get; set; }

        public string Id => "Widget" + Code;

        public DashboardWidget()
        {
            Code = _generator.Next();
        }

        private static RandomStringGenerator _generator = new RandomStringGenerator(RandomStringType.Alphabetic, 6);
    }
}