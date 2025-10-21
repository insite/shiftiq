using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Messages.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Admin.Messages.Messages.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private const string DefaultBodyText = @"(Add your message content here, using the Shift iQ markdown content authoring tool, which is designed to be fast, light, and easy to use. This tool helps create content that's less likely to be marked as spam, making it more likely that your messages will reach your email subscribers.)";

        #region Properties

        private Guid MessageIdentifier => (Guid)(ViewState[nameof(MessageIdentifier)]
            ?? (ViewState[nameof(MessageIdentifier)] = CurrentSession?.MessageIdentifier ?? Guid.Empty));

        private Guid SessionIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        string DefaultParameters => $"message={MessageIdentifier}&tab=content";

        private ContentSession CurrentSession
        {
            get
            {
                if (!_sessionLoaded)
                {
                    _session = ContentSessionStorage.Get(SessionIdentifier, User.UserIdentifier);
                    _sessionLoaded = true;
                }

                return _session;
            }
        }

        #endregion

        #region Fields

        private bool _sessionLoaded;

        private ContentSession _session;

        private static readonly string _styleItemsPath = "~/UI/Layout/common/parts/css/markdown/fonts";

        private static readonly Tuple<string, string>[] _styleItems;

        #endregion

        #region Contruction

        static Content()
        {
            var server = HttpContext.Current.Server;

            {
                var items = new List<Tuple<string, string>>();
                var physPath = server.MapPath(_styleItemsPath);

                foreach (var filePath in Directory.EnumerateFiles(physPath, "*.css"))
                {
                    var fileVirtualPath = _styleItemsPath + "/" + Path.GetFileName(filePath);
                    var fileRelativePath = VirtualPathUtility.ToAbsolute(fileVirtualPath);

                    items.Add(new Tuple<string, string>(
                        Path.GetFileNameWithoutExtension(filePath),
                        fileRelativePath));
                }

                _styleItems = items.ToArray();
            }
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AutosavePanel.Request += AutosavePanel_Request;

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            DownloadButton.Click += DownloadButton_Click;

            MessageEditorHelpUrl.HRef = ServiceLocator.Urls.MessageEditorHelpUrl;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToParent();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void AutosavePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "autosave")
            {
                var session = ContentSessionStorage.Get(SessionIdentifier, User.UserIdentifier);
                if (session == null)
                {
                    Status.AddMessage(AlertType.Error, "Unable to save the session: the session doesn't exist.");
                    return;
                }

                try
                {
                    session.DocumentStyle = (DocumentStyle.GetSelectedOption()?.Text).EmptyIfNull();
                    session.DocumentType = DocumentType.Value;
                    session.MarkdownText = ContentTranslation.Text.Clone();

                    ContentSessionStorage.Set(session);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);

                    Status.AddMessage(AlertType.Error, "Unable to save the session: an error occurred on the server side.");
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ContentSessionStorage.Remove(SessionIdentifier, User.UserIdentifier);

            RedirectToParent();
        }

        private void DownloadButton_Click(object sender, CommandEventArgs e)
        {
            var title = "New Document";
            byte[] data;

            if (e.CommandName == "html")
            {
                var html = MessageHelper.CreateHtmlBody(title, ContentTranslation.Text.Default);
                html = MessageHelper.ReplaceTemplates(html, null);

                data = Encoding.UTF8.GetBytes(html);
                title += ".html";
            }
            else
            {
                data = Encoding.UTF8.GetBytes(ContentTranslation.Text.Default);
                title += ".md";
            }

            Response.SendFile(title, data, MediaTypeNames.Application.Octet);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var session = CurrentSession;
            if (session == null)
                RedirectToSearch();

            var message = ServiceLocator.MessageSearch.GetMessage(session.MessageIdentifier);
            if (message == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>");

            DocumentStyle.LoadItems(_styleItems, "Item2", "Item1");

            DuplicateButton.Visible = message.MessageType != MessageTypeName.Invitation && message.MessageType != MessageTypeName.Alert;
            DuplicateButton.NavigateUrl = $"/ui/admin/messages/create?action=duplicate&message={message.MessageIdentifier}";

            SetInputValues(session);
            ValidPlaceHolderNames.LoadData(message);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var session = ContentSessionStorage.Get(SessionIdentifier, User.UserIdentifier);
            if (session == null)
            {
                Status.AddMessage(AlertType.Error, "The session doesn't exist.");
                return false;
            }

            var compose = new ChangeContent(session.MessageIdentifier, ContentTranslation.Text);

            ServiceLocator.SendCommand(compose);

            ContentSessionStorage.Remove(SessionIdentifier, User.UserIdentifier);

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(ContentSession session)
        {
            ConfirmAlert.Visible = IsWarningMessageVisible(session.MessageIdentifier);

            var styleItem = DocumentStyle.FindOptionByText(session.DocumentStyle);
            if (styleItem != null)
                styleItem.Selected = true;
            else
                DocumentStyle.Value = null;

            if (session.MarkdownText == null)
                session.MarkdownText = new MultilingualString();

            if (session.MarkdownText.IsEmpty)
                session.MarkdownText.Default = DefaultBodyText;

            ContentTranslation.Text = session.MarkdownText;
            ContentUpload.FolderPath = $"/messages/uploads/{session.MessageIdentifier}";
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/messages/messages/search", true);

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"{GetParentUrl(DefaultParameters)}", true);

        private static string SetupTemplates()
        {
            return $"messageMarkdown.templates = {JsonHelper.SerializeJsObject(MessageHelper.Templates)};";
        }

        public bool IsWarningMessageVisible(Guid messageIdentifier)
        {
            List<VMailout> mailouts = ServiceLocator.MessageSearch.GetMailouts(new MailoutFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                MessageIdentifier = messageIdentifier,
                IsCompleted = false,
                IsCancelled = false
            });

            if (mailouts == null || !mailouts.Any(m => m.MailoutScheduled != default))
                return false;

            if (mailouts.Any(m => m.MailoutScheduled > DateTimeOffset.UtcNow))
                return true;

            return false;
        }

        #endregion

        #region Methods (navigation back)

        public string GetParentLinkParameters(IWebRoute parent)
            => GetParentLinkParameters(parent, null).IfNullOrEmpty($"message={MessageIdentifier}");

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        #endregion
    }
}