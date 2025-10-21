using System;
using System.Data.Entity;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    internal static class StandardDocumentFilterHelper
    {
        public static IQueryable<Standard> ApplyFilter(InternalDbContext db, StandardDocumentFilter filter)
        {
            var query = db.Standards.AsQueryable().AsNoTracking();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.StandardIdentifiers != null && filter.StandardIdentifiers.Length > 0)
                query = query.Where(x => filter.StandardIdentifiers.Contains(x.StandardIdentifier));

            if (filter.StandardType.IsNotEmpty())
                query = query.Where(x => x.StandardType == filter.StandardType);

            if (filter.DocumentType.IsNotEmpty())
                query = query.Where(x => x.DocumentType == filter.DocumentType);

            if (filter.DepartmentGroupIdentifier.HasValue)
                query = query.Where(x => x.DepartmentGroupIdentifier == filter.DepartmentGroupIdentifier.Value);

            if (filter.Title.IsNotEmpty())
            {
                query = query.Where(x =>
                    x.ContentTitle.Contains(filter.Title)
                    || db.TContents.Any(y => y.ContainerIdentifier == x.StandardIdentifier && y.ContentLabel == ContentLabel.Title && y.ContentText.Contains(filter.Title))
                );
            }

            if (filter.Level.IsNotEmpty())
                query = query.Where(x => x.LevelType == filter.Level);

            if (filter.Keyword.IsNotEmpty())
                query = query.Where(
                    x => db.TContents.Any(
                        y => y.ContainerIdentifier == x.StandardIdentifier
                             && (y.ContentText.Contains(filter.Keyword) || y.ContentHtml.Contains(filter.Keyword))));

            if (filter.Posted != null)
            {
                if (filter.Posted.Since.HasValue)
                    query = query.Where(x => x.DatePosted >= filter.Posted.Since.Value);

                if (filter.Posted.Before.HasValue)
                    query = query.Where(x => x.DatePosted < filter.Posted.Before.Value);
            }

            if (filter.IsPortal == true)
                query = query.Where(x => x.IsTemplate || x.CreatedBy == filter.CreatedBy);
            else if (filter.CreatedBy.HasValue)
                query = query.Where(x => x.CreatedBy == filter.CreatedBy.Value);

            if (filter.IsTemplate.HasValue)
                query = query.Where(x => x.IsTemplate == filter.IsTemplate.Value);

            if (filter.PrivacyScope != null)
            {
                if (filter.PrivacyScope.Name.IsNotEmpty())
                {
                    if (filter.PrivacyScope.Name == "User")
                        query = query.Where(x => x.StandardPrivacyScope == "User" && x.CreatedBy == filter.PrivacyScope.User);
                    else
                        query = query.Where(x => x.StandardPrivacyScope == null || x.StandardPrivacyScope == filter.PrivacyScope.Name);
                }

            }

            return query;
        }
    }
}
