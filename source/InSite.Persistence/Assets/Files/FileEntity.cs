using System;
using System.Linq.Expressions;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class FileEntity
    {
        public Guid UploadId { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string ContentFingerprint { get; set; }
        public string NavigateUrl { get; set; }
        public DateTimeOffset Posted { get; set; }
        public string AccessControlType { get; set; }

        public static readonly Expression<Func<Upload, FileEntity>> BindExpr = LinqExtensions1.Expr((Upload x) => new FileEntity
        {
            UploadId = x.UploadIdentifier,
            OrganizationIdentifier = x.OrganizationIdentifier,
            Name = x.Name,
            ContentType = x.ContentType,
            ContentFingerprint = x.ContentFingerprint,
            NavigateUrl = x.NavigateUrl,
            Posted = x.Uploaded,
            AccessControlType = x.UploadPrivacyScope,
        });
    }
}