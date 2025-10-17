using System;

namespace Shift.Sdk.UI
{
    public class ActivityRepeaterItem
    {
        public Guid ActivityIdentifier { get; set; }
        public string ActivityClass { get; set; }
        public string ActivityUrl { get; set; }
        public string ActivityIcon { get; set; }
        public string ActivityName { get; set; }
        public bool IsActive { get; set; }
        public bool IsHidden { get; set; }
    }
}