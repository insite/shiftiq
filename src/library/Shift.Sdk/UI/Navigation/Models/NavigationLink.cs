using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI.Navigation
{
    public class NavigationLink
    {
        public string Resource { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Href { get; set; }
        public List<NavigationLink> Links { get; set; }
        public NavigationSecurity Security { get; set; }

        public NavigationLink Clone(List<NavigationLink> links = null) => new NavigationLink
        {
            Resource = Resource,
            Icon = Icon,
            Text = Text,
            Href = Href,
            Links = (links ?? Links)?.Select(x => x.Clone()).ToList(),
            Security = Security?.Clone(),
        };
    }
}