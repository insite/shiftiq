using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsAchievementSearch
    {
        #region Classes

        public class CountModel
        {
            public string Type { get; set; }
            public int Count { get; set; }
        }

        private class SelectorItem
        {
            public Guid Value { get; set; }
            public string AchievementLabel { get; set; }
            public string AchievementTitle { get; set; }
            public int? Count { get; set; }
        }

        public class CredentialExpirationModel
        {
            public CredentialExpirationModel(VCredential credential)
            {
                Credential = credential;
            }

            public Guid OrganizationIdentifier { get; set; }
            public string OrganizationName { get; set; }
            public string DepartmentNames { get; set; }
            public VCredential Credential { get; }
        }

        public class CompetencyAchievement
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementLabel { get; set; }
            public string AchievementTitle { get; set; }
            public DateTimeOffset? DateCompleted { get; set; }
            public bool IsShiftCourse { get; set; }
            public Guid? UserIdentifier { get; set; }
            public string ValidationStatus { get; set; }
            public bool? EnableSignOff { get; set; }
            public Guid? UploadContainerIdentifier { get; set; }
            public string UploadName { get; set; }
            public string UploadTitle { get; set; }
            public string UploadType { get; set; }
        }

        private class AchievementReadHelper : ReadHelper<VCmdsAchievement>
        {
            public static readonly AchievementReadHelper Instance = new AchievementReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VCmdsAchievement>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VCmdsAchievements.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static VCmdsAchievement Select(Guid id, params Expression<Func<VCmdsAchievement, object>>[] includes)
        {
            return AchievementReadHelper.Instance.SelectFirst(x => x.AchievementIdentifier == id, includes);
        }

        public static VCmdsAchievement SelectFirst(Expression<Func<VCmdsAchievement, bool>> filter,
            params Expression<Func<VCmdsAchievement, object>>[] includes)
        {
            return AchievementReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<VCmdsAchievement> Select(
            Expression<Func<VCmdsAchievement, bool>> filter,
            params Expression<Func<VCmdsAchievement, object>>[] includes)
        {
            return AchievementReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<VCmdsAchievement> Select(
            Expression<Func<VCmdsAchievement, bool>> filter,
            string sortExpression,
            params Expression<Func<VCmdsAchievement, object>>[] includes)
        {
            return AchievementReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<VCmdsAchievement, T>> binder,
            Expression<Func<VCmdsAchievement, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VCmdsAchievement, T>> binder,
            Expression<Func<VCmdsAchievement, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<VCmdsAchievement, bool>> filter)
        {
            return AchievementReadHelper.Instance.Count(filter);
        }

        #endregion

        #region SELECT

        public static List<VCmdsAchievement> SelectByCategoryIdentifier(Guid categoryIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsAchievementCategories
                    .Where(x => x.CategoryIdentifier == categoryIdentifier)
                    .Select(x => x.Achievement)
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static DataTable SelectForDepartmentChecklist(Guid organizationId, Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.Departments
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .Select(x => new
                    {
                        DepartmentIdentifier = x.DepartmentIdentifier,
                        DepartmentName = x.DepartmentName,
                        IsChecked = db.VCmdsAchievementDepartments.Any(y => y.DepartmentIdentifier == x.DepartmentIdentifier && y.AchievementIdentifier == achievementIdentifier)
                    })
                    .OrderBy("DepartmentName")
                    .ToDataTable();
            }
        }

        public static IEnumerable<CountModel> CountCredentialsByAchievementLabel(Guid department)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsCredentials
                    .Where(
                      x => x.Achievement.Departments.Any(y => y.DepartmentIdentifier == department)
                        && x.UserUtcArchived == null
                        && x.User.Memberships.Any(z => z.GroupIdentifier == department && z.MembershipType == "Department")
                        )
                    .GroupBy(x => x.AchievementLabel)
                    .Select(x => new CountModel
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();
            }
        }

        public static CredentialExpirationModel[] SelectCredentialExpirations(ReminderType reminderType)
        {
            var now = DateTimeOffset.Now;
            var twoWeeksAgo = now.AddDays(-14);

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 90;

                var query = db.VCredentials
                    .Where(x => x.UserEmailEnabled && (
                        x.OrganizationIdentifier == OrganizationIdentifiers.CMDS || x.ParentOrganizationIdentifier == OrganizationIdentifiers.CMDS
                    ));

                if (reminderType == ReminderType.Today)
                    query = query.Where(x => twoWeeksAgo <= x.CredentialExpirationReminderRequested0 && x.CredentialExpirationReminderRequested0 <= now && !x.CredentialExpirationReminderDelivered0.HasValue);
                else if (reminderType == ReminderType.InOneMonth)
                    query = query.Where(x => twoWeeksAgo <= x.CredentialExpirationReminderRequested1 && x.CredentialExpirationReminderRequested1 <= now && !x.CredentialExpirationReminderDelivered1.HasValue);
                else if (reminderType == ReminderType.InTwoMonths)
                    query = query.Where(x => twoWeeksAgo <= x.CredentialExpirationReminderRequested2 && x.CredentialExpirationReminderRequested2 <= now && !x.CredentialExpirationReminderDelivered2.HasValue);
                else if (reminderType == ReminderType.InThreeMonths)
                    query = query.Where(x => twoWeeksAgo <= x.CredentialExpirationReminderRequested3 && x.CredentialExpirationReminderRequested3 <= now && !x.CredentialExpirationReminderDelivered3.HasValue);
                else
                    throw new NotImplementedException();

                var expirations = query.ToArray();

                var organizationAchievements = db.TAchievementOrganizations
                    .Where(x => x.Organization.AccountClosed == null)
                    .SelectMany(x => x.Organization.Departments
                        .Select(y => new
                        {
                            AchievementIdentifier = x.AchievementIdentifier,
                            OrganizationIdentifier = x.OrganizationIdentifier,
                            CompanyName = x.Organization.CompanyTitle,
                            DepartmentIdentifier = y.DepartmentIdentifier,
                            DepartmentName = y.DepartmentName
                        })
                    )
                    .ToArray()
                    .GroupBy(x => new Tuple<Guid, Guid>(x.AchievementIdentifier, x.DepartmentIdentifier))
                    .ToDictionary(x => x.Key, x => x.Single());

                var memberships = db.Memberships
                    .Where(
                        x => x.Group.GroupType == GroupTypes.Department
                          && (x.MembershipType == "Department" || x.MembershipType == "Company"))
                    .Select(x => new
                    {
                        x.GroupIdentifier,
                        x.UserIdentifier
                    })
                    .ToArray()
                    .GroupBy(x => x.UserIdentifier)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.GroupIdentifier).ToArray());

                var result = new List<CredentialExpirationModel>();

                foreach (var item in expirations)
                {
                    if (!memberships.ContainsKey(item.UserIdentifier))
                        continue;

                    var organizations = memberships[item.UserIdentifier]
                        .Select(department =>
                        {
                            var key = new Tuple<Guid, Guid>(item.AchievementIdentifier, department);
                            return organizationAchievements.ContainsKey(key) ? organizationAchievements[key] : null;
                        })
                        .Where(x => x != null)
                        .GroupBy(x => x.OrganizationIdentifier);

                    if (!organizations.Any())
                        continue;

                    foreach (var group in organizations)
                    {
                        var first = group.First();

                        result.Add(new CredentialExpirationModel(item)
                        {
                            OrganizationIdentifier = first.OrganizationIdentifier,
                            OrganizationName = first.CompanyName,
                            DepartmentNames = string.Join(", ", group.Select(x => x.DepartmentName).OrderBy(x => x))
                        });
                    }
                }

                return result.ToArray();
            }
        }

        #endregion

        #region SELECT for resource category

        public static List<AchievementListGridItem> SelectNewCategoryAchievements(
            Guid categoryIdentifier,
            string searchText,
            Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                var categoryOrganizationIdentifier = db.VCmdsAchievementCategories
                    .Where(x => x.CategoryIdentifier == categoryIdentifier)
                    .FirstOrDefault()?.OrganizationIdentifier;

                if (categoryOrganizationIdentifier == null)
                    categoryOrganizationIdentifier = organization;

                var category = db.VAchievementCategories.FirstOrDefault(x => x.CategoryIdentifier == categoryIdentifier);

                var categoryName = category?.CategoryName;

                var query = db.VCmdsAchievements
                    .Where(x => x.OrganizationIdentifier == categoryOrganizationIdentifier && !x.Categories.Any(y => y.CategoryIdentifier == categoryIdentifier));

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.AchievementTitle.Contains(searchText));

                return query
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility,
                        CategoryName = categoryName
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectCategoryAchievements(Guid categoryIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsAchievementCategories
                    .Where(x => x.CategoryIdentifier == categoryIdentifier)
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.Achievement.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.Achievement.AchievementTitle,
                        AchievementLabel = x.Achievement.AchievementLabel,
                        Visibility = x.Achievement.Visibility,
                        CategoryName = x.CategoryName
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        #endregion

        #region Filtering

        public static List<VCmdsAchievement> SelectByFilter(VCmdsAchievementFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.AchievementLabel)
                    .ThenBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        private class SearchResultItem
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementLabel { get; set; }
            public string ValidForUnit { get; set; }
            public int? ValidForCount { get; set; }
            public int UploadCount { get; set; }

            public static readonly Expression<Func<InternalDbContext, VCmdsAchievement, SearchResultItem>> BinderWithUploadCount = LinqExtensions1.Expr((InternalDbContext db, VCmdsAchievement x) => new SearchResultItem
            {
                AchievementIdentifier = x.AchievementIdentifier,
                AchievementTitle = x.AchievementTitle,
                AchievementLabel = x.AchievementLabel,
                ValidForUnit = x.ValidForUnit,
                ValidForCount = x.ValidForCount,
                UploadCount = db.Uploads.Where(y => y.ContainerIdentifier == x.AchievementIdentifier).Count()
                              + db.UploadRelations.Where(y => y.ContainerIdentifier == x.AchievementIdentifier).Count()
            });

            public static readonly Expression<Func<VCmdsAchievement, SearchResultItem>> Binder = LinqExtensions1.Expr((VCmdsAchievement x) => new SearchResultItem
            {
                AchievementIdentifier = x.AchievementIdentifier,
                AchievementTitle = x.AchievementTitle,
                AchievementLabel = x.AchievementLabel,
                ValidForUnit = x.ValidForUnit,
                ValidForCount = x.ValidForCount
            });
        }

        public static DataTable SelectSearchResults(VCmdsAchievementFilter filter, bool includeUploadCount)
        {
            DataTable table;

            using (var db = new InternalDbContext())
            {
                var query = includeUploadCount
                    ? CreateQuery(filter, db).Select(LinqExtensions1.Expr((VCmdsAchievement x) => SearchResultItem.BinderWithUploadCount.Invoke(db, x)).Expand())
                    : CreateQuery(filter, db).Select(SearchResultItem.Binder);

                table = query
                    .OrderBy(x => x.AchievementLabel)
                    .ThenBy(x => x.AchievementTitle)
                    .ApplyPaging(filter)
                    .ToDataTable();
            }

            PrepareForSearchResults(filter, table);

            return table;
        }

        public static DataTable SelectSearchResults(IEnumerable<Guid> ids, VCmdsAchievementFilter filter, bool includeUploadCount)
        {
            DataTable table;

            using (var db = new InternalDbContext())
            {
                var query = includeUploadCount
                    ? CreateQuery(filter, db).Select(LinqExtensions1.Expr((VCmdsAchievement x) => SearchResultItem.BinderWithUploadCount.Invoke(db, x)).Expand())
                    : CreateQuery(filter, db).Select(SearchResultItem.Binder);

                query = query.Where(x => ids.Contains(x.AchievementIdentifier));

                table = query.OrderBy("AchievementLabel, AchievementTitle").ToDataTable();
            }

            PrepareForSearchResults(filter, table);

            return table;
        }

        private static void PrepareForSearchResults(VCmdsAchievementFilter filter, DataTable table)
        {
            table.Columns.Add("SelectorText");

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var type = row["AchievementLabel"] as string;
                    var label = AchievementTypes.Pluralize(type, filter.OrganizationCode);

                    row["AchievementLabel"] = string.IsNullOrEmpty(type) ? string.Empty : label;
                    row["SelectorText"] = StringHelper.Acronym(type) + ": " + (string)row["AchievementTitle"];
                }
            }
        }

        public static int CountSearchResults(VCmdsAchievementFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        private static IQueryable<VCmdsAchievement> CreateQuery(VCmdsAchievementFilter filter, InternalDbContext db)
        {
            var query = db.VCmdsAchievements.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AchievementVisibility))
                query = query.Where(x => x.Visibility == filter.AchievementVisibility);

            if (filter.ExcludeHidden ?? true)
                query = query.Where(x => x.AchievementIsEnabled);

            if (filter.AchievementOrganizationIdentifier.HasValue)
            {
                if (filter.GlobalOrCompanySpecific && (string.IsNullOrEmpty(filter.AchievementVisibility) || filter.AchievementVisibility == AccountScopes.Enterprise))
                {
                    query = query.Where(x => x.OrganizationIdentifier == filter.AchievementOrganizationIdentifier || x.Visibility == AccountScopes.Enterprise);
                }
                else
                {
                    query = query.Where(x => x.OrganizationIdentifier == filter.AchievementOrganizationIdentifier);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.AchievementCategory)) // && filter.AchievementType != "Module")
                query = query.Where(x => x.Categories.Any(y => y.CategoryName.Contains(filter.AchievementCategory)));

            if (filter.CategoryIdentifier.HasValue)
                query = query.Where(x => x.Categories.Any(y => y.CategoryIdentifier == filter.CategoryIdentifier));

            if (filter.DepartmentIdentifier.HasValue)
            {
                query = query.Where(x => x.Departments.Any(y => y.DepartmentIdentifier == filter.DepartmentIdentifier));
            }
            else if (filter.OrganizationIdentifier.HasValue)
            {
                if (filter.GlobalOrCompanySpecific)
                {
                    query = query.Where(x => x.Visibility == AccountScopes.Enterprise || x.OrganizationIdentifier == filter.OrganizationIdentifier || x.Organizations.Any(y => y.OrganizationIdentifier == filter.OrganizationIdentifier));
                }
                else
                {
                    query = query.Where(x => x.Organizations.Any(y => x.OrganizationIdentifier == filter.OrganizationIdentifier || y.OrganizationIdentifier == filter.OrganizationIdentifier));
                }
            }

            if (filter.ExcludeAchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier != filter.ExcludeAchievementIdentifier);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(x => x.AchievementTitle.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Description))
                query = query.Where(x => x.AchievementDescription.Contains(filter.Description));

            if (!string.IsNullOrEmpty(filter.AchievementType))
                query = query.Where(x => x.AchievementLabel == filter.AchievementType);

            if (filter.AllowSelfDeclared != null)
                query = query.Where(x => x.AchievementAllowSelfDeclared == filter.AllowSelfDeclared);

            if (filter.IsTimeSensitive.HasValue)
            {
                if (filter.IsTimeSensitive.Value)
                    query = query.Where(x => x.ValidForCount != null);
                else
                    query = query.Where(x => x.ValidForCount == null);
            }

            return query;
        }

        #endregion

        #region SelectForSelector

        public static string SelectTextForSelector(Guid id, string organizationCode)
        {
            var info = Select(id);

            if (info == null)
                return null;

            return StringHelper.Acronym(AchievementTypes.Pluralize(info.AchievementLabel, organizationCode));
        }

        public static DataTable SelectForSelector(VCmdsAchievementFilter filter, bool includeUserResource)
        {
            DataTable table;

            using (var db = new InternalDbContext())
            {
                var query = CreateSelectorQuery(filter, includeUserResource, db);

                table = query
                    .OrderBy("AchievementLabel, AchievementTitle")
                    .ApplyPaging(filter)
                    .ToDataTable();
            }

            PrepareForSelector(filter, table, includeUserResource);

            return !string.IsNullOrEmpty(filter.AchievementCategory)
                ? SortByAssetTags(table, filter.AchievementCategory)
                : table;

        }

        public static DataTable SelectForSelector(Guid[] ids, VCmdsAchievementFilter filter, bool includeUserResource)
        {
            DataTable table;

            using (var db = new InternalDbContext())
            {
                var query = CreateSelectorQuery(filter, includeUserResource, db);

                table = query
                    .Where(x => ids.Contains(x.Value))
                    .OrderBy("AchievementLabel, AchievementTitle")
                    .ToDataTable();
            }

            PrepareForSelector(filter, table, includeUserResource);

            return table;
        }

        private static void PrepareForSelector(VCmdsAchievementFilter filter, DataTable table, bool includeUserResource)
        {
            table.Columns.Add("Text");

            foreach (DataRow row in table.Rows)
            {
                var text = StringHelper.Acronym(AchievementTypes.Pluralize(row["AchievementLabel"] as string, filter.OrganizationCode))
                                + ": "
                                + (string)row["AchievementTitle"];

                if (includeUserResource)
                    text += $" ({row["Count"]})";

                row["Text"] = text;
            }
        }

        public static int CountForSelector(VCmdsAchievementFilter filter, bool includeUserResource)
        {
            using (var db = new InternalDbContext())
                return CreateSelectorQuery(filter, includeUserResource, db).Count();
        }

        private static IQueryable<SelectorItem> CreateSelectorQuery(VCmdsAchievementFilter filter, bool includeUserResource, InternalDbContext db)
        {
            var query = CreateQuery(filter, db);

            if (filter.DepartmentIdentifier == null || !includeUserResource)
            {
                return query
                    .Select(x => new SelectorItem
                    {
                        Value = x.AchievementIdentifier,
                        AchievementLabel = x.AchievementLabel,
                        AchievementTitle = x.AchievementTitle
                    });
            }

            var countQuery = db
                .VCmdsCredentials
                .Join(db.ActiveUsers,
                    a => a.UserIdentifier,
                    b => b.UserIdentifier,
                    (a, b) => a
                )
                .Join(db.Memberships.Where(x => x.MembershipType == "Department" && x.GroupIdentifier == filter.DepartmentIdentifier),
                    a => a.UserIdentifier,
                    b => b.UserIdentifier,
                    (a, b) => a.AchievementIdentifier
                )
                .GroupBy(x => x)
                .Select(x => new
                {
                    AchievementIdentifier = x.Key,
                    Count = x.Count()
                });

            return query
                .Join(countQuery,
                    a => a.AchievementIdentifier,
                    b => b.AchievementIdentifier,
                    (a, b) => new SelectorItem
                    {
                        Value = a.AchievementIdentifier,
                        AchievementLabel = a.AchievementLabel,
                        AchievementTitle = a.AchievementTitle,
                        Count = b.Count
                    }
                )
                .Where(x => x.Count > 0);
        }

        private static DataTable SortByAssetTags(DataTable table, string categoryName)
        {
            table.Columns.Add("Sequence", typeof(int));

            using (var db = new InternalDbContext())
            {
                foreach (DataRow row in table.Rows)
                {
                    var id = (Guid)row["Value"];

                    var classification = db.VAchievementClassifications
                        .FirstOrDefault(x => x.AchievementIdentifier == id && x.Category.CategoryName == categoryName);

                    if (classification != null)
                        row["Sequence"] = classification.ClassificationSequence ?? 0;
                }
            }

            var dv = table.DefaultView;
            dv.Sort = "Sequence, Text";
            return dv.ToTable();
        }

        #endregion

        #region SELECT (Old)

        public static DataTable SelectRelatedGroups(Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var query1 = db.VCmdsAchievementOrganizations
                    .Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .Join(db.Organizations,
                        a => a.OrganizationIdentifier,
                        b => b.OrganizationIdentifier,
                        (a, b) => new
                        {
                            CompanyName = b.CompanyTitle,
                            DepartmentIdentifier = (Guid?)null,
                            DepartmentName = (string)null
                        }
                    );

                var query2 = db.VCmdsAchievementDepartments
                    .Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .Join(db.Departments,
                        a => a.DepartmentIdentifier,
                        b => b.DepartmentIdentifier,
                        (a, b) => new
                        {
                            CompanyName = b.Organization.CompanyTitle,
                            DepartmentIdentifier = (Guid?)b.DepartmentIdentifier,
                            DepartmentName = b.DepartmentName
                        }
                    );

                return query1
                    .Union(query2)
                    .OrderBy("CompanyName, DepartmentName")
                    .ToDataTable();
            }
        }

        public static DataTable SelectRelatedCredentials(Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsCredentials
                    .Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .Select(x => new
                    {
                        x.UserIdentifier,
                        FullName = x.UserFullName
                    })
                    .OrderBy("FullName")
                    .ToDataTable();
            }
        }

        public static DataTable SelectRelatedGradebooks(Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.QGradebooks
                    .Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .Select(x => new
                    {
                        x.GradebookIdentifier,
                        x.GradebookTitle
                    })
                    .OrderBy("GradebookTitle")
                    .ToDataTable();
            }
        }

        public static DataTable SelectRelatedGradeItems(Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.QGradeItems
                    .Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .Select(x => new
                    {
                        x.GradeItemIdentifier,
                        x.GradeItemName
                    })
                    .OrderBy("GradeItemName")
                    .ToDataTable();
            }
        }

        public static List<CompetencyAchievement> SelectCompetencyAchievements(Guid competencyStandardIdentifier, Guid userIdentifier, Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var query1 = db.VCmdsAchievementCompetencies
                    .Where(x => x.CompetencyStandardIdentifier == competencyStandardIdentifier
                        && (
                            x.Achievement.OrganizationIdentifier == organizationId
                            || x.Achievement.Visibility == AccountScopes.Enterprise
                            || x.Achievement.Visibility == null
                            || x.Achievement.Organizations.Any(y => y.OrganizationIdentifier == organizationId)
                        )
                    )
                    .SelectMany(x => x.Achievement.Credentials.Where(y => y.UserIdentifier == userIdentifier).DefaultIfEmpty().Select(y => new
                    {
                        Achievement = x.Achievement,
                        Credential = y
                    }))
                    .GroupJoin(db.Uploads,
                        a => a.Achievement.AchievementIdentifier,
                        b => b.ContainerIdentifier,
                        (a, b) => new
                        {
                            Achievement = a.Achievement,
                            Credential = a.Credential,
                            Uploads = b.DefaultIfEmpty()
                        }
                    )
                    .SelectMany(x => x.Uploads.Select(y => new
                    {
                        Achievement = x.Achievement,
                        Credential = x.Credential,
                        Upload = y
                    }));

                var uploadRelations = db.UploadRelations
                    .Join(db.Organizations.Where(x => x.OrganizationIdentifier == organizationId),
                        a => a.Upload.ContainerIdentifier,
                        b => b.OrganizationIdentifier,
                        (a, b) => a
                    );

                var query2 = db.VCmdsAchievementCompetencies
                    .Where(x => x.CompetencyStandardIdentifier == competencyStandardIdentifier)
                    .SelectMany(x => x.Achievement.Credentials.Where(y => y.UserIdentifier == userIdentifier).DefaultIfEmpty().Select(y => new
                    {
                        Achievement = x.Achievement,
                        Credential = y
                    }))
                    .Join(uploadRelations,
                        a => a.Achievement.AchievementIdentifier,
                        b => b.ContainerIdentifier,
                        (a, b) => new
                        {
                            Achievement = a.Achievement,
                            Credential = a.Credential,
                            Upload = b.Upload
                        }
                    );

                var unionQuery = query1
                    .Union(query2)
                    .OrderBy(x => x.Achievement.AchievementLabel)
                    .ThenBy(x => x.Achievement.AchievementTitle);

                var achievements = unionQuery
                    .Select(x => new CompetencyAchievement
                    {
                        AchievementIdentifier = x.Achievement.AchievementIdentifier,
                        AchievementLabel = x.Achievement.AchievementLabel,
                        AchievementTitle = x.Achievement.AchievementTitle,
                        DateCompleted = x.Credential.CredentialGranted,
                        IsShiftCourse = x.Achievement.AchievementLabel == "Module",
                        UserIdentifier = x.Credential.UserIdentifier,
                        ValidationStatus = x.Credential.CredentialStatus,
                        EnableSignOff = true,
                        UploadContainerIdentifier = x.Upload.ContainerIdentifier,
                        UploadName = x.Upload.Name,
                        UploadTitle = x.Upload.Title,
                        UploadType = x.Upload.UploadType
                    })
                    .ToList();

                var departments = MembershipSearch.Bind(
                    x => x.GroupIdentifier,
                    x => x.UserIdentifier == userIdentifier
                      && x.Group.GroupType == GroupTypes.Department
                      && x.Group.OrganizationIdentifier == organizationId);

                var departmentAchievements = db.TAchievementDepartments
                    .Where(da => departments.Any(d => d == da.DepartmentIdentifier))
                    .Select(a => a.AchievementIdentifier)
                    .ToList();

                return achievements.Where(a => a.AchievementLabel == "Module" || departmentAchievements.Any(da => da == a.AchievementIdentifier)).ToList();
            }
        }

        public static List<AchievementListGridItem> SelectNewDepartmentAchievements(Guid department, string searchText)
        {
            using (var db = new InternalDbContext())
            {
                var organizationId = db.Departments.Where(x => x.DepartmentIdentifier == department).FirstOrDefault().OrganizationIdentifier;

                var query = db.VCmdsAchievements
                    .Where(x =>
                        (x.OrganizationIdentifier == organizationId
                            || (x.Visibility == null || x.Visibility == AccountScopes.Enterprise)
                                && x.Organizations.Any(y => y.OrganizationIdentifier == organizationId)
                        )
                        && !x.Departments.Any(y => y.DepartmentIdentifier == department)
                    );

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.AchievementTitle.Contains(searchText));

                return query
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectDepartmentAchievements(Guid department)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsAchievementDepartments
                    .Where(x => x.DepartmentIdentifier == department)
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.Achievement.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.Achievement.AchievementTitle,
                        AchievementLabel = x.Achievement.AchievementLabel,
                        Visibility = x.Achievement.Visibility
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectAllNewDepartmentTemplateAchievements(Guid? department, Guid organization)
        {
            var organizations = new[] { organization, CurrentPartition.OrganizationId };

            using (var db = new InternalDbContext())
            {
                var query = department.HasValue
                    ? db.VCmdsAchievementDepartments
                        .Where(x => x.DepartmentIdentifier == department.Value)
                        .Select(x => x.Achievement)
                    : db.VCmdsAchievements
                        .Where(x => organizations.Contains(x.OrganizationIdentifier));

                return query
                    .SelectMany(achievement => achievement.Categories.DefaultIfEmpty(),
                        (achievement, category) => new { achievement, category })
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.achievement.OrganizationIdentifier,
                        AchievementIdentifier = x.achievement.AchievementIdentifier,
                        AchievementTitle = x.achievement.AchievementTitle,
                        AchievementLabel = x.achievement.AchievementLabel,
                        Visibility = x.achievement.Visibility,
                        CategoryName = x.category != null ? x.category.CategoryName : null
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectNewDepartmentTemplateAchievements(
            Guid organizationId,
            Guid departmentId,
            Guid programId,
            string searchText)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsAchievements.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.AchievementTitle.Contains(searchText));

                return query
                    .Where(x =>
                        (
                            (x.Visibility == AccountScopes.Enterprise &&
                             x.Organizations.Any(y => y.OrganizationIdentifier == organizationId))
                            ||
                            (departmentId != Guid.Empty &&
                             x.Departments.Any(y => y.DepartmentIdentifier == departmentId))
                            ||
                            (departmentId == Guid.Empty &&
                             x.OrganizationIdentifier == organizationId)
                        ) &&
                        !db.TTasks.Any(y => y.ProgramIdentifier == programId &&
                        y.ObjectIdentifier == x.AchievementIdentifier)
                    )
                    .SelectMany(x => x.Categories.DefaultIfEmpty().Select(y => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility,
                        CategoryName = y.CategoryName
                    }))
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectDepartmentTemplateAchievements(
            Guid enterpriseId, Guid organizationId, string scope, string keyword,
            Guid programId)
        {
            using (var db = new InternalDbContext())
            {
                var items = db.TTasks
                    .Where(x => x.ProgramIdentifier == programId)
                    .Join(db.VCmdsAchievements,
                        a => a.ObjectIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => b
                    )
                    .SelectMany(x => x.Categories.DefaultIfEmpty().Select(y => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility,
                        CategoryName = y.CategoryName
                    }));

                if (scope == AccountScopes.Enterprise && enterpriseId != Guid.Empty)
                    items = items.Where(x => x.OrganizationIdentifier == enterpriseId);

                else if (scope == AccountScopes.Partition && enterpriseId != Guid.Empty)
                    items = items.Where(x =>
                        x.OrganizationIdentifier == enterpriseId ||
                        x.OrganizationIdentifier == organizationId);

                else
                    items = items.Where(x => x.OrganizationIdentifier == organizationId);

                if (!string.IsNullOrEmpty(keyword))
                    items = items.Where(x => x.AchievementTitle.Contains(keyword));

                return items
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectNewOrganizationAchievements(
            Guid enterpriseId,
            Guid organizationId,
            string scope,
            string keyword)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsAchievements.Where(x => !x.Organizations.Any(y => y.OrganizationIdentifier == organizationId));

                if (scope == AccountScopes.Enterprise && enterpriseId != Guid.Empty)
                    query = query.Where(x => x.Visibility == null || x.Visibility == AccountScopes.Enterprise);

                else if (scope == AccountScopes.Organization)
                    query = query.Where(x => x.OrganizationIdentifier == organizationId);

                else
                    query = query.Where(x => x.Visibility == null || x.Visibility == AccountScopes.Enterprise || x.OrganizationIdentifier == organizationId);

                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.AchievementTitle.Contains(keyword));

                return query
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectOrganizationAchievements(
            Guid enterpriseId, Guid organizationId, string scope, string keyword, Guid? departmentId = null)
        {
            using (var db = new InternalDbContext())
            {
                var items = db.VCmdsAchievements
                    .Include(x => x.Categories)
                    .AsNoTracking()
                    .AsQueryable();

                if (scope == AccountScopes.Enterprise && enterpriseId != Guid.Empty)
                    items = items.Where(x => x.OrganizationIdentifier == enterpriseId);

                else if (scope == AccountScopes.Partition && enterpriseId != Guid.Empty)
                    items = items.Where(x =>
                        x.OrganizationIdentifier == enterpriseId ||
                        x.OrganizationIdentifier == organizationId ||
                        x.Organizations.Any(y => y.OrganizationIdentifier == organizationId));

                else
                    items = items.Where(x => x.OrganizationIdentifier == organizationId);

                if (departmentId.HasValue)
                    items = items.Where(x => x.Departments.Any(d => d.DepartmentIdentifier == departmentId));

                if (!string.IsNullOrEmpty(keyword))
                    items = items.Where(x => x.AchievementTitle.Contains(keyword));

                var list = items
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility,
                        CategoryName = x.Categories.Any()
                            ? x.Categories.FirstOrDefault().CategoryName
                            : "No Category"
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();

                return list;
            }
        }

        public static List<AchievementListGridItem> SelectNewUploadAchievements(Guid? organizationId, Guid uploadId, string searchText)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsAchievements
                    .Where(x => !db.UploadRelations.Any(y => y.UploadIdentifier == uploadId && y.ContainerIdentifier == x.AchievementIdentifier));

                if (organizationId.HasValue)
                    query = query.Where(x => x.Organizations.Any(y => y.OrganizationIdentifier == organizationId));

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.AchievementTitle.Contains(searchText));

                return query
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility
                    })
                    .OrderBy(x => x.AchievementLabel).ThenBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectUploadRelatedAchievement(Guid uploadId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsAchievements
                    .Join(db.UploadRelations.Where(x => x.UploadIdentifier == uploadId),
                        a => a.AchievementIdentifier,
                        b => b.ContainerIdentifier,
                        (a, b) => new AchievementListGridItem
                        {
                            OrganizationIdentifier = a.OrganizationIdentifier,
                            AchievementIdentifier = a.AchievementIdentifier,
                            AchievementTitle = a.AchievementTitle,
                            AchievementLabel = a.AchievementLabel,
                            Visibility = a.Visibility
                        }
                    )
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectNewCompetencyAchievements(Guid competencyId, string searchText)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsAchievements
                    .Where(x => !x.Competencies.Any(y => y.CompetencyStandardIdentifier == competencyId));

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.AchievementTitle.Contains(searchText));

                return query
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.AchievementTitle,
                        AchievementLabel = x.AchievementLabel,
                        Visibility = x.Visibility
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<AchievementListGridItem> SelectCompetencyAchievements(Guid competencyId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsAchievementCompetencies
                    .Where(x => x.CompetencyStandardIdentifier == competencyId)
                    .Select(x => new AchievementListGridItem
                    {
                        OrganizationIdentifier = x.Achievement.OrganizationIdentifier,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.Achievement.AchievementTitle,
                        AchievementLabel = x.Achievement.AchievementLabel,
                        Visibility = x.Achievement.Visibility
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static ListItemArray SelectAchievementLabels(string organization, string[] inclusions, string[] exclusions)
        {
            var list = new ListItemArray();

            var all = inclusions;

            var items = AchievementTypes.Collect();

            var labels = all.Where(i => items.Contains(i) && (exclusions == null || !exclusions.Any(e => i == e)));

            foreach (var label in labels)
            {
                var value = AchievementTypes.Pluralize(label, organization);
                list.Add(label, value);
            }

            return list;
        }

        public static ListItemArray SelectAchievementLabels(string organization, string[] exclusions)
        {
            var list = new ListItemArray();

            var items = AchievementTypes.Collect();

            var labels = items.Where(i => items.Contains(i) && (exclusions == null || !exclusions.Any(e => i == e)));

            foreach (var label in labels)
            {
                var value = AchievementTypes.Pluralize(label, organization);
                list.Add(label, value);
            }

            return list;
        }

        public static Dictionary<string, string> SearchAchievementTypesInUse(
            Guid organizationId,
            string organizationCode,
            string[] labels = null)
        {
            if (labels == null)
                labels = ServiceLocator.AchievementSearch.GetAchievementLabels(organizationId);

            var types = SelectAchievementLabels(organizationCode, labels, null);

            var result = types.Items
                .Where(x => !string.Equals(x.Value, "Other Achievement", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    string value = AchievementTypes.Pluralize(x.Value, organizationCode);
                    return new { Key = x.Value, Value = Shift.Common.Humanizer.Pluralize(value) };
                })
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

            result.Add("Other Achievement", "Other Achievement");

            return result;
        }

        #endregion

        public static void PublishGlobalModule()
        {
            const string query = "EXEC custom_cmds.PublishGlobalModule";

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(query);
            }
        }
    }
}