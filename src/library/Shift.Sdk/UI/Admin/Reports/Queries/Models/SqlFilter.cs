using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class SqlFilter : Filter
    {
        public string Query{ get; set; }
    }
}