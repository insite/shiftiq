using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class IconInternal : WebControl
    {
        #region Properties

        public virtual String AlternateText
        {
            get { return (String)ViewState[nameof(AlternateText)]; }
            set { ViewState[nameof(AlternateText)] = value; }
        }

        public ImageAlign ImageAlign
        {
            get { return ViewState[nameof(ImageAlign)] == null ? ImageAlign.AbsMiddle : (ImageAlign)ViewState[nameof(ImageAlign)]; }
            set { ViewState[nameof(ImageAlign)] = value; }
        }

        [UrlProperty]
        public virtual String ImageUrl
        {
            get { return (String)ViewState[nameof(ImageUrl)]; }
            set { ViewState[nameof(ImageUrl)] = value; }
        }

        public Boolean IsFontIcon { get; set; }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

            RenderIconImage(true, writer);
        }

        protected void RenderIconImage(Boolean addAttributes, HtmlTextWriter writer)
        {
            var cssClass = string.Empty; // "icon";

            if (!String.IsNullOrEmpty(CssClass))
                cssClass += IsFontIcon ? " fa fa-" + CssClass : " " + CssClass;

            if (ForeColor != null)
            {
                if (ForeColor == System.Drawing.Color.Red)
                    cssClass += " text-danger";
                else if (ForeColor == System.Drawing.Color.Green)
                    cssClass += " text-success";
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            if (!String.IsNullOrEmpty(ToolTip))
                writer.AddAttribute(HtmlTextWriterAttribute.Title, ToolTip);

            if (!IsFontIcon)
            {
                var imageAlign = GetImageAlignValue(ImageAlign);
                if (!String.IsNullOrEmpty(imageAlign))
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, imageAlign);

                writer.AddAttribute(HtmlTextWriterAttribute.Alt, !String.IsNullOrEmpty(AlternateText) ? AlternateText : String.Empty);

                var imageUrl = ImageUrl;
                if (!String.IsNullOrEmpty(imageUrl))
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl(imageUrl));
            }

            if (addAttributes)
                AddAttributesToRender(writer);

            writer.RenderBeginTag(IsFontIcon ? "i" : "img");
            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        protected static String GetImageAlignValue(ImageAlign value)
        {
            String result = String.Empty;

            if (value != ImageAlign.NotSet)
            {
                switch (value)
                {
                    case ImageAlign.Left:
                        result = "left";
                        break;
                    case ImageAlign.Right:
                        result = "right";
                        break;
                    case ImageAlign.Baseline:
                        result = "baseline";
                        break;
                    case ImageAlign.Top:
                        result = "top";
                        break;
                    case ImageAlign.Middle:
                        result = "middle";
                        break;
                    case ImageAlign.Bottom:
                        result = "bottom";
                        break;
                    case ImageAlign.AbsBottom:
                        result = "absbottom";
                        break;
                    case ImageAlign.AbsMiddle:
                        result = "absmiddle";
                        break;
                    default:
                        result = "texttop";
                        break;
                }
            }

            return result;
        }

        #endregion
    }

    [SupportsEventValidation]
    public class IconButton : IconInternal, IButtonControl, IPostBackEventHandler, IHasConfirmText
    {
        #region Events

        public event EventHandler Click;
        private void OnClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

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

        public bool AutoPostBack
        {
            get { return ViewState[nameof(AutoPostBack)] == null || (bool)ViewState[nameof(AutoPostBack)]; }
            set { ViewState[nameof(AutoPostBack)] = value; }
        }

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

        public string Text
        {
            get { return base.AlternateText; }
            set { base.AlternateText = value; }
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
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);

            var cssClass = "icon";
            if (!string.IsNullOrEmpty(CssClass))
                cssClass += string.Format(" {0}", CssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            var imageAlign = GetImageAlignValue(ImageAlign);
            if (!string.IsNullOrEmpty(imageAlign))
                writer.AddAttribute(HtmlTextWriterAttribute.Align, imageAlign);

            if (!string.IsNullOrEmpty(AlternateText))
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, AlternateText);

            if (!string.IsNullOrEmpty(ToolTip))
                writer.AddAttribute(HtmlTextWriterAttribute.Title, ToolTip);

            var imageUrl = ImageUrl;
            if (!string.IsNullOrEmpty(imageUrl))
                writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl(imageUrl));

            var script = new StringBuilder();

            if (Enabled)
            {
                if (!string.IsNullOrEmpty(OnClientClick))
                    AddScript(script, OnClientClick);

                if (!string.IsNullOrEmpty(ConfirmText))
                    AddScript(script, string.Format("if (!confirm('{0}')) {{ return false; }}", HttpUtility.JavaScriptStringEncode(ConfirmText)));

                var postBackOptions = GetPostBackOptions();
                Page.ClientScript.RegisterForEventValidation(postBackOptions);

                var postBackEventReference = Page.ClientScript.GetPostBackEventReference(postBackOptions, false);
                if (!string.IsNullOrEmpty(postBackEventReference))
                {
                    AddScript(script, postBackEventReference);
                    if (postBackOptions.ClientSubmit)
                        AddScript(script, "return false;");
                }
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, script.Length > 0 ? script.ToString() : "return false;");

            AddAttributesToRender(writer);


            if (IsFontIcon)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("javascript:__doPostBack('{0}','')", UniqueID));

                writer.RenderBeginTag(HtmlTextWriterTag.A);

                RenderIconImage(false, writer);

                writer.RenderEndTag();
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");

                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
            }
        }

        #endregion

        #region Helpers

        private PostBackOptions GetPostBackOptions()
        {
            var options = new PostBackOptions(this, string.Empty)
            {
                AutoPostBack = AutoPostBack
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

        private static void AddScript(StringBuilder sb, string script)
        {
            if (string.IsNullOrEmpty(script))
                return;

            script = script.Trim();
            if (!script.EndsWith(";") && !script.EndsWith("}"))
                script += ";";

            if (sb.Length > 0)
                sb.Append(" ");

            sb.Append(script);
        }

        #endregion
    }
}