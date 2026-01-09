using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Forms.Utilities
{
    internal static class AddendumHelper
    {
        public class RepeaterDataItem
        {
            public int AssetNumber { get; set; }
            public int AssetVersion { get; set; }
            public string TypeName { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
        }

        public static RepeaterDataItem[][] GetRepeaterDataSource(params IEnumerable<Attachment>[] groups)
        {
            var uploads = UploadSearch
                .Bind(x => new { x.UploadIdentifier, x.Name, x.NavigateUrl, x.ContentSize, x.Uploaded }, groups.SelectMany(x => x))
                .ToDictionary(x => x.UploadIdentifier);

            return groups
                .Select(group => group
                    .Where(x => uploads.ContainsKey(x.Upload))
                    .Select(x =>
                    {
                        var upload = uploads[x.Upload];

                        return new RepeaterDataItem
                        {
                            AssetNumber = x.Asset,
                            AssetVersion = x.AssetVersion,
                            TypeName = x.Type.GetName(),
                            Title = (x.Content?.Title.Default).IfNullOrEmpty("(Untitled)"),
                            Url = "/files" + upload.NavigateUrl
                        };
                    })
                    .ToArray())
                .ToArray();
        }
    }
}