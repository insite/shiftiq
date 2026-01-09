using System;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    internal static class UploadFilterHelper
    {
        public static IQueryable<Upload> ApplyFilter(this IQueryable<Upload> query, UploadFilter filter, InternalDbContext db)
        {
            if (string.IsNullOrEmpty(filter.ContainerType) || (
                filter.ContainerType != UploadContainerType.Asset
                && filter.ContainerType != UploadContainerType.ContactExperience
                && filter.ContainerType != UploadContainerType.Oganization
                && filter.ContainerType != UploadContainerType.Workflow))
                throw new ArgumentException($"Unsupported ContainerType = '{filter.ContainerType}'");

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier && x.ContainerType == filter.ContainerType);

            if (filter.UploadType.IsNotEmpty())
                query = query.Where(x => filter.UploadType.Contains(x.UploadType));

            if (filter.Keyword.IsNotEmpty())
                query = query.Where(x => x.Name.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) || x.Title.Contains(filter.Keyword));

            if (filter.PostedSince.HasValue)
                query = query.Where(x => x.Uploaded >= filter.PostedSince.Value);

            if (filter.PostedBefore.HasValue)
                query = query.Where(x => x.Uploaded < filter.PostedBefore.Value);

            return query;
        }
    }
}
