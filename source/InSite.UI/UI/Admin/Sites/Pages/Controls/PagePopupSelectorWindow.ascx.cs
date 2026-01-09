using System;

using InSite.Common.Web.UI;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class PagePopupSelectorWindow : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StyleLiteral.ContentKey = GetType().FullName;
        }
    }
}