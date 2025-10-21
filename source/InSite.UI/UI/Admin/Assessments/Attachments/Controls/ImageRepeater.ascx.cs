using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        private class UploadedImage
        {
            #region Properties

            public bool IsUploaded { get; set; }
            public bool IsAttached => Attachment != null;

            public AttachmentInfo Attachment { get; set; }

            public string Name { get; private set; }
            public string Url { get; private set; }
            public string Path { get; private set; }

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

            #endregion

            #region Construction

            public UploadedImage(string url)
            {
                url = HttpUtility.UrlDecode(url);

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

                Path = image.Groups["Path"].Value;
                Url = url;

                if (Path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) < 0)
                    Name = System.IO.Path.GetFileName(Path);

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
            var images = new Dictionary<string, UploadedImage>(StringComparer.OrdinalIgnoreCase);

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
                    var image = new UploadedImage(match.Groups["Url"].Value);
                    if (!images.ContainsKey(image.Url))
                        images.Add(image.Url, image);
                }
            }

            var uploads = UploadSearch.Bind(
                x => new { x.UploadIdentifier, x.Name, x.NavigateUrl },
                bank.EnumerateAllAttachments().Where(x => x.Type == AttachmentType.Image));
            var attachments = bank.EnumerateAllAttachments().GroupBy(x => x.Upload).ToDictionary(x => x.Key, x => x.First());

            foreach (var upload in uploads)
            {
                var url = FileHelper.GetUrl(upload.NavigateUrl);
                if (!images.TryGetValue(url, out var image))
                    images.Add(url, image = new UploadedImage(url));

                if (attachments.TryGetValue(upload.UploadIdentifier, out var attachment))
                    image.Attachment = new AttachmentInfo(attachment);

                image.IsUploaded = true;
            }

            Repeater.DataSource = images.Select(x => x.Value)
                .OrderBy(x => !x.IsAttached)
                .ThenBy(x => (x.Attachment?.Title).IfNullOrEmpty(x.Name));
            Repeater.DataBind();
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