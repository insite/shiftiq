using System.Web.UI;

namespace InSite.Web.Optimization
{
    public class DefaultPageAdapter : System.Web.UI.Adapters.PageAdapter
    {
        public override PageStatePersister GetStatePersister()
        {
            return new CachePageStatePersister(Page);
        }
    }
}