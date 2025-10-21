using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Foundations;
using InSite.Persistence;

namespace InSite.UI.Layout.Common.Controls.Navigation.Models
{
    public class NavigationLink
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Href { get; set; }
        public List<NavigationLink> Links { get; set; }
        public NavigationSecurity Security { get; set; }

        public bool IsAccessPermitted(ISecurityFramework identity)
        {
            var action = TActionSearch.Get(Href);
            var permissionId = action?.PermissionParentActionIdentifier;

            return (!permissionId.HasValue || identity.IsGranted(permissionId.Value))
                && (Security == null || Security.Evaluate(identity));
        }

        public NavigationLink Clone() => new NavigationLink
        {
            Icon = this.Icon,
            Text = this.Text,
            Href = this.Href,
            Links = this.Links?.Select(x => x.Clone()).ToList(),
            Security = Security?.Clone()
        };
    }
}