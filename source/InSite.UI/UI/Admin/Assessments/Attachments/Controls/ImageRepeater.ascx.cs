using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using Path = System.IO.Path;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class ImageRepeater : UserControl
    {
        #region Classes

        private class FileInfo
        {
            public Guid Identifier { get; set; }
            public string FileName { get; set; }
            public string DocumentName { get; set; }
            public string NavigateUrl { get; set; }
        }

        private class UploadedImage
        {
            #region Properties

            public bool IsUploaded { get; set; }
            public bool IsAttached => Attachment != null;

            public AttachmentInfo Attachment { get; set; }

            public string Name { get; private set; }
            public string Url { get; private set; }

            public string Icon
            {
                get
                {
                    var label = "custom-default";
                    var environment = "External";
                    var source = IsAttached ? "Bank " : string.Empty;

                    if (_environment == EnvironmentName.Production)
                    {
                        label = "success";
                        environment = "Production";
                    }
                    else if (_environment == EnvironmentName.Sandbox)
                    {
                        label = "warning";
                        environment = "Sandbox";
                    }
                    else if (_environment == EnvironmentName.Development)
                    {
                        label = "danger";
                        environment = "Development";
                    }
                    else if (_environment == EnvironmentName.Local)
                    {
                        label = "primary";
                        environment = "Local";
                    }

                    return $"<span class='badge bg-{label}'>{source}{environment}</span>";
                }
            }

            #endregion

            #region Fields

            private EnvironmentName _environment = EnvironmentName.External;

            private static readonly Regex _urlPattern = new Regex(
                @"https://(?:(?<Environment>dev|local|sandbox)-)?(?<Organization>.*)\.(?<Host>(?:insite|keyeracmds|shiftiq)\.com)/files(?<Path>/[^\?]+)(?<Query>\?.*)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );

            private static readonly Regex _environmentPattern = new Regex(
                @"https://(?:(?<Environment>dev|local|sandbox)-)?(?<Organization>.*)\.",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );

            #endregion

            #region Construction

            public UploadedImage(string url, Guid id, string fileName, string documentName)
            {
                url = HttpUtility.UrlDecode(url);

                if (!string.IsNullOrEmpty(fileName))
                {
                    Name = documentName;
                    Url = HttpRequestHelper.CurrentRootUrl + ServiceLocator.StorageService.GetFileUrl(id, fileName);
                    _environment = ServiceLocator.AppSettings.Environment.Name;
                    return;
                }

                var image = _urlPattern.Match(url);

                if (image.Success)
                {
                    var environment = image.Groups["Environment"];

                    if (!environment.Success)
                        _environment = EnvironmentName.Production;
                    else if (environment.Value == "dev")
                        _environment = EnvironmentName.Development;
                    else
                        _environment = environment.Value.ToEnum<EnvironmentName>(true);
                }

                var query = image.Groups["Query"];
                if (query.Success)
                    url = url.Substring(0, url.Length - query.Length);

                Url = url;

                var path = image.Groups["Path"].Value;
                if (path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) < 0)
                    Name = System.IO.Path.GetFileName(path);

                if (string.IsNullOrEmpty(Name))
                    Name = url;
            }

            #endregion
        }

        private class AttachmentInfo
        {
            #region Properties

            public string Title { get; }
            public string Number { get; }
            public string Condition { get; }
            public string PublicationStatus { get; }
            public string Dimension { get; }

            #endregion

            #region Construction

            public AttachmentInfo(Attachment attachment)
            {
                Title = (attachment.Content?.Title.Default).IfNullOrEmpty("(Untitled)");
                Number = $"{attachment.Asset}.{attachment.AssetVersion}";
                Condition = attachment.Condition;
                PublicationStatus = attachment.PublicationStatus.GetDescription();
                Dimension = attachment.Image != null ? $"{attachment.Image.Actual.Width} x {attachment.Image.Actual.Height}" : "0 x 0";
            }

            #endregion
        }

        [Serializable]
        private class UploadedFilterType
        {
            public int Number { get; set; }
            public string Subtype { get; set; }
        }

        [Serializable]
        private class UsedFilterType
        {
            public int AssetId { get; set; }
            public bool IsRelationshipSource { get; set; }
        }

        #endregion

        #region Constants

        private static readonly Regex MarkdownImagePattern = new Regex(
            @"!\[(?<Alt>.*?)]\((?<Url>.*?)(?:\s""(?<Title>.*)"")?\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Properties

        public bool IsEmpty => Repeater.Items.Count == 0;

        private Guid? BankID
        {
            get => (Guid?)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        #endregion

        #region Loading

        public void LoadData(Guid bankId)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(bankId);
            if (bank == null)
                return;
            LoadData(bank);
        }

        public void LoadData(BankState bank)
        {
            BankID = bank.Identifier;

            var organization = CurrentSessionState.Identity.Organization;
            var attachments = bank.EnumerateAllAttachments().GroupBy(x => x.FileIdentifier ?? x.Upload).ToDictionary(x => x.Key, x => x.First());
            var images = new Dictionary<string, UploadedImage>(StringComparer.OrdinalIgnoreCase);

            var files = GetFiles(bank);

            foreach (var file in files)
            {
                var image = new UploadedImage(file.NavigateUrl, file.Identifier, file.FileName, file.DocumentName);
                if (!images.ContainsKey(image.Url))
                    images.Add(image.Url, image);

                if (attachments.TryGetValue(file.Identifier, out var attachment))
                    image.Attachment = new AttachmentInfo(attachment);

                image.IsUploaded = true;
            }

            ParseQuestions(bank, attachments, images);

            Repeater.DataSource = images.Select(x => x.Value)
                .OrderBy(x => !x.IsAttached)
                .ThenBy(x => (x.Attachment?.Title).IfNullOrEmpty(x.Name));
            Repeater.DataBind();
        }

        private void ParseQuestions(BankState bank, Dictionary<Guid, Attachment> attachments, Dictionary<string, UploadedImage> images)
        {
            foreach (var question in bank.Sets.SelectMany(x => x.EnumerateAllQuestions()))
            {
                var title = question.Content.Title?.Default;
                if (string.IsNullOrEmpty(title))
                    continue;

                var matches = MarkdownImagePattern.Matches(title);
                if (matches.Count == 0)
                    continue;

                foreach (Match match in matches)
                {
                    var url = match.Groups["Url"].Value;
                    var files = ServiceLocator.StorageService.ExtractAndParseFileUrls(url);

                    var image = files.Count > 0 && attachments.ContainsKey(files[0].FileIdentifier)
                        ? new UploadedImage(null, files[0].FileIdentifier, files[0].FileName, files[0].FileName)
                        : new UploadedImage(url, Guid.Empty, null, null);

                    if (!images.ContainsKey(image.Url))
                        images.Add(image.Url, image);
                }
            }
        }

        private static List<FileInfo> GetFiles(BankState bank)
        {
            var attachments = bank.EnumerateAllAttachments().Where(x => x.Type == AttachmentType.Image).ToList();

            var uploads = UploadSearch.Bind(x => new { x.UploadIdentifier, x.Name, x.NavigateUrl }, attachments)
                .Select(x => new FileInfo
                {
                    Identifier = x.UploadIdentifier,
                    NavigateUrl = FileHelper.GetUrl(x.NavigateUrl)
                })
                .ToList();

            var fileIds = attachments.Where(x => x.FileIdentifier.HasValue).Select(x => x.FileIdentifier.Value).ToArray();
            var files = ServiceLocator.FileSearch
                .GetModels(fileIds, false)
                .Select(x => new FileInfo
                {
                    Identifier = x.FileIdentifier,
                    FileName = x.FileName,
                    DocumentName = x.Properties.DocumentName,
                })
                .ToList();

            files.AddRange(uploads);

            return files;
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ImageRepeater),
                "init" + ClientID,
                $"attachmentsImageRepeater.init('{Container.ClientID}');",
                true);
        }

        #endregion
    }
}