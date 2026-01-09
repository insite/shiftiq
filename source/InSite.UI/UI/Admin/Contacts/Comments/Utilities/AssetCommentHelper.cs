using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Humanizer;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.Infrastructure
{
    public static class AssetCommentHelper
    {
        public static string ConvertToHtml_When(DateTimeOffset postedOn)
        {
            return $"posted {postedOn.UtcDateTime.Humanize()}";
        }

        public static string GetDescription(string description, string attachmentName, string attachmentPath, long? attachmentSize)
        {
            // Start with the file description.
            description = StringHelper.TrimAndClean(description);

            // Apply a markdown transformation on the text.
            var html = Markdown.ToHtml(description) ?? string.Empty;

            var attachmentUrl = UploadSearch.ExistsByOrganizationIdentifier(CurrentSessionState.Identity.Organization.Identifier, attachmentPath)
                ? FileHelper.GetUrl(attachmentPath)
                : "#";
            var isImage = FileExtension.IsImage(Path.GetExtension(attachmentUrl));

            var text = new StringBuilder();

            if (isImage)
                text.Append(@"<div class='attached-image-description'>");

            // If there are multiple paragraphs then the download link should appear in its own paragraph. Otherwise it
            // should appear immediately below the description.
            var paragraphCount = Regex.Matches(html, @"</p>").Count;

            if (!string.IsNullOrEmpty(description) && paragraphCount == 1)
            {
                var pEndIndex = html.IndexOf("</p>");

                text.Append(html.Substring(0, pEndIndex)).Append("<br/>");

                WriteFileLink(text, attachmentUrl, attachmentName, attachmentPath, attachmentSize);

                text.Append(html.Substring(pEndIndex));
            }
            else
            {
                text.Append(html).Append("<p>");

                WriteFileLink(text, attachmentUrl, attachmentName, attachmentPath, attachmentSize);

                text.Append("</p>");
            }

            if (isImage)
                text.AppendFormat(@"
    <div class='attached-image'>
        <div>
            <a href='{0}'><img src='{0}' alt='' style='max-width:100px;max-height:100px;'/></a>
        </div>
    </div>
</div>", attachmentUrl);

            return text.ToString();
        }

        private static void WriteFileLink(StringBuilder sb, string url, string name, string path, long? size)
        {
            if (size == null)
                return;

            if (url != "#")
            {
                sb.AppendFormat(
                    "<i class='far fa-download'></i> <a target='_blank' href='{1}'>{0}</a> <span class='form-text'>({2} KB)</span>",
                    name ?? "&nbsp;",
                    url,
                    (size.Value / 1024D).ToString("n0")
                );
            }
            else
            {
                using (var writer = new StringWriter(sb))
                    writer.Write($"<span class='text-danger'><i class='fas fa-circle'></i> File Not Found: {path}</span>");
            }
        }
    }
}