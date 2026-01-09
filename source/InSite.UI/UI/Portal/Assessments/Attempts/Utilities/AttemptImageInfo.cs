using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using InSite.Common.Web.Infrastructure;
using InSite.Domain.Banks;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AttemptImageInfo
    {
        public static readonly Regex ImageUrlPattern = new Regex("^(https?://)(dev-|local-|sandbox-)?([^\\.]+)\\.([^/]+)/files(/.+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        public int Height { get; set; }

        public static Dictionary<string, AttemptImageInfo> CreateDictionary(IEnumerable<Attachment> attachments)
        {
            var uploads = UploadSearch.Bind(
                x => new { x.UploadIdentifier, x.NavigateUrl, },
                attachments.Where(x => x.Type == AttachmentType.Image && x.Image.TargetOnline?.HasValue == true));

            var attachmentMapping = attachments
                .GroupBy(x => x.Upload)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault(y => y.IsLastVersion()));

            var images = new Dictionary<string, AttemptImageInfo>();

            foreach (var upload in uploads)
            {
                var url = FileHelper.GetUrl(upload.NavigateUrl);

                var urlMatch = ImageUrlPattern.Match(url);
                if (urlMatch.Success)
                    url = urlMatch.Groups[3].Value + urlMatch.Groups[5].Value;

                var attachment = attachmentMapping[upload.UploadIdentifier];
                if (attachment?.Image != null)
                {
                    var targetOnline = attachment.Image.TargetOnline;

                    images.Add(url.ToLower(), new AttemptImageInfo
                    {
                        Width = targetOnline.Width,
                        Height = targetOnline.Height,
                    });
                }
            }

            return images;
        }
    }
}