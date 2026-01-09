using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Domain.Foundations;

namespace InSite.UI.Layout.Common.Controls.Navigation.Models
{
    public class NavigationCategory
    {
        public string Category { get; set; }
        public List<NavigationLink> Links { get; set; }
        public NavigationSecurity Security { get; set; }

        public bool IsAccessPermitted(ISecurityFramework identity)
        {
            return Security == null || Security.Evaluate(identity);
        }

        public NavigationCategory Clone() => new NavigationCategory
        {
            Category = this.Category,
            Links = this.Links?.Select(x => x.Clone()).ToList()
        };
    }
}