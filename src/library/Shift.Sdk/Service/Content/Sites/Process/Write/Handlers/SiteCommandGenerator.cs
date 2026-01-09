using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Sites.Read;
using InSite.Domain.Sites.Sites;

namespace InSite.Application.Sites.Write
{
    public class SiteCommandGenerator
    {
        /// <summary>
        /// Returns the list of commands to create a Site.
        /// </summary>
        public List<ICommand> GetCommands(SiteState site)
        {
            var script = new List<ICommand>
            {
                new CreateSite(site.Identifier, site.Tenant, site.Author, null, site.Domain, site.Title, null)
            };

            return script;
        }

        public List<ICommand> GetCommands(QSite site)
        {
            var script = new List<ICommand>
            {
                new CreateSite(site.SiteIdentifier, site.OrganizationIdentifier,System.Guid.Empty, "" , site.SiteDomain, site.SiteTitle, null)
            };

            return script;
        }

        public ICommand[] GetDifferenceCommands(SiteState original, SiteState changed)
        {
            var script = new List<ICommand>();

            if (original.Title != changed.Title)
                script.Add(new ChangeSiteTitle(original.Identifier, changed.Title));

            if (original.Domain != changed.Domain)
                script.Add(new ChangeSiteDomain(original.Identifier, changed.Domain));

            return script.ToArray();
        }
    }
}
