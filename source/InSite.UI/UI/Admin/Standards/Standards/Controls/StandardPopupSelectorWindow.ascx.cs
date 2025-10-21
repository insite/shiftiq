using System;
using System.Web.UI;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class StandardPopupSelectorWindow : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            StyleLiteral.ContentKey = GetType().FullName;

            base.OnInit(e);
        }
    }
}