using System.Web.UI;

namespace InSite.Common.Web.UI
{
    public class UpdateProgress : System.Web.UI.UpdateProgress
    {
        private class DefaultTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                container.Controls.Add(new LoadingPanel { VisibleOnLoad = true });
            }
        }

        public UpdateProgress()
        {
            ProgressTemplate = new DefaultTemplate();
        }
    }
}