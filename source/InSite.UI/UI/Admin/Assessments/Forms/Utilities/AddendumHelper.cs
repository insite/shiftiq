using System;
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

        public static RepeaterDataItem[][] GetRepeaterDataSource(Guid bankId, params IEnumerable<Attachment>[] groups)
        {
            var uploads = UploadSearch
                .Bind(x => new { Identifier = x.UploadIdentifier, NavigateUrl = "/files" + x.NavigateUrl }, groups.SelectMany(x => x));

            var files = ServiceLocator.FileSearch
                .GetModels(null, bankId, null, false)
                .Select(x => new { Identifier = x.FileIdentifier, NavigateUrl = ServiceLocator.StorageService.GetFileUrl(x) })
                .ToList();

            files.AddRange(uploads);

            var dictionary = files.ToDictionary(x => x.Identifier);

            return groups
                .Select(group => group
                    .Where(x => dictionary.ContainsKey(x.FileIdentifier ?? x.Upload))
                    .Select(x =>
                    {
                        var upload = dictionary[x.FileIdentifier ?? x.Upload];

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