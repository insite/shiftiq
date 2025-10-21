using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [SupportsEventValidation]
    public class IconButton : Icon, IButtonControl, IPostBackEventHandler, IHasConfirmText
    {
        #region Events

        public event EventHandler Click;
        private void OnClick() => Click?.Invoke(this, EventArgs.Empty);

        public event CommandEventHandler Command;
        private void OnCommand(string commandName, object commandArgument)
        {
            if (string.IsNullOrEmpty(commandName))
                return;

            var e = new CommandEventArgs(commandName, commandArgument);

            Command?.Invoke(this, e);

            RaiseBubbleEvent(this, e);
        }

        #endregion

        #region Properties

        public bool CausesValidation
        {
            get { return ViewState[nameof(CausesValidation)] == null || (bool)ViewState[nameof(CausesValidation)]; }
            set { ViewState[nameof(CausesValidation)] = value; }
        }

        public string CommandArgument
        {
            get { return (string)ViewState[nameof(CommandArgument)]; }
            set { ViewState[nameof(CommandArgument)] = value; }
        }

        public string CommandName
        {
            get { return (string)ViewState[nameof(CommandName)]; }
            set { ViewState[nameof(CommandName)] = value; }
        }

        [UrlProperty]
        public string PostBackUrl
        {
            get { return (string)ViewState[nameof(PostBackUrl)]; }
            set { ViewState[nameof(PostBackUrl)] = value; }
        }

        public string ValidationGroup
        {
            get { return (string)ViewState[nameof(ValidationGroup)]; }
            set { ViewState[nameof(ValidationGroup)] = value; }
        }

        public string OnClientClick
        {
            get { return (string)ViewState[nameof(OnClientClick)]; }
            set { ViewState[nameof(OnClientClick)] = value; }
        }

        public string ConfirmText
        {
            get { return (string)ViewState[nameof(ConfirmText)]; }
            set { ViewState[nameof(ConfirmText)] = value; }
        }

        string IButtonControl.Text
        {
            get { return ToolTip; }
            set { ToolTip = value; }
        }

        #endregion

        #region IPostBackEventHandler

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (!Visible || !Enabled)
                return;

            Page.ClientScript.ValidateEvent(UniqueID, eventArgument);

            if (CausesValidation)
                Page.Validate(ValidationGroup);

            OnClick();
            OnCommand(CommandName, CommandArgument);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

            var script = new StringBuilder();
            var href = "javascript:void(0)";

            if (Enabled)
            {
                if (!string.IsNullOrEmpty(OnClientClick))
                    ControlHelper.AddScript(script, OnClientClick);

                if (!string.IsNullOrEmpty(ConfirmText))
                    ControlHelper.AddScript(script, string.Format("if (!confirm('{0}')) {{ return false; }}", HttpUtility.JavaScriptStringEncode(ConfirmText)));

                var postBackOptions = GetPostBackOptions();

                var postBackEventReference = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);
                if (!string.IsNullOrEmpty(postBackEventReference))
                    href = postBackEventReference;
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }

            if (script.Length > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, script.ToString());

            AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Href, href);

            writer.RenderBeginTag(HtmlTextWriterTag.A);

            RenderIcon(writer, true);

            writer.RenderEndTag();
        }

        #endregion

        #region Helpers

        private PostBackOptions GetPostBackOptions()
        {
            var options = new PostBackOptions(this, string.Empty)
            {
                RequiresJavaScriptProtocol = true
            };

            if (!string.IsNullOrEmpty(PostBackUrl))
                options.ActionUrl = HttpUtility.UrlPathEncode(ResolveUrl(PostBackUrl));

            if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
            {
                options.PerformValidation = true;
                options.ValidationGroup = ValidationGroup;
            }

            return options;
        }

        #endregion
    }
}