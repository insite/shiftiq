using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using Path = System.IO.Path;

namespace InSite.Admin.Assessments.Attachments.Forms
{
    public partial class ScanImages : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        [Serializable]
        private class ImageInfo
        {
            public int Index { get; set; }
            public string Title { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }
            public string FileSize { get; set; }
            public string Url { get; set; }
            public bool IsAttachment { get; set; }
            public Guid? SourceQuestion { get; set; }
            public HashSet<int> BankIndexes { get; private set; } = new HashSet<int>();

            public string BankIndexHtml
            {
                get
                {
                    var sortedList = new List<int>(BankIndexes);
                    sortedList.Sort();

                    var html = new StringBuilder((sortedList[0] + 1).ToString());

                    if (sortedList.Count > 1)
                    {
                        html.Append("<div class='form-text'>also ");

                        for (int i = 1; i < sortedList.Count; i++)
                        {
                            if (i > 1)
                                html.Append(", ");

                            html.Append(sortedList[i] + 1);
                        }

                        html.Append("</div>");
                    }

                    return html.ToString();
                }
            }
        }

        private class AttachmentInfo
        {
            public string Url { get; set; }
            public int? FileSize { get; set; }
        }

        private class SourceInfo
        {
            public Guid BankIdentifier { get; internal set; }
            public SourceAttachmentInfo[] Attachments { get; internal set; }
        }

        private class SourceAttachmentInfo
        {
            public string NavigateUrl { get; internal set; }
            public Attachment Attachment { get; internal set; }
        }

        #endregion

        #region Constants

        private static readonly Regex ImageRegex = new Regex(@"\!\[(?<title>[^\]]*)\]\((?<url>[^\)]*?)(?<url_query>\?.*?)?\)", RegexOptions.Compiled);

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private List<ImageInfo> Images
        {
            get => (List<ImageInfo>)ViewState[nameof(Images)];
            set => ViewState[nameof(Images)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ImageRepeater.ItemDataBound += ImageRepeater_ItemDataBound;
            ImageRepeater.ItemCommand += ImageRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var bank = LoadBank();

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            Scan(bank);

            ImageRepeater.DataSource = Images;
            ImageRepeater.DataBind();

            ImageRepeater.Visible = Images.Count > 0;
            NoImages.Visible = Images.Count == 0;

            CancelButton.NavigateUrl = GetBankReaderUrl();
        }

        #endregion

        #region Event handlers

        private void ImageRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
                e.Item.FindControl("AddAllButton").Visible = CanCreate && Images.Any(x => !x.IsAttachment && x.FileSize != null);
        }

