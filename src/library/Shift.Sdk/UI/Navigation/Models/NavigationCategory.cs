using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI.Navigation
{
    public class NavigationCategory
    {
        public string Category { get; set; }
        public List<NavigationLink> Links { get; set; }
        public NavigationSecurity Security { get; set; }

        public NavigationCategory Clone() => new NavigationCategory
        {
            Category = this.Category,
            Links = this.Links?.Select(x => x.Clone()).ToList()
        };
    }
}