using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OutputVideo : OutputMedia
    {
        #region Properties

        public string VideoURL
        {
            get => (string)(ViewState[nameof(VideoURL)] ?? string.Empty);
            set => ViewState[nameof(VideoURL)] = value;
        }

        public bool AllowFullScreen
        {
            get => (bool)(ViewState[nameof(AllowFullScreen)] ?? true);
            set => ViewState[nameof(AllowFullScreen)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        #endregion

        #region Fields

        private BaseClientData _clientData;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            _clientData = new BaseClientData
            {
                Enabled = Enabled,
                Volume = Volume,
                Muted = Muted
            };

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(InputAudio),
                "init_" + ClientID,
                $"inSite.common.outputVideo.init('{ClientID}');",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "insite-output-video");

            if (DeleteScript.IsNotEmpty())
                writer.AddAttribute("data-delete", DeleteScript);

            if (VideoURL.IsNotEmpty())
                writer.AddAttribute("data-video", VideoURL);

            if (AutoLoad)
                writer.AddAttribute("data-auto-load", "1");

            var style = GetStyle();
            if (style != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                RenderVideoContainer(writer);
                RenderControlContainer(writer);
            }

            writer.RenderEndTag();
        }

        protected string GetStyle()
        {
            var sb = new StringBuilder();

            if (!Width.IsEmpty)
                sb.Append("width:").Append(Width.ToString()).Append(";");

            if (Hidden)
                sb.Append("display:none;");

            return sb.Length == 0 ? null : sb.ToString();
        }

        private void RenderVideoContainer(HtmlTextWriter writer)
        {
            writer.Write("<div class=\"ov-video\"><i class='fas fa-video ov-icon'></i><video></video></div>");
        }

        private void RenderControlContainer(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ov-control");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "d-flex me-3 flex-grow-1");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    RenderPosition(writer);
                    RenderVolume(writer);
                }
                writer.RenderEndTag();

                RenderButtons(writer);
            }
            writer.RenderEndTag();
        }

        private void RenderPosition(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ov-position me-3");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.Write("<span class=\"ov-label\">Record Time</span>");

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    writer.Write("<span class=\"me-1 me-md-2 align-middle ov-from\">00:00</span>");
                    writer.Write("<div class=\"align-middle d-inline-block d-md-none\">/</div>");
                    writer.Write(
                        "<div class=\"align-middle d-none d-md-inline-block ov-progress\">" +
                        "<div class=\"progress\">" +
                        "<div class=\"progress-bar progress-bar-striped\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"1\" style=\"width:0;\">" +
                        "</div>" +
                        "</div>" +
                        "</div>");
                    writer.Write("<span class=\"ms-1 ms-md-2 align-middle ov-thru\">00:00</span>");

                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, JsonConvert.SerializeObject(_clientData));
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();
                    }
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private void RenderVolume(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ov-volume");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.Write("<span class=\"ov-label\">Volume</span>");

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    writer.Write(
                        "<div class=\"align-middle d-none d-md-inline-block ov-progress\">" +
                        "<div class=\"progress\">" +
                        "<div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"1\" style=\"width:0;\">" +
                        "</div>" +
                        "</div>" +
                        "</div>");
                    writer.Write("<span class=\"ms-2 align-middle ov-mute\"><i class=\"fas fa-volume\"></i></span>");
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private void RenderButtons(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-group");
            writer.AddAttribute("role", "group");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                RenderButton(writer, "play", "Play", "secondary", "play");

                if (AllowFullScreen)
                    RenderButton(writer, "fullscreen", "Full Screen", "secondary", "expand");

                if (AllowDelete)
                    RenderButton(writer, "delete", "Delete", "danger", "trash-alt");
            }
            writer.RenderEndTag();
        }

        private void RenderButton(HtmlTextWriter writer, string action, string title, string color, string icon)
        {
            writer.AddAttribute("data-action", action);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Title, title);
            writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"btn btn-outline-{color}");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write($"<i class='fas fa-{icon}'></i>");
            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        // DON'T REMOVE!
        private static string SetupSettings()
        {
            return string.Empty;
        }

        #endregion
    }
}