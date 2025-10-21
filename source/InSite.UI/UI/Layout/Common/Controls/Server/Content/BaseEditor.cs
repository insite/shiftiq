using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(false), ValidationProperty(nameof(Value))]
    public abstract class BaseEditor : Control, IPostBackDataHandler
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class UploadSuccessArgs
        {
            [JsonProperty(PropertyName = "textId")]
            public string TextID { get; }

            [JsonProperty(PropertyName = "url")]
            public string FileUrl { get; }

            [JsonProperty(PropertyName = "title")]
            public string FileTitle { get; }

            [JsonProperty(PropertyName = "img")]
            public bool IsImage { get; }

            [JsonProperty(PropertyName = "msgs")]
            public string[] Messages { get; }

            public UploadSuccessArgs(string textId, EditorUpload.SuccessEventArgs args)
                : this(textId, args.Title, args.Url)
            {
                var messages = new List<string>();

                foreach (var message in args.Messages)
                {
                    if (message.StartsWith("Warning: image resized"))
                    {
                        var sizeStartIndex = message.IndexOf('(') + 1;
                        var sizeEndIndex = message.IndexOf(')', sizeStartIndex);
                        var sizeStringParts = message.Substring(sizeStartIndex, sizeEndIndex - sizeStartIndex).Split(new[] { " -> " }, StringSplitOptions.None);

                        messages.Add(
                            $"The recommended maximum size for an upload image is {sizeStringParts[1]} pixels." +
                            $"\r\nThe size of your image is {sizeStringParts[0]} pixels." +
                            $"\r\nThe system has automatically scaled the image for you, but you may want to resize your own images in the future before you upload them.");
                    }
                }

                Messages = messages.ToArray();
            }

            public UploadSuccessArgs(string textId, string title, string url)
            {
                TextID = textId;
                FileTitle = title;
                FileUrl = url;

                var fileExt = System.IO.Path.GetExtension(url);
                IsImage = FileExtension.IsImage(fileExt);
            }
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;
        private void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        #endregion

        #region Properties

        public string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public Unit Height
        {
            get => (Unit)(ViewState[nameof(Height)] ?? Unit.Empty);
            set => ViewState[nameof(Height)] = value;
        }

        public string Value
        {
            get => (string)(ViewState[nameof(Value)] ?? string.Empty);
            set => ViewState[nameof(Value)] = NormalizeNewLine(value ?? string.Empty);
        }

        public string UploadControl
        {
            get => (string)ViewState[nameof(UploadControl)];
            set
            {
                if (_isInited)
                    SetupUploadControl(value);

                ViewState[nameof(UploadControl)] = value;
            }
        }

        #endregion

        #region Fields

        private EditorUpload _upload = null;
        private bool _isInited = false;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _isInited = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            SetupUploadControl(UploadControl);

            base.OnLoad(e);
        }

        private void SetupUploadControl(string id)
        {
            if (_upload != null)
                _upload.Success -= Upload_Success;

            if (id.IsEmpty())
                return;

            _upload = (EditorUpload)(NamingContainer is DynamicControl
                ? NamingContainer.NamingContainer.FindControl(id)
                : NamingContainer.FindControl(id));

            if (_upload != null)
                _upload.Success += Upload_Success;
        }

        #endregion

        #region Event handlers

        private void Upload_Success(object sender, EditorUpload.SuccessEventArgs args) => OnUploadSuccess(args);

        protected abstract void OnUploadSuccess(EditorUpload.SuccessEventArgs args);

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            var text = NormalizeNewLine(postCollection[postDataKey]);
            var isChanged = Visible && !Value.Equals(text, StringComparison.Ordinal);
            if (isChanged)
                Value = text;

            return isChanged;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            OnValueChanged(EventArgs.Empty);
        }

        #endregion

        #region Methods (helpers)

        private static string NormalizeNewLine(string value)
        {
            return value.IsEmpty()
                ? value
                : value.Replace("\r", string.Empty).Replace("\n", System.Environment.NewLine);
        }

        #endregion
    }
}