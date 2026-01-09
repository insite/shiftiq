using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public interface IToast
    {
        AlertType Indicator { get; set; }
        string Icon { get; set; }
        string Title { get; set; }
        string Text { get; set; }
    }

    [Serializable]
    public class ToastItem
    {
        public AlertType Color { get; set; }
        public string Icon { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }

        public static ToastItem UrlDecode(string data)
        {
            if (data.IsEmpty())
                return null;

            var result = new ToastItem();

            try
            {
                StringHelper.DecodeBase64Url(data, stream =>
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        result.Color = (AlertType)reader.ReadByte();
                        result.Icon = reader.ReadString();
                        result.Header = reader.ReadString();
                        result.Body = reader.ReadString();
                    }
                });
            }
            catch (ArgumentNullException)
            {
                return null;
            }

            return result;
        }

        public static string UrlEncode(AlertType color, string icon, string header, string body)
        {
            if (icon.IsEmpty())
                throw new ArgumentNullException(nameof(icon));

            if (header.IsEmpty())
                throw new ArgumentNullException(nameof(header));

            if (body.IsEmpty())
                throw new ArgumentNullException(nameof(body));

            return StringHelper.EncodeBase64Url(stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write((byte)color);
                    writer.Write(icon);
                    writer.Write(header);
                    writer.Write(body);
                }
            });
        }
    }

    public class Toast : Control, IToast, IHasText
    {
        #region Properties

        public AlertType Indicator
        {
            get;
            set;
        }

        public string Icon
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text
        {
            get;
            set;
        }

        public bool ShowClose
        {
            get { return ViewState[nameof(ShowClose)] == null || (bool)ViewState[nameof(ShowClose)]; }
            set { ViewState[nameof(ShowClose)] = value; }
        }

        #endregion

        #region Fields

        private List<Tuple<AlertType, string, string, string>> _messages;

        #endregion

        #region Construction

        public Toast()
        {
            EnableViewState = false;

            _messages = new List<Tuple<AlertType, string, string, string>>();
        }

        #endregion

        #region Public methods

        public void AddMessage(AlertType indicator, string icon, string title, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            foreach (var t in _messages)
            {
                if (t.Item4 == text)
                    return;
            }

            _messages.Add(new Tuple<AlertType, string, string, string>(indicator, icon, title, text));
        }

        public IEnumerable<Tuple<AlertType, string, string, string>> GetMessages() => _messages.ToArray();

        public void Clear()
        {
            Indicator = AlertType.None;
            Icon = null;
            Title = null;
            Text = null;

            _messages.Clear();
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Visible)
                RenderControlInternal(writer);
        }

        public void RenderControlInternal(HtmlTextWriter writer)
        {
            var closeHtml = string.Empty;

            if (ShowClose)
            {
                closeHtml = "<button type='button' class='btn-close btn-close-white ms-2 mb-1' data-bs-dismiss='toast' aria-label='Close'></button>";
            }

            writer.Write("<div class='position-fixed p-3' style='z-index:1021; top:80px; right:0;'>");

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderMessage(writer, Indicator, Icon, Title, Text, closeHtml);

            foreach (var message in _messages)
                RenderMessage(writer, message.Item1, message.Item2, message.Item3, message.Item4, closeHtml);

            writer.RenderEndTag();

            writer.Write("</div>");
        }

        private static void RenderMessage(HtmlTextWriter writer, AlertType indicator, string icon, string title, string text, string closeHtml)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var indicatorCssClass = indicator.GetContextualClass();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "toast fade show");
            writer.AddAttribute("role", "alert");
            writer.AddAttribute("aria-live", "assertive");
            writer.AddAttribute("aria-atomic", "true");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"toast-header bg-{indicatorCssClass} text-white");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"{icon} me-2");
            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "me-auto");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(title);
            writer.RenderEndTag();

            writer.Write(closeHtml);

            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"toast-body text-{indicatorCssClass}");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(text);
            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        #endregion
    }
}