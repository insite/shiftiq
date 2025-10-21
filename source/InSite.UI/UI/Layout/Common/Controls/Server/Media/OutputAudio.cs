using System;
using System.Text;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OutputAudio : OutputMedia
    {
        #region Properties

        public string AudioURL
        {
            get => (string)(ViewState[nameof(AudioURL)] ?? string.Empty);
            set => ViewState[nameof(AudioURL)] = value;
        }

        public int AttemptLimit
        {
            get => (int)(ViewState[nameof(AttemptLimit)] ?? 0);
            set
            {
                var limit = Number.CheckRange(value, 0, InputAudio.MaximumAttemptLimit);

                ViewState[nameof(AttemptLimit)] = limit;

                if (CurrentAttempt > limit)
                    CurrentAttempt = limit;
            }
        }

        public int CurrentAttempt
        {
            get => (int)(ViewState[nameof(CurrentAttempt)] ?? 0);
            set => ViewState[nameof(CurrentAttempt)] = Number.CheckRange(value, 0, AttemptLimit);
        }

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class AudioClientData : BaseClientData
        {
            [JsonProperty(PropertyName = "attemptNow", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public int CurrentAttempt { get; set; }

            [JsonProperty(PropertyName = "attemptLimit", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public int AttemptLimit { get; set; }
        }

        #endregion

        #region Fields

        private AudioClientData _clientData;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            _clientData = new AudioClientData
            {
                Enabled = Enabled,
                CurrentAttempt = CurrentAttempt,
                AttemptLimit = AttemptLimit,
                Volume = Volume,
                Muted = Muted
            };

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(InputAudio),
                "init_" + ClientID,
                $"inSite.common.outputAudio.init('{ClientID}');",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler & IPostBackEventHandler

        protected override BaseClientData DeserializeClientData(string data)
        {
            return JsonConvert.DeserializeObject<AudioClientData>(data);
        }

        protected override bool LoadPostData(BaseClientData clientData)
        {
            var audioData = (AudioClientData)clientData;

            CurrentAttempt = audioData.CurrentAttempt;
            AttemptLimit = audioData.AttemptLimit;

            return base.LoadPostData(clientData);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "insite-output-audio input-group");

            if (DeleteScript.IsNotEmpty())
                writer.AddAttribute("data-delete", DeleteScript);

            if (AudioURL.IsNotEmpty())
                writer.AddAttribute("data-audio", AudioURL);

            if (AutoLoad)
                writer.AddAttribute("data-auto-load", "1");

            var style = GetStyle();
            if (style != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderTime(writer);
            RenderVolume(writer);
            RenderAttempts(writer);
            RenderButtons(writer);

            writer.RenderEndTag();
        }

        protected string GetStyle()
        {
            var sb = new StringBuilder();

            if (Hidden)
                sb.Append("display:none;");

            return sb.Length == 0 ? null : sb.ToString();
        }

        private void RenderTime(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text oa-position");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.Write("<span>Record Time</span>");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.Write("<div class=\"me-1 me-md-2 align-middle oa-from\">00:00</div>");
                writer.Write("<div class=\"align-middle d-inline-block d-md-none\">/</div>");
                writer.Write(
                    "<div class=\"align-middle d-none d-md-block oa-progress\">" +
                    "<div class=\"progress\">" +
                    "<div class=\"progress-bar progress-bar-striped\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"1\" style=\"width:0;\">" +
                    "</div>" +
                    "</div>" +
                    "</div>");
                writer.Write("<div class=\"ms-1 ms-md-2 align-middle oa-thru\">00:00</div>");

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, JsonConvert.SerializeObject(_clientData));
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }
            }
            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        private void RenderVolume(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text border-start-0 oa-volume");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.Write("<span>Volume</span>");

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    writer.Write(
                        "<div class=\"align-middle d-none d-md-inline-block oa-progress\">" +
                        "<div class=\"progress\">" +
                        "<div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"1\" style=\"width:0;\">" +
                        "</div>" +
                        "</div>" +
                        "</div>");
                    writer.Write("<span class=\"ms-2 align-middle oa-mute\"><i class=\"fas fa-volume\"></i></span>");
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private void RenderAttempts(HtmlTextWriter writer)
        {
            if (AttemptLimit == 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none;");

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text border-start-0 oa-attempts");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.Write("<span>Attempts</span>");

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write($"{CurrentAttempt}/{AttemptLimit}");
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private void RenderButtons(HtmlTextWriter writer)
        {
            RenderButton(writer, "play", "Play", "secondary fs-2", "play");

            if (AllowDelete)
                RenderButton(writer, "delete", "Delete", "danger fs-5", "trash-alt");
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
            return $"inSite.common.outputAudio.settings({InputAudio.MaximumAttemptLimit});";
        }

        #endregion
    }
}