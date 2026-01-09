using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class ImagePrint : UserControl
    {
        #region Classes

        public class ImageInfo
        {
            public int Sequence { get; set; }
            public Attachment Attachment { get; }
            public string Url { get; }

            public ImageInfo(Attachment attachment, string url)
            {
                Attachment = attachment;
                Url = url;
            }
        }

        private class ControlData
        {
            public OutputType Output { get; set; }
            public bool FitPageSize { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }
            public ImageInfo[] Images { get; set; }
        }

        public class BankOptions : Options
        {
            public Guid BankID { get; }

            public BankOptions(OrganizationState organization, Guid bankId, OutputType output)
                : base(organization, output)
            {
                BankID = bankId;
            }
        }

        public class FormOptions : Options
        {
            public Guid FormID { get; }

            public FormOptions(OrganizationState organization, Guid formId, OutputType output)
                : base(organization, output)
            {
                FormID = formId;
            }
        }

        public class Options
        {
            public Guid OrganizationID { get; }
            public OutputType Output { get; }

            public string CurrentUrl { get; }
            public string HeaderUrl { get; }
            public string FooterUrl { get; }

            public Options(OrganizationState organization, OutputType output)
            {
                OrganizationID = organization.Identifier;
                Output = output;

                var request = HttpContext.Current.Request;

                CurrentUrl = request.Url.Scheme + "://" + request.Url.Host + request.RawUrl;
                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Assessments/Attachments/Html/PrintHeader.html");
                FooterUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Assessments/Attachments/Html/PrintFooter.html");
            }
        }

        #endregion

        #region Constants

        protected const int PageSize = 36;
        private const decimal DefaultDpi = 96.0m;
        private const decimal DefaultZoom = 0.89m;
        private const decimal PageDpi = DefaultDpi / DefaultZoom;

        #endregion

        #region Properties

        protected int PageCount { get; set; }

        #endregion

        private void LoadData(ControlData data)
        {
            PageTitle.InnerText = data.Title;
            PageCount = (int)Math.Ceiling((decimal)data.Images.Length / PageSize);
            ImgFitPageStyle.Visible = data.FitPageSize;

            var images = data.Images.Select(x =>
            {
                string style = null;

                var dimension = x.Attachment.Image?.TargetPaper;

                if (dimension != null && (dimension.Height <= 0 || dimension.Width <= 0))
                {
                    dimension = x.Attachment.Image?.Actual;

                    if (dimension != null && (dimension.Height <= 0 || dimension.Width <= 0))
                        dimension = null;
                }

                if (!data.FitPageSize && dimension != null && x.Attachment.Image != null && x.Attachment.Image.Resolution > 0)
                {
                    var scale = PageDpi / x.Attachment.Image.Resolution;
                    var width = (int)(scale * dimension.Width);

                    style = $"width:{width}px;";
                }

                return new
                {
                    Sequence = x.Sequence,
                    Title = (x.Attachment.Content?.Title.Default).IfNullOrEmpty("(Untitled)"),
                    Url = x.Url,
                    ImgStyle = style
                };
            }).OrderBy(x => x.Sequence).ThenBy(x => x.Title).ToArray();

            TocRepeater.DataSource = images;
            TocRepeater.DataBind();

            ImageRepeater.DataSource = images;
            ImageRepeater.DataBind();
        }

        #region Methods (render PDF)

        public static PrintOutputFile RenderPdf(BankOptions options)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(options.BankID);
            if (bank == null || bank.Tenant != options.OrganizationID)
                return null;

            ImageInfo[] images;

            if (options.Output == OutputType.Addendum)
            {
                var addendum = bank.Specifications
                    .SelectMany(x => x.EnumerateAllForms().SelectMany(y => y.Addendum.EnumerateAllItems()));

                images = GetInfoByAssetNumber(bank, addendum);
            }
            else
            {
                var imageAttachments = bank.Attachments.Where(x => x.Type == AttachmentType.Image).ToArray();

                images = ConvertToInfo(imageAttachments);
            }

            return RenderPdf(new ControlData
            {
                Output = options.Output,
                FitPageSize = true,
                Title = (bank.Content?.Title?.Default).IfNullOrEmpty(bank.Name),
                Code = $"{bank.Edition}{bank.Asset}",
                Images = images
            }, options);
        }

        public static PrintOutputFile RenderPdf(FormOptions options)
        {
            var form = ServiceLocator.BankSearch.GetFormData(options.FormID);
            if (form == null || form.Specification.Bank.Tenant != options.OrganizationID)
                return null;

            ImageInfo[] images;

            if (options.Output == OutputType.Addendum)
            {
                var addendum = form.Addendum.EnumerateAllItems();

                images = GetInfoByAssetNumber(form.Specification.Bank, addendum);

                for (var i = 0; i < images.Length; i++)
                    images[i].Sequence = i + 1;
            }
            else
            {
                var attachmentFilter = form.Sections
                    .SelectMany(x => x.Fields)
                    .SelectMany(y => y.Question.AttachmentIdentifiers)
                    .ToHashSet();
                var attachments = form.Specification.Bank.Attachments
                    .Where(x => x.Type == AttachmentType.Image && attachmentFilter.Contains(x.Identifier))
                    .ToArray();

                images = ConvertToInfo(attachments);
            }

            return RenderPdf(new ControlData
            {
                Output = options.Output,
                FitPageSize = false,
                Title = (form.Content?.Title?.Default).IfNullOrEmpty(form.Name),
                Code = $"{form.Code}{form.Asset}0{form.AssetVersion}",
                Images = images
            }, options);
        }

        private static PrintOutputFile RenderPdf(ControlData data, Options options)
        {
            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (ImagePrint)page.LoadControl("~/UI/Admin/Assessments/Attachments/Controls/ImagePrint.ascx");

                report.LoadData(data);

                var htmlBuilder = new StringBuilder();

                using (var writer = new StringWriter(htmlBuilder))
                {
                    using (var htmlWriter = new HtmlTextWriter(writer))
                        report.RenderControl(htmlWriter);
                }

                var htmlString = HtmlHelper.ResolveRelativePaths(options.CurrentUrl, htmlBuilder);

                var fileName = data.Output == OutputType.Addendum
                    ? $"{StringHelper.Sanitize(data.Title, '_')}_addendum_{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}"
                    : $"{StringHelper.Sanitize(data.Title, '_')}_{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}";

                var fileData = HtmlConverter.HtmlToPdf(htmlString, new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    EnableSmartShrinking = false,
                    EnablePrintMediaType = true,
                    EnableJavaScript = false,

                    Dpi = 400,
                    PageSize = PageSizeType.Letter,
                    MarginTop = 19.7f,
                    MarginRight = 13f,
                    MarginBottom = 19.7f,
                    MarginLeft = 13f,
                    HeaderSpacing = 7,
                    Zoom = (float)DefaultZoom,

                    HeaderUrl = options.HeaderUrl,
                    FooterUrl = options.FooterUrl,
                    Variables = new HtmlConverterSettings.Variable[]
                    {
                        new HtmlConverterSettings.Variable("footer_title", data.Code),
                        new HtmlConverterSettings.Variable("header_title", data.Title),
                    }
                });

                return new PrintOutputFile(fileName, fileData);
            }
        }

        #endregion

        #region Methods (helpers)

        private static ImageInfo[] GetInfoByAssetNumber(BankState bank, IEnumerable<FormAddendumItem> addendum)
        {
            var attachmentMapping = bank.EnumerateAllAttachments()
                .ToDictionary(x => (x.Asset, x.AssetVersion), x => x);
            var addendumAttachments = addendum
                .Select(x => (x.Asset, x.Version))
                .Distinct()
                .Where(x => attachmentMapping.ContainsKey(x))
                .Select(x => attachmentMapping[x])
                .ToArray();

            return ConvertToInfo(addendumAttachments);
        }

        private static ImageInfo[] ConvertToInfo(IEnumerable<Attachment> attachments)
        {
            var uploads = UploadSearch
                .Bind(x => new { x.UploadIdentifier, x.Name, x.NavigateUrl, x.ContentSize, x.Uploaded }, attachments)
                .ToDictionary(x => x.UploadIdentifier);

            return attachments
                .Where(x => uploads.ContainsKey(x.Upload))
                .Select(x => new ImageInfo(x, "/files" + uploads[x.Upload].NavigateUrl))
                .ToArray();
        }

        #endregion
    }
}