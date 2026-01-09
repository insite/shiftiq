using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web.Infrastructure;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Web.Infrastructure
{
    public static class MarkdownHelper
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class UploadResult
        {
            [JsonProperty(PropertyName = "links")]
            public List<string> Links { get; } = new List<string>();

            [JsonProperty(PropertyName = "errors")]
            public List<string> Errors { get; } = new List<string>();

            public List<string> Paths { get; } = new List<string>();
        }

        public static UploadResult TryUploadFile(Page page, FileUpload upload, Guid organization, string path)
        {
            return TryUploadFile(page, upload, (fileName, stream) =>
            {
                var filePath = path;
                if (!filePath.EndsWith("/"))
                    filePath += "/";
                filePath += fileName;

                FileHelper.Provider.Save(organization, filePath, stream);

                return filePath;
            });
        }

        public static UploadResult TryUploadFile(Page page, FileUpload upload, Func<string, Stream, string> save)
        {
            const int maxFileSize = 2 * 1024 * 1024;

            var result = new UploadResult();

            if (!page.IsValid)
            {
                foreach (IValidator validator in page.Validators)
                {
                    if (!validator.IsValid)
                        result.Errors.Add(validator.ErrorMessage);
                }
            }
            else if (upload.PostedFile.ContentLength == 0)
            {
                result.Errors.Add("The uploaded file is empty.");
            }
            else if (maxFileSize > 0 && upload.PostedFile.ContentLength > maxFileSize)
            {
                result.Errors.Add($"You cannot upload a file larger than {maxFileSize.Bytes().Humanize("0.##")}.");
            }
            else
            {
                try
                {
                    var file = upload.PostedFile;
                    var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                    var fileExt = Path.GetExtension(file.FileName);

                    var path = save($"{fileName}{fileExt}", file.InputStream);

                    result.Paths.Add(path);
                    result.Links.Add($"{(FileExtension.IsImage(fileExt) ? "!" : string.Empty)}[{fileName}{fileExt}]({FileHelper.GetUrl(path)})");
                }
                catch (ApplicationError kex)
                {
                    result.Errors.Add(kex.Message);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);
                    result.Errors.Add("An error occurred on the server side");
                }
            }

            return result;
        }
    }
}