using System;
using System.Web.UI;

using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class JsonEditor : BaseUserControl
    {
        public string Value
        {
            get => Input.Text;
            set => Input.Text = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(JsonEditor).FullName;
            CommonScript.ContentKey = typeof(JsonEditor).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(JsonEditor),
                "init",
                $"reportJsonEditor.init('{Input.ClientID}');",
                true);
        }
    }
}