        private void ImageRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddAll")
            {
                try
                {
                    var imgs = Images.Where(x => !x.IsAttachment).ToArray();

                    AddAttachments(imgs);

                    ScreenStatus.AddMessage(AlertType.Success, $"{"attachment".ToQuantity(imgs.Length, "n0")} have been successfully added.");
                }
                catch (ApplicationError apperr)
                {
                    ScreenStatus.AddMessage(AlertType.Error, apperr.Message);
                }

                ImageRepeater.DataSource = Images;
                ImageRepeater.DataBind();
            }
            else if (e.CommandName == "AddOne")
            {
                try
                {
                    var index = int.Parse((string)e.CommandArgument);
                    var image = Images[index];

                    AddAttachments(new[] { image });

                    ScreenStatus.AddMessage(AlertType.Success, "The attachment has been successfully added.");
                }
                catch (ApplicationError apperr)
                {
                    ScreenStatus.AddMessage(AlertType.Error, apperr.Message);
                }

                ImageRepeater.DataSource = Images;
                ImageRepeater.DataBind();
            }
        }

        #endregion

        #region Methods

        private BankState LoadBank()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);

            if (bank == null)
                RedirectToSearch();

            return bank;
        }

        private static AttachmentInfo[] LoadAttachments(BankState bank)
        {
            var uploads = UploadSearch
                .Bind(x => new { x.UploadIdentifier, x.Name, x.NavigateUrl, x.ContentSize, x.Uploaded }, bank.EnumerateAllAttachments())
                .ToDictionary(x => x.UploadIdentifier);

            return bank.EnumerateAllAttachments()
                .Where(x => x.Type == AttachmentType.Image && uploads.ContainsKey(x.Upload))
                .Select(x =>
                {
                    var upload = uploads[x.Upload];

                    return new AttachmentInfo
                    {
                        Url = "/files" + upload.NavigateUrl,
                        FileSize = upload.ContentSize
                    };
                }).ToArray();
        }

        private void AddAttachments(IEnumerable<ImageInfo> images)
        {
            var bank = LoadBank();

            var sourceMapping = new Dictionary<Guid, SourceInfo>();
            {
                var questions = ServiceLocator.BankSearch
                    .GetQuestions(images.Where(x => x.SourceQuestion.HasValue).Select(x => x.SourceQuestion.Value).Distinct());
                var banks = ServiceLocator.BankSearch
                    .GetBankStates(questions.Select(x => x.BankIdentifier).Distinct());
                var sourceByBank = new Dictionary<Guid, SourceInfo>();

                foreach (var info in questions)
                {
                    if (!sourceByBank.TryGetValue(info.BankIdentifier, out var source))
                    {
                        var sourceBank = banks.FirstOrDefault(x => x.Identifier == info.BankIdentifier);
                        if (sourceBank == null)
                            continue;

                        var sAttachments = sourceBank.EnumerateAllAttachments()
                            .Where(x => x.Type == AttachmentType.Image).ToArray();
                        var uploads = UploadSearch
                            .Bind(
                                x => new { x.UploadIdentifier, x.NavigateUrl },
                                sourceBank.EnumerateAllAttachments().Where(x => x.Type == AttachmentType.Image))
                            .ToDictionary(x => x.UploadIdentifier);

                        sourceByBank.Add(info.BankIdentifier, source = new SourceInfo
                        {
                            BankIdentifier = sourceBank.Identifier,
                            Attachments = sAttachments
                                .Where(x => uploads.ContainsKey(x.Upload))
                                .Select(x => new SourceAttachmentInfo
                                {
                                    NavigateUrl = uploads[x.Upload].NavigateUrl,
                                    Attachment = x
                                })
                                .ToArray()
                        });
                    }

                    sourceMapping.Add(info.QuestionIdentifier, source);
                }
            }

            foreach (var image in images)
            {
                Attachment attachment;
                {
                    var filePath = AttachmentHelper.GetFilePath(bank.Asset, image.FileName);

                    if (image.Url.EndsWith(filePath, StringComparison.OrdinalIgnoreCase) && UploadSearch.ExistsByOrganizationIdentifier(Organization.OrganizationIdentifier, filePath))
                    {
                        var file = FileHelper.Provider.OpenModel(Organization.OrganizationIdentifier, filePath);
                        if (bank.EnumerateAllAttachments().Any(x => x.Type == AttachmentType.Image && x.Upload == file.Guid))
                            continue;

                        attachment = Add.AttachFile(bank.Identifier, file, image.Title);
                    }
                    else
                    {
                        try
                        {
                            var imgUrl = image.Url;
                            if (imgUrl.IndexOf(':') == -1)
                                imgUrl = HttpRequestHelper.CurrentRootUrl + imgUrl;

                            var request = CreateWebRequest(imgUrl, "GET");
                            using (var response = request.GetResponse())
                            {
                                using (var stream = new MemoryStream())
                                {
                                    response.GetResponseStream().CopyTo(stream);

                                    stream.Seek(0, SeekOrigin.Begin);

                                    attachment = Add.AttachFile(bank.Identifier, bank.Asset, image.FileName, image.Title, stream);
                                }
                            }
                        }
                        catch (NotSupportedException)
                        {
                            continue;
                        }
                        catch (WebException)
                        {
                            continue;
                        }

                        var source = image.SourceQuestion.HasValue && sourceMapping.ContainsKey(image.SourceQuestion.Value)
                            ? sourceMapping[image.SourceQuestion.Value].Attachments
                                .Where(x => image.Url.EndsWith(x.NavigateUrl, StringComparison.OrdinalIgnoreCase))
                                .Select(x => x.Attachment)
                                .FirstOrDefault()
                            : null;

                        if (source != null && source.Image != null)
                        {
                            if (attachment.Image.Resolution == default)
                                attachment.Image.Resolution = source.Image.Resolution;

                            attachment.Image.TargetOnline = source.Image.TargetOnline?.Clone();
                            attachment.Image.TargetPaper = source.Image.TargetPaper?.Clone();
                            attachment.Image.IsColor = source.Image.IsColor;

                            ServiceLocator.SendCommand(new ChangeAttachment(BankID, attachment.Identifier, attachment.Condition, attachment.Content, attachment.Image));
                        }
                    }
                }

                var upload = UploadSearch.Select(attachment.Upload);
                var url = FileHelper.GetUrl(upload.NavigateUrl);
                url = UrlHelper.EncodeMarkdownUrl(url);

                var questions = bank.Sets.SelectMany(x => x.EnumerateAllQuestions().Where(y => image.BankIndexes.Contains(y.BankIndex))).ToList();

                foreach (var question in questions)
                {
                    var content = question.Content;
                    var title = content.Title?.Default;

                    if (title == null || !title.Contains(image.Url, StringComparison.OrdinalIgnoreCase))
                        continue;

                    content.Title.Default = title.Replace(image.Url, url, StringComparison.OrdinalIgnoreCase);

                    ServiceLocator.SendCommand(new ChangeQuestionContent(BankID, question.Identifier, content));
                }

                image.Url = url;
                image.IsAttachment = true;
            }
        }

        #endregion

        #region Methods (scan images)

        private void Scan(BankState bank)
        {
            var attachments = LoadAttachments(bank);
            var images = new List<ImageInfo>();
            var imageMap = new Dictionary<string, ImageInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var question in bank.Sets.SelectMany(x => x.EnumerateAllQuestions()))
                ParseImages(question, attachments, images, imageMap);

            images.Sort((x1, x2) => x1.BankIndexes.Min().CompareTo(x2.BankIndexes.Min()));

            Images = images;
        }

        private void ParseImages(Question question, IEnumerable<AttachmentInfo> attachments, List<ImageInfo> images, Dictionary<string, ImageInfo> imageMap)
        {
            var text = question.Content.Title.Default;

            if (!text.HasValue())
                return;

            var matches = ImageRegex.Matches(text);

            foreach (Match match in matches)
            {
                var url = HttpUtility.UrlDecode(match.Groups["url"].Value.Trim());

                if (!imageMap.TryGetValue(url, out var image))
                {
                    var fileName = GetFileNameFromUrl(url);
                    if (string.IsNullOrEmpty(fileName))
                        continue;

                    var title = match.Groups["title"].Value.Trim();
                    var attachment = attachments.FirstOrDefault(x => url.EndsWith(x.Url, StringComparison.OrdinalIgnoreCase));
                    var fileSize = attachment?.FileSize ?? GetFileSize(url);

                    if (!fileSize.HasValue)
                        continue;

                    image = new ImageInfo
                    {
                        Index = images.Count,
                        Title = title,
                        FileName = fileName,
                        FileExtension = Path.GetExtension(fileName),
                        FileSize = fileSize?.Bytes().Humanize("0.#"),
                        Url = url,
                        IsAttachment = attachment != null,
                        SourceQuestion = question.Source
                    };

                    images.Add(image);
                    imageMap.Add(url, image);
                }

                image.BankIndexes.Add(question.BankIndex);
            }
        }

        private static string GetFileNameFromUrl(string url)
        {
            try
            {
                return Path.GetFileName(url);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static long? GetFileSize(string url)
        {
            if (url.IndexOf(':') < 0)
                url = HttpRequestHelper.CurrentRootUrl + url;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return null;

            if (TryGetFileSize("HEAD", out var size) || TryGetFileSize("GET", out size))
                return size;

            try
            {
                using (var client = new WebClient())
                {
                    var data = client.DownloadData(url);
                    return data.IsEmpty() ? null : (long?)data.Length;
                }
            }
            catch (WebException)
            {
                return null;
            }

            bool TryGetFileSize(string method, out long result)
            {
                result = 0;

                try
                {
                    var request = CreateWebRequest(url, method);
                    using (var response = request.GetResponse())
                    {
                        if (response.ContentLength > 0)
                        {
                            result = response.ContentLength;
                            return true;
                        }
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (WebException)
                {
                }

                return false;
            }
        }

        private static WebRequest CreateWebRequest(string url, string method)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";
            return request;
        }

        #endregion

        #region Methods (navigation)

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private string GetBankReaderUrl() =>
            $"/ui/admin/assessments/banks/outline?bank={BankID}&panel=attachments";

        public string GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;

        #endregion
    }
}