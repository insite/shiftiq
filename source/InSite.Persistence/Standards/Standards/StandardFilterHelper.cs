using System;
using System.Data.Entity.SqlServer;
using System.Linq;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    internal static class StandardFilterHelper
    {
        public static IQueryable<Standard> ApplyFilter(IQueryable<Standard> query, StandardFilter filter, InternalDbContext db)
        {
            if (filter.SelectorText.IsNotEmpty())
            {
                var separatorIndex = filter.SelectorText.IndexOf(":");

                if (separatorIndex < 0)
                {
                    query = query.Where(x =>
                        SqlFunctions.PatIndex(filter.SelectorText, SqlFunctions.StringConvert((double)x.AssetNumber).Trim()) > 0
                        || x.ContentTitle.Contains(filter.SelectorText)
                        || x.Code.Contains(filter.SelectorText));
                }
                else
                {
                    var number = filter.SelectorText.Substring(0, separatorIndex);
                    var title = separatorIndex < filter.SelectorText.Length - 2
                        ? filter.SelectorText.Substring(separatorIndex + 2)
                        : null;

                    query = query.Where(x => SqlFunctions.StringConvert((double)x.AssetNumber).EndsWith(number));

                    if (title.IsNotEmpty())
                        query = query.Where(x => x.ContentTitle.Contains(title));
                }
            }

            if (filter.Number.HasValue)
                query = query.Where(x => x.AssetNumber == filter.Number);

            if (filter.StandardLabel.IsNotEmpty())
                query = query.Where(x => x.StandardLabel == filter.StandardLabel);

            if (filter.DepartmentGroupIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.DepartmentGroupIdentifiers.Contains(x.DepartmentGroupIdentifier.Value));

            if (filter.StandardIdentifiers != null && filter.StandardIdentifiers.Length > 0)
                query = query.Where(x => filter.StandardIdentifiers.Contains(x.StandardIdentifier));

            if (filter.StandardTier.IsNotEmpty())
                query = query.Where(x => x.StandardTier.Contains(filter.StandardTier));

            if (filter.ParentStandardIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.ParentStandardIdentifiers.Contains(x.Parent.StandardIdentifier));

            if (filter.StandardTypes.IsNotEmpty())
                query = query.Where(x => filter.StandardTypes.Contains(x.StandardType));

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.Title.IsNotEmpty())
                query = query.Where(x => x.ContentTitle.Contains(filter.Title) ||
                    db.TContents.Any(
                        y => y.ContainerIdentifier == x.StandardIdentifier
                            && y.ContentLabel == ContentLabel.Title
                            && (y.ContentText.Contains(filter.Title) || y.ContentHtml.Contains(filter.Title))
                    )
                );

            if (filter.ParentTitle.IsNotEmpty())
                query = query.Where(x => x.Parent.ContentTitle.Contains(filter.ParentTitle));

            if (filter.ContentName.IsNotEmpty())
                query = query.Where(x => x.ContentName.Contains(filter.ContentName));

            if (filter.Modified != null)
            {
                if (filter.Modified.Since.HasValue)
                    query = query.Where(x => x.Modified >= filter.Modified.Since.Value);

                if (filter.Modified.Before.HasValue)
                    query = query.Where(x => x.Modified <= filter.Modified.Before.Value);
            }

            if (filter.IsHidden.HasValue)
                query = query.Where(x => x.IsHidden == filter.IsHidden.Value);

            if (filter.PortalUserIdentifier.HasValue)
                query = query.Where(x => x.IsTemplate || x.CreatedBy == filter.PortalUserIdentifier.Value);

            if (filter.IsPublished.HasValue)
                query = query.Where(x => x.IsPublished == filter.IsPublished.Value);

            if (filter.Inclusions.IsNotEmpty())
                query = query.Where(x => filter.Inclusions.Contains(x.StandardIdentifier));

            if (filter.Exclusions.IsHidden.HasValue)
                query = query.Where(x => x.IsHidden != filter.Exclusions.IsHidden.Value);

            if (filter.Exclusions.IsPublished.HasValue)
                query = query.Where(x => x.IsPublished != filter.Exclusions.IsPublished.Value);

            if (filter.Exclusions.StandardIdentifier.Count > 0)
                query = query.Where(x => !filter.Exclusions.StandardIdentifier.Contains(x.StandardIdentifier));

            if (filter.Exclusions.StandardIdentifier.Count > 0)
                query = query.Where(x => !filter.Exclusions.StandardIdentifier.Contains(x.StandardIdentifier));

            if (filter.Exclusions.StandardType.Count > 0)
                query = query.Where(x => !filter.Exclusions.StandardType.Contains(x.StandardType));

            if (filter.Code.IsNotEmpty())
                query = query.Where(x => x.Code.Contains(filter.Code));

            if (filter.HasCode.HasValue)
            {
                if (filter.HasCode.Value)
                    query = query.Where(x => x.Code != null);
                else
                    query = query.Where(x => x.Code == null);
            }

            if (filter.HasChildren.HasValue)
                query = filter.HasChildren.Value
                    ? query.Where(x => x.Children.Count > 0)
                    : query.Where(x => x.Children.Count == 0);

            if (filter.HasParent.HasValue)
                query = filter.HasParent.Value
                    ? query.Where(x => x.ParentStandardIdentifier != null)
                    : query.Where(x => x.ParentStandardIdentifier == null);

            switch (filter.Scope)
            {
                case StandardTypeEnum.Framework:
                    query = query.Where(x => x.ParentStandardIdentifier == null);
                    break;
                case StandardTypeEnum.Competency:
                    query = query.Where(x => x.ParentStandardIdentifier != null && x.Children.FirstOrDefault() == null);
                    break;
                case StandardTypeEnum.Cluster:
                    query = query.Where(x => x.ParentStandardIdentifier != null && x.Children.FirstOrDefault() != null);
                    break;
            }

            if (filter.Tags != null)
            {
                var tagExpr = PredicateBuilder.True<Standard>();

                foreach (var tag in filter.Tags)
                {
                    var tag1 = "\"" + tag + "\"";

                    tagExpr = tagExpr.And(x => x.Tags.Contains(tag1));
                }

                query = query.Where(tagExpr.Expand());
            }

            if (filter.ValidationUserIdentifier.HasValue)
                query = query.Where(x => x.StandardValidations.Any(y => y.UserIdentifier == filter.ValidationUserIdentifier.Value));

            if (filter.DocumentType.IsNotEmpty())
                query = query.Where(x => filter.DocumentType.Contains(x.DocumentType));

            if (filter.RootStandardIdentifier.HasValue)
                query = ApplyRootStandard(query, filter.RootStandardIdentifier.Value, db);

            if (filter.DepartmentIdentifier.HasValue)
                query = query.Where(x =>
                    x.DepartmentCompetencies.Any(y => y.DepartmentIdentifier == filter.DepartmentIdentifier.Value)
                 || x.DepartmentUsers.Any(y => y.DepartmentIdentifier == filter.DepartmentIdentifier.Value));

            if (filter.Keyword.IsNotEmpty())
                query = query.Where(
                    x => db.TContents.Any(
                        y => y.ContainerIdentifier == x.StandardIdentifier
                             && (y.ContentText.Contains(filter.Keyword) || y.ContentHtml.Contains(filter.Keyword))));

            if (filter.IsPortal ?? false)
                query = query.Where(x => x.IsTemplate || x.CreatedBy == filter.CreatedBy);

            if (filter.CategoryItemIdentifier.HasValue)
                query = query.Where(x => x.CategoryItemIdentifier == filter.CategoryItemIdentifier);

            return query;
        }

        private static IQueryable<Standard> ApplyRootStandard(IQueryable<Standard> query, Guid rootStandardIdentifier, InternalDbContext db)
        {
            var realRoot = db.StandardHierarchies
                .Where(x => x.StandardIdentifier == rootStandardIdentifier)
                .Select(x => new { x.RootStandardIdentifier, x.PathKey })
                .FirstOrDefault();

            if (realRoot?.RootStandardIdentifier == null)
                return query.Where(x => 1 == 0);

            if (rootStandardIdentifier == realRoot.RootStandardIdentifier)
            {
                query = query.Where(x =>
                    db.StandardHierarchies.Any(y =>
                        y.RootStandardIdentifier == rootStandardIdentifier
                        && y.StandardIdentifier == x.StandardIdentifier
                    )
                );
            }
            else
            {
                query = query.Where(x =>
                    db.StandardHierarchies.Any(y =>
                        y.RootStandardIdentifier == realRoot.RootStandardIdentifier
                        && y.PathKey.StartsWith(realRoot.PathKey)
                        && y.StandardIdentifier == x.StandardIdentifier
                    )
                );
            }

            return query;
        }
    }
}

