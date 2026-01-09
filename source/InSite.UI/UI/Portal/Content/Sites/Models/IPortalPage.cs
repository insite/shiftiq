using InSite.Domain.Foundations;
using InSite.Persistence.Content;

namespace InSite.UI.Portal.Sites
{
    public interface IPortalPage
    {
        PortalPageModel Model { get; set; }
        
        void BindModelToControls(PortalPageModel model, ISecurityFramework identity);
    }
}