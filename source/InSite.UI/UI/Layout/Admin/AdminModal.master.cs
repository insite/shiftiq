using System.Web.UI;

using InSite.Domain.Foundations;

namespace InSite.UI.Layout.Admin
{
    public partial class AdminModal : MasterPage
    {
        protected ISecurityFramework Identity => CurrentSessionState.Identity;
    }
}