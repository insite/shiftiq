using System.Web;

using InSite.Domain.Foundations;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls.Navigation
{
    public class Navigator
    {
        public Navigator(HttpRequest request)
        {
            var cmds = request.QueryString["cmds"];
            if (cmds != null && bool.TryParse(cmds, out bool result))
                IsCmds = result;
        }

        public string GetHomeUrl(ISecurityFramework identity)
        {
            return IsCmds && DefaultToCmds(identity)
                ? Urls.CmdsHomeUrl
                : DefaultToAdmin(identity)
                ? Urls.AdminHomeUrl
                : Urls.PortalHomeUrl;
        }

        public bool IsCmds
        {
            get
            {
                return CurrentSessionState.EnableCmdsNavigation;
            }
            set
            {
                CurrentSessionState.EnableCmdsNavigation = value;
            }
        }

        public bool DefaultToCmds(ISecurityFramework identity)
        {
            var isE03 = ServiceLocator.Partition.IsE03();

            return isE03 && identity.User.AccessGrantedToCmds;
        }

        public bool DefaultToAdmin(ISecurityFramework identity)
        {
            return identity.IsAdministrator;
        }
    }
}