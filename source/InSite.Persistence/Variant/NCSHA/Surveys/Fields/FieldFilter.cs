using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class FieldFilter : Shift.Common.Filter
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool? IsNumeric { get; set; }
    }
}
