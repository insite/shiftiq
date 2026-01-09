using System;
using System.Web.UI;

using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    [ParseChildren(true, "Message")]
    public sealed class CmdsAlert : Control
    {
        #region Properties

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public String Message
        {
            get;
            set;
        }

        public AlertType Type
        {
            get;
            set;
        }

        public bool AllowClose
        {
            get => (bool?)ViewState[nameof(AllowClose)] != false;
            set => ViewState[nameof(AllowClose)] = value;
        }

        #endregion

        #region Construction

        public CmdsAlert()
        {
            EnableViewState = false;
        }

        #endregion

        #region Public methods

        public void SetMessage(AlertType type, String text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            Message = text;
            Type = type;
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible || Type == AlertType.None)
                return;

            var cssClass = "alert alert-cmds";

            switch(Type)
            {
                case AlertType.Error:
                    cssClass += " alert-danger";
                    break;
                case AlertType.Information:
                    cssClass += " alert-info";
                    break;
                case AlertType.Success:
                    cssClass += " alert-success";
                    break;
                case AlertType.Warning:
                    cssClass += " alert-warning";
                    break;
            }

            if (AllowClose)
                cssClass += " alert-dismissible";

            writer.Write(@"<div class=""{0}"" role=""alert"">", cssClass);

            if (AllowClose)
                writer.Write(@"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button>");

            writer.Write(Message);
            writer.Write("</div>");
        }

        #endregion
    }
}