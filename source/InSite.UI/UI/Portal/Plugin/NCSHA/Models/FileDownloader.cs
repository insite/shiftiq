using System;
using System.IO;

using Shift.Common;

namespace InSite.UI.Variant.NCSHA.Reports
{
    public static class FileDownloader
    {
        public static void DownloadFile(System.Web.HttpResponse response, string filename, Action<Stream> write, int length, string mime)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (string.IsNullOrEmpty(mime))
                mime = MimeMapping.GetContentType(filename);

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.AddHeader("Content-Disposition", $"attachment; filename={filename}");
            response.AddHeader("Content-Length", length.ToString());

            response.ContentType = mime;

            write(response.OutputStream);

            response.End();
        }
    }
}