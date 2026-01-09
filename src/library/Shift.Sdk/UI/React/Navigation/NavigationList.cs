using System;

namespace Shift.Contract
{
    [Serializable]
    public class NavigationList
    {
        public string Title { get; set; }
        public NavigationItem[] MenuItems { get; set; }
    }
}