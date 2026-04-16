using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attachments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private class FileInfo
        {
            public string Name { get; set; }
            public string NavigateUrl { get; set; }
            public int? ContentSize { get; set; }
        }

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid AttachmentID => Guid.TryParse(Request.QueryString["attachment"], out var value) ? value : Guid.Empty;

        private AttachmentType? AttachmentType
        {
            get => (AttachmentType?)ViewState[nameof(AttachmentType)];
            set => ViewState[nameof(AttachmentType)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, DataAccess.Update))
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");

            if (!IsPostBack)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToSearch();

                var attachment = bank.FindAttachment(AttachmentID);
                if (attachment == null)
                    RedirectToReader(false);

                PageHelper.AutoBindHeader(this, null, $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

                AttachmentTitle.Text = attachment.Content.Title.Default;
                AssetNumber.Text = $"{attachment.Asset}.{attachment.AssetVersion}";
                PublicationStatus.Text = attachment.PublicationStatus.GetDescription();

                var author = UserSearch.Bind(attachment.Author, x => new { x.FullName });
                var authorName = author == null ? "(Unknown)" : author.FullName;

                Timestamp.Text = $"Posted by {authorName} on {attachment.Uploaded.Format(User.TimeZone)}";
                AttachmentType = attachment.Type;

                var fileInfo = ReadFileInfo(attachment.FileIdentifier, attachment.Upload);
                if (fileInfo != null)
                {
                    FileName.Text = $"<a href=\"{fileInfo.NavigateUrl}\">{fileInfo.Name}</a>";
                    FileSize.Text = (fileInfo.ContentSize ?? 0).Bytes().Humanize("0.##");
                }
                else
                {
                    FileName.Text = "Unknown";
                    FileSize.Text = "Unknown";
                }

                if (attachment.IsSingleVersion())
                {
                    var item = VersionSelector.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Value == "all").FirstOrDefault();
                    if (item != null)
                        item.Enabled = false;
                }

                BindQuestions(bank, attachment);

                CancelButton.NavigateUrl = GetReaderUrl(false);
            }
        }

        private static FileInfo ReadFileInfo(Guid? fileId, Guid uploadId)
        {
            if (fileId.HasValue)
            {
                var file = ServiceLocator.FileSearch.GetModel(fileId.Value);
                return file != null
                    ? new FileInfo
                        {
                            Name = file.Properties.DocumentName,
                            NavigateUrl = ServiceLocator.StorageService.GetFileUrl(file, true),
                            ContentSize = file.FileSize
                        }
                    : null;
            }

            var upload = UploadSearch.Select(uploadId);
            return upload != null
                ? new FileInfo
                {
                    Name = upload.Name,
                    NavigateUrl = $"/files{upload.NavigateUrl}?download=1",
                    ContentSize = upload.ContentSize
                }
                : null;
        }

        private void BindQuestions(BankState bank, Attachment attachment)
        {
            var allQuestionIds = attachment
                .EnumerateAllVersions()
                .SelectMany(x => x.QuestionIdentifiers)
                .ToHashSet();

            QuestionListField.Visible = allQuestionIds.Count > 0;

            if (allQuestionIds.Count == 0)
                return;

            var questions = new List<Question>();

            foreach (var questionId in allQuestionIds)
            {
                var question = bank.FindQuestion(questionId);
                questions.Add(question);
            }

            questions.Sort((a, b) => a.BankIndex.CompareTo(b.BankIndex));

            QuestionList.DataSource = questions;
            QuestionList.DataBind();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var attachment = bank?.FindAttachment(AttachmentID);

            if (VersionSelector.SelectedValue == "all")
            {
                if (bank != null)
                {
                    foreach (var v in attachment.EnumerateAllVersions(SortOrder.Descending))
                    {
                        ServiceLocator.SendCommand(new DeleteAttachment(BankID, v.Identifier));
                        if (v.FileIdentifier.HasValue)
                            ServiceLocator.StorageService.Delete(v.FileIdentifier.Value);
                    }
                }
            }
            else if (attachment != null)
            {
                ServiceLocator.SendCommand(new DeleteAttachment(BankID, AttachmentID));

                if (attachment.FileIdentifier.HasValue)
                    ServiceLocator.StorageService.Delete(attachment.FileIdentifier.Value);
            }

            RedirectToReader(true);
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(bool isRemoved) => HttpResponseHelper.Redirect(GetReaderUrl(isRemoved), true);

        private string GetReaderUrl(bool isRemoved)
        {
            string tabQuery = null;

            if (AttachmentType.HasValue)
            {
                if (AttachmentType.Value == Shift.Constant.AttachmentType.Image)
                    tabQuery = "tab=images";
                else if (AttachmentType == Shift.Constant.AttachmentType.Document)
                    tabQuery = "tab=documents";
                else
                    tabQuery = "tab=other";
            }

            var returnUrl = new ReturnUrl();
            var url = isRemoved
                ? returnUrl.GetReturnUrl("attachment=", tabQuery, tabQuery)
                : returnUrl.GetReturnUrl();

            if (url == null)
            {
                url = $"/ui/admin/assessments/banks/outline?bank={BankID}&panel=attachments";
                if (tabQuery == null)
                    url += "&" + tabQuery;
            }

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}