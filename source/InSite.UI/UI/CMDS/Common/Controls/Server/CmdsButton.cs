using System;
using System.Web.UI.WebControls;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class CmdsButton : LinkButton
    {
        #region Properties

        public String NavigateUrl { get; set; }

        public override bool SupportsDisabledAttribute => true;

        #endregion

        #region Construction

        public CmdsButton()
        {
            CssClass = "btn btn-default";
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Click += Button_Click;
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            RegisterScript();

            if (!Text.StartsWith("<span>", StringComparison.CurrentCulture))
                Text = string.Format("<span>{0}</span>", Text);

            if (String.IsNullOrEmpty(OnClientClick))
                OnClientClick = string.Format("return submitLinkButton(this)");
        }

        private void RegisterScript()
        {
            const String script = @"
function submitLinkButton(btn) {
  if (btn.getAttribute('disabled') == 'true')
    return false;

  var code = btn.href;
  if (code != null && code.length > 0) {
    if (code.indexOf('%20') >= 0)
      code = decodeURI(code);

    eval(code);
  }

  if (typeof Page_IsValid == 'undefined' || Page_IsValid)
    btn.setAttribute('disabled', 'true');

  return false;
}";

            Page.ClientScript.RegisterClientScriptBlock(typeof(CmdsButton), "init", script, true);
        }

        #endregion

        #region Event handlers

        private void Button_Click(Object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(NavigateUrl))
                Page.Response.Redirect(NavigateUrl);
        }

        #endregion
    }
}