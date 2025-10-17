using System;

namespace Shift.Contract
{
    [Serializable]
    public class NavigationItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public Guid? PermissionActionIdentifier { get; set; }
    }
}