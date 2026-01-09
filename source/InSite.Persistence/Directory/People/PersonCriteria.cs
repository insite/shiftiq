using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Constant.Enumerations;

namespace InSite.Persistence
{
    public static class PersonCriteria
    {
        public static List<Person> Select(PersonFilter filter, params Expression<Func<Person, object>>[] includes)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "User.FirstName,User.LastName,User.Email";

            using (var db = new InternalDbContext())
            {
                return ApplyPersonFilter(filter, db.Persons.AsQueryable().AsNoTracking(), db)
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .OrderBy(orderBy)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public static Person SelectFirst(PersonFilter filter, params Expression<Func<Person, object>>[] includes)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "User.FirstName,User.LastName,User.Email";

            using (var db = new InternalDbContext())
            {
                return ApplyPersonFilter(filter, db.Persons.AsQueryable().AsNoTracking(), db)
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .OrderBy(orderBy)
                    .FirstOrDefault();
            }
        }

        public static PersonPortalSearchResultItem[] SelectForPortalSearch(PersonFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return ApplyPersonFilter(filter, db.Persons.AsQueryable().AsNoTracking(), db)
                    .OrderByDescending(x => x.Created)
                    .ApplyPaging(filter)
                    .Select(x => new PersonPortalSearchResultItem
                    {
                        UserIdentifier = x.UserIdentifier,
                        PersonCode = x.PersonCode,
                        Email = x.User.Email,
                        FullName = x.User.FullName,
                        Referred = x.Created,
                        LastAuthenticated = x.LastAuthenticated,
                    })
                    .ToArray();
            }
        }

        public static T[] Bind<T>(Expression<Func<Person, T>> binder, PersonFilter filter) =>
            PersonReadHelper.Instance.Bind(binder, filter, filter.OrderBy);

        public static T BindFirst<T>(
            Expression<Func<Person, T>> binder, PersonFilter filter,
            string modelSort = null, string entitySort = null) =>
            PersonReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static bool Exists(PersonFilter filter)
        {
            using (var db = new InternalDbContext())
                return ApplyPersonFilter(filter, db.Persons.AsQueryable(), db).Any();
        }

        public static int Count(PersonFilter filter)
        {
            using (var db = new InternalDbContext())
                return ApplyPersonFilter(filter, db.Persons.AsQueryable(), db).Count();
        }

        public static List<PersonSearchResultItem> SelectSearchResults(PersonFilter filter)
        {
            var sortExpression = filter.OrderBy.IfNullOrEmpty("FullName");

            using (var db = new InternalDbContext())
            {
                var list = ApplyPersonFilter(filter, db.Persons.AsQueryable().AsNoTracking(), db)
                    .Select(x => new PersonSearchResultItem
                    {
                        UserIdentifier = x.UserIdentifier,
                        FirstName = x.User.FirstName,
                        FullName = x.User.FullName,
                        Email = x.User.Email,
                        EmailAlternate = x.User.EmailAlternate,
                        EmailEnabled = x.EmailEnabled,
                        LastName = x.User.LastName,
                        Honorific = x.User.Honorific,
                        ModifiedBy = x.ModifiedBy,
                        UserPasswordHash = x.User.UserPasswordHash,
                        Phone = x.Phone,
                        PhoneHome = x.PhoneHome,
                        Birthdate = x.Birthdate,
                        ImageUrl = x.User.ImageUrl,
                        JobTitle = x.JobTitle,
                        IsArchived = x.IsArchived || x.User.UtcArchived.HasValue,
                        IsApproved = x.UserAccessGranted.HasValue,
                        UserAccessGranted = x.UserAccessGranted,
                        UserAccessGrantedBy = x.UserAccessGrantedBy,
                        EmployerGroupIdentifier = x.EmployerGroupIdentifier,
                        EmployerGroupCode = x.EmployerGroup.GroupCode,
                        EmployerGroupName = x.EmployerGroup.GroupName,
                        EmployerGroupRegion = x.EmployerGroup.GroupRegion,
                        EmployerDistrictIdentifier = x.EmployerGroup.Parent.GroupIdentifier,
                        EmployerDistrictName = x.EmployerGroup.Parent.GroupName,
                        EmployerDistrictAccountNumber = x.EmployerGroup.Parent.GroupCode,
                        StatusText = x.MembershipStatus.ItemName,
                        StatusHtmlColor = "#888888",
                        Created = x.Created,
                        Modified = x.Modified,
                        HomeAddress = x.HomeAddress,
                        WorkAddress = x.WorkAddress,
                        ShippingAddress = x.ShippingAddress,
                        SessionCount = db.TUserSessions.Count(y => y.UserIdentifier == x.UserIdentifier && y.SessionIsAuthenticated),
                        PersonCode = x.PersonCode,
                        LastAuthenticated = x.LastAuthenticated,
                        Region = x.Region
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToList();

                return FilterIssues(db, FilterCommentFlags(db, list, filter), filter);
            }
        }

        private static List<PersonSearchResultItem> FilterCommentFlags(InternalDbContext db, List<PersonSearchResultItem> list, PersonFilter filter)
        {
            if (filter.CommentsFlag.IsEmpty())
                return list;

            var result = new List<PersonSearchResultItem>();

            foreach (var item in list)
            {
                foreach (var flag in filter.CommentsFlag)
                {
                    switch (flag)
                    {
                        case 1: TryAddItem(x => x.CommentPosted != null); break;
                        case 2: TryAddItem(x => x.CommentFlagged != null); break;
                        case 3: TryAddItem(x => x.CommentSubmitted != null); break;
                        case 4: TryAddItem(x => x.CommentResolved != null); break;
                    }
                }

                void TryAddItem(Expression<Func<QComment, bool>> fieldFilter)
                {
                    if (db.QComments.Where(x => x.AuthorUserIdentifier == item.UserIdentifier).Where(fieldFilter).Any())
                        result.Add(item);
                }
            }

            return result;
        }

        private static List<PersonSearchResultItem> FilterIssues(InternalDbContext db, List<PersonSearchResultItem> list, PersonFilter filter)
        {
            if (filter.PersonIssue == Shift.Constant.Enumerations.PersonCaseType.None)
                return list;

            var result = new List<PersonSearchResultItem>();

            foreach (var item in list)
            {
                switch (filter.PersonIssue)
                {
                    case PersonCaseType.OpenCaseAssignee:
                        TryAddItem(x => x.TopicUserIdentifier == item.UserIdentifier && x.IssueClosed == null);
                        break;
                    case PersonCaseType.ClosedCaseAssignee:
                        TryAddItem(x => x.TopicUserIdentifier == item.UserIdentifier && x.IssueClosed != null);
                        break;
                    case PersonCaseType.OpenCaseAdministrator:
                        TryAddItem(x => x.AdministratorUserIdentifier == item.UserIdentifier && x.IssueClosed != null);
                        break;
                    case PersonCaseType.ClosedCaseAdministrator:
                        TryAddItem(x => x.AdministratorUserIdentifier == item.UserIdentifier && x.IssueClosed != null);
                        break;
                }

                void TryAddItem(Expression<Func<QIssue, bool>> fieldFilter)
                {
                    if (db.QIssues.Any(fieldFilter))
                        result.Add(item);
                }
            }

            return result;
        }

        public static SearchResultList SelectForGrid(PersonFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return ApplyPersonFilter(filter, db.Persons.AsQueryable().AsNoTracking(), db)
                    .Select(x => new
                    {
                        x.UserIdentifier,
                        x.OrganizationIdentifier,
                        x.User.FirstName,
                        x.User.LastName,
                        x.User.FullName,
                        x.User.Email,
                        OrganizationName = x.Organization.CompanyName,
                        WorkAddress = new { x.WorkAddress.City, x.WorkAddress.Province },
                        SingleCompanyName = x.User.Persons.Count > 1
                            ? "(Multiple Companies)"
                            : x.Organization.CompanyName
                    })
                    .OrderBy("FullName")
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        #region Classes

        private class PersonReadHelper : ReadHelper<Person>
        {
            public static readonly PersonReadHelper Instance = new PersonReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Person>, TResult> func)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var query = db.Persons.AsNoTracking().AsQueryable();

                    return func(query);
                }
            }

            public T[] Bind<T>(
                Expression<Func<Person, T>> binder,
                PersonFilter filter,
                string modelSort = null,
                string entitySort = null)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var modelQuery = BuildQuery(
                        db.Persons.AsQueryable().AsNoTracking(),
                        (IQueryable<Person> q) => q.Select(binder),
                        (IQueryable<Person> query) => ApplyPersonFilter(filter, query, db),
                        q => q,
                        filter.Paging, modelSort, entitySort, false);

                    return modelQuery.ToArray();
                }
            }

            public T BindFirst<T>(
                Expression<Func<Person, T>> binder,
                PersonFilter filter,
                string modelSort = null,
                string entitySort = null)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var modelQuery = BuildQuery(
                        db.Persons.AsQueryable().AsNoTracking(),
                        (IQueryable<Person> q) => q.Select(binder),
                        (IQueryable<Person> query) => ApplyPersonFilter(filter, query, db),
                        q => q,
                        null, modelSort, entitySort, false);

                    return modelQuery.FirstOrDefault();
                }
            }
        }

        #endregion

        #region Methods (PersonFilter)

        private static IQueryable<Person> ApplyPersonFilter(PersonFilter filter, IQueryable<Person> query, InternalDbContext db)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);
            else if (filter.OrganizationOrParentIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationOrParentIdentifier || x.Organization.ParentOrganizationIdentifier == filter.OrganizationOrParentIdentifier);
            else
                throw new ApplicationError("Both OrganizationIdentifier and OrganizationOrParentIdentifier are empty");

            if (filter.CodeContains.IsNotEmpty())
                query = query.Where(x => x.PersonCode.Contains(filter.CodeContains));

            if (filter.CodesExact.IsNotEmpty())
                query = query.Where(x => filter.CodesExact.Contains(x.PersonCode));

            if (filter.CodeNotExact.IsNotEmpty())
                query = query.Where(x => x.PersonCode != filter.CodeNotExact);

            if (filter.FullName.HasValue())
            {
                switch (filter.NameFilterType)
                {
                    case "Exact":
                        query = query.Where(x =>
                            (x.User.FirstName + " " + x.User.LastName).Contains(filter.FullName)
                            || (x.User.LastName + ", " + x.User.FirstName).Contains(filter.FullName)
                            || x.User.FullName.Contains(filter.FullName));
                        break;
                    case "ExactName":
                        query = query.Where(x => x.User.FirstName == filter.FullName || x.User.LastName == filter.FullName || x.User.FullName == filter.FullName);
                        break;
                    case "Similar":
                        var nameSoundex = Pronunciation.Soundex(filter.FullName, 4, 0);

                        query = query.Where(x =>
                            x.User.FullName.Contains(filter.FullName)
                            || (x.User.FirstName + " " + x.User.LastName).Contains(filter.FullName)
                            || (x.User.LastName + ", " + x.User.FirstName).Contains(filter.FullName)
                            || x.User.SoundexFirstName == nameSoundex
                            || x.User.SoundexLastName == nameSoundex
                        );
                        break;
                    default:
                        throw new NotImplementedException("Filter Not Found: " + filter.NameFilterType);
                }
            }

            if (filter.FirstName.HasValue())
            {
                switch (filter.NameFilterType)
                {
                    case "Exact":
                        query = query.Where(x => x.User.FirstName.Contains(filter.FirstName));
                        break;
                    case "ExactName":
                        query = query.Where(x => x.User.FirstName == filter.FirstName);
                        break;
                    case "Similar":
                        var firstNameSoundex = Pronunciation.Soundex(filter.FirstName, 4, 0);

                        query = query.Where(x => x.User.FirstName.Contains(filter.FirstName) || x.User.SoundexFirstName == firstNameSoundex);
                        break;
                    default:
                        throw new NotImplementedException("Filter Not Found: " + filter.NameFilterType);
                }
            }

            if (filter.LastName.HasValue())
            {
                switch (filter.NameFilterType)
                {
                    case "Exact":
                        query = query.Where(x => x.User.LastName.Contains(filter.LastName));
                        break;
                    case "ExactName":
                        query = query.Where(x => x.User.LastName == filter.LastName);
                        break;
                    case "Similar":
                        var lastNameSoundex = Pronunciation.Soundex(filter.LastName, 4, 0);

                        query = query.Where(x => x.User.LastName.Contains(filter.LastName) || x.User.SoundexLastName == lastNameSoundex);
                        break;
                    default:
                        throw new NotImplementedException("Filter Not Found: " + filter.NameFilterType);
                }
            }

            if (filter.IncludeUserIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.IncludeUserIdentifiers.Contains(x.UserIdentifier));

            if (filter.ExcludeUserIdentifiers.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeUserIdentifiers.Contains(x.UserIdentifier));

            if (filter.EmployerGroups.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Select(y => y.GroupIdentifier).Intersect(filter.EmployerGroups).Any());

            switch (filter.EmailStatus)
            {
                case "Valid Email":
                    query = query.Where(x => x.User.Email != null && DbFunctions.Like(x.User.Email, Pattern.ValidEmailLike));
                    break;
                case "Invalid Email":
                    query = query.Where(x => x.User.Email != null && !DbFunctions.Like(x.User.Email, Pattern.ValidEmailLike));
                    break;
            }

            if (filter.EmailNotEndsWith.IsNotEmpty())
                query = query.Where(x => !x.User.Email.EndsWith(filter.EmailNotEndsWith));

            if (filter.IsEmailPatternValid == true)
                query = query.Where(x => DbFunctions.Like(x.User.Email, Pattern.ValidEmailLike));
            else if (filter.IsEmailPatternValid == false)
                query = query.Where(x => !DbFunctions.Like(x.User.Email, Pattern.ValidEmailLike));

            if (filter.IsPasswordAssigned.HasValue)
            {
                query = filter.IsPasswordAssigned.Value
                    ? query.Where(x => x.User.UserPasswordHash != null)
                    : query.Where(x => x.User.UserPasswordHash == null);
            }

            if (filter.IsApproved.HasValue)
            {
                query = filter.IsApproved.Value
                    ? query.Where(x => x.UserAccessGranted.HasValue)
                    : query.Where(x => !x.UserAccessGranted.HasValue);
            }

            if (filter.IsConsentToShare.HasValue)
            {
                query = filter.IsConsentToShare.Value
                   ? query.Where(x => x.ConsentToShare == "Yes")
                   : query.Where(x => x.ConsentToShare == null || x.ConsentToShare != "Yes");
            }

            if (filter.IsJobsApproved.HasValue)
            {
                query = filter.IsJobsApproved.Value
                    ? query.Where(x => x.JobsApproved.HasValue)
                    : query.Where(x => !x.JobsApproved.HasValue);
            }

            if (filter.IsArchived.HasValue)
            {
                query = filter.IsArchived.Value
                    ? query.Where(x => x.IsArchived || x.User.UtcArchived.HasValue)
                    : query.Where(x => !x.IsArchived && !x.User.UtcArchived.HasValue);
            }

            if (filter.IsCmds.HasValue)
                query = query.Where(x => x.User.AccessGrantedToCmds == filter.IsCmds);

            if (filter.EmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.EmployerGroupIdentifier == filter.EmployerGroupIdentifier.Value);

            if (filter.EmployerParentGroupIdentifier.HasValue)
                query = query.Where(x => x.EmployerGroup.ParentGroupIdentifier == filter.EmployerParentGroupIdentifier.Value);

            if (filter.IsMultiOrganization.HasValue && filter.IsMultiOrganization.Value)
                query = query.Where(x => x.User.Persons.Count > 1);

            if (filter.OrganizationPersonTypes.IsNotEmpty())
            {
                var isAdministrator = filter.OrganizationPersonTypes.Contains(OrganizationPersonTypes.Administrator);
                var isLearner = filter.OrganizationPersonTypes.Contains(OrganizationPersonTypes.Learner);

                if (isAdministrator && isLearner)
                    query = query.Where(x => x.IsAdministrator && x.IsLearner);
                else if (isAdministrator)
                    query = query.Where(x => x.IsAdministrator);
                else if (isLearner)
                    query = query.Where(x => x.IsLearner);
            }

            if (filter.EmailContains.IsNotEmpty())
                query = query.Where(x => x.User.Email.Contains(filter.EmailContains));

            if (filter.EmailsExact.IsNotEmpty())
                query = query.Where(x => filter.EmailsExact.Contains(x.User.Email));

            if (filter.EmailAlternateContains.IsNotEmpty())
                query = query.Where(x => x.User.EmailAlternate.Contains(filter.EmailAlternateContains));

            if (filter.EmailDomains.IsNotEmpty())
            {
                var domains = StringHelper.Split(filter.EmailDomains);
                query = query.Where(x => domains.Any(d => x.User.Email.EndsWith(d)));
                query = query.Where(x => !x.User.Email.StartsWith(x.UserIdentifier.ToString().Substring(0, 8)));
            }

            if (filter.EmailOrEmailAlternateEnabled != null)
            {
                query = query.Where(x => x.EmailEnabled == filter.EmailOrEmailAlternateEnabled.Value
                                      || x.EmailAlternateEnabled == filter.EmailOrEmailAlternateEnabled.Value);
            }
            else
            {
                if (filter.EmailEnabled != null)
                    query = query.Where(x => x.EmailEnabled == filter.EmailEnabled.Value);

                if (filter.EmailAlternateEnabled != null)
                    query = query.Where(x => x.EmailAlternateEnabled == filter.EmailAlternateEnabled.Value);
            }

            if (filter.EmailVerified != null)
            {
                if (filter.EmailVerified.Value)
                    query = query.Where(x => x.User.Email == x.User.EmailVerified);
                else
                    query = query.Where(x => x.User.Email != x.User.EmailVerified);
            }

            if (filter.AccountStatuses.IsNotEmpty())
                query = query.Where(x => x.MembershipStatusItemIdentifier.HasValue && filter.AccountStatuses.Contains(x.MembershipStatusItemIdentifier.Value));

            if (filter.Cities.IsNotEmpty() || filter.Provinces.IsNotEmpty() || filter.Country.HasValue())
            {
                var isBilling = true;
                var isHome = true;
                var isShipping = true;
                var isWork = true;

                if (filter.AddressTypes.IsNotEmpty())
                {
                    isBilling = filter.AddressTypes.Contains("Billing");
                    isHome = filter.AddressTypes.Contains("Home");
                    isShipping = filter.AddressTypes.Contains("Shipping");
                    isWork = filter.AddressTypes.Contains("Work");

                    if (!isBilling && !isHome && !isShipping && !isWork)
                        isBilling = isHome = isShipping = isWork = true;
                }

                if (filter.Cities.IsNotEmpty())
                    query = query.Where(x =>
                        (isBilling && filter.Cities.Contains(x.BillingAddress.City)) ||
                        (isHome && filter.Cities.Contains(x.HomeAddress.City)) ||
                        (isShipping && filter.Cities.Contains(x.ShippingAddress.City)) ||
                        (isWork && filter.Cities.Contains(x.WorkAddress.City))
                    );

                if (filter.Provinces.IsNotEmpty())
                    query = query.Where(x =>
                        (isBilling && filter.Provinces.Contains(x.BillingAddress.Province)) ||
                        (isHome && filter.Provinces.Contains(x.HomeAddress.Province)) ||
                        (isShipping && filter.Provinces.Contains(x.ShippingAddress.Province)) ||
                        (isWork && filter.Provinces.Contains(x.WorkAddress.Province))
                    );

                if (filter.Country.HasValue())
                    query = query.Where(x =>
                        (isBilling && x.BillingAddress.Country == filter.Country) ||
                        (isHome && x.HomeAddress.Country == filter.Country) ||
                        (isShipping && x.ShippingAddress.Country == filter.Country) ||
                        (isWork && x.WorkAddress.Country == filter.Country)
                    );

                if (filter.Street1.IsNotEmpty())
                    query = query.Where(x =>
                        (isBilling && filter.Street1.Contains(x.BillingAddress.Street1)) ||
                        (isHome && filter.Street1.Contains(x.HomeAddress.Street1)) ||
                        (isShipping && filter.Street1.Contains(x.ShippingAddress.Street1)) ||
                        (isWork && filter.Street1.Contains(x.WorkAddress.Street1))
                    );
            }

            if (filter.JobTitle.IsNotEmpty())
                query = query.Where(x => x.JobTitle.Contains(filter.JobTitle));

            if (filter.MustHaveComments && db != null)
                query = query.Where(x => x.User.Email != null && db.QComments.Select(y => y.AuthorUserIdentifier).Contains(x.UserIdentifier));

            if (filter.UpstreamUserIdentifiers.IsNotEmpty())
                query = query.Where(x => x.User.UpstreamConnections.Any(y => filter.UpstreamUserIdentifiers.Contains(y.FromUserIdentifier)));

            if (filter.DownstreamUserIdentifiers.IsNotEmpty())
                query = query.Where(x => x.User.DownstreamConnections.Any(y => filter.DownstreamUserIdentifiers.Contains(y.ToUserIdentifier)));

            if (filter.NameOrAccountNumber.IsNotEmpty())
            {
                query = query.Where(x =>
                    (x.User.FirstName + " " + x.User.LastName).Contains(filter.NameOrAccountNumber)
                    || (x.User.LastName + ", " + x.User.FirstName).Contains(filter.NameOrAccountNumber)
                    || x.User.FullName.Contains(filter.NameOrAccountNumber)
                    || x.PersonCode.Contains(filter.NameOrAccountNumber)
                );
            }

            if (filter.UtcCreatedSince.HasValue)
                query = query.Where(x => x.Created >= filter.UtcCreatedSince.Value);

            if (filter.UtcCreatedBefore.HasValue)
                query = query.Where(x => x.Created < filter.UtcCreatedBefore.Value);

            if (filter.ModifiedSince.HasValue)
                query = query.Where(x => x.Modified >= filter.ModifiedSince.Value);

            if (filter.ModifiedBefore.HasValue)
                query = query.Where(x => x.Modified < filter.ModifiedBefore.Value);

            if (filter.GroupIdentifier.HasValue)
            {
                if (filter.GroupMembershipDate.HasValue)
                    query = query.Where(x => x.User.Memberships.Any(y => y.GroupIdentifier == filter.GroupIdentifier && y.Assigned >= filter.GroupMembershipDate));
                else
                    query = query.Where(x => x.User.Memberships.Any(y => y.GroupIdentifier == filter.GroupIdentifier));
            }

            if (filter.Groups.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Any(y => filter.Groups.Any(z => z == y.GroupIdentifier)));

            if (filter.ExcludeGroupIdentifier.HasValue)
                query = query.Where(x => !x.User.Memberships.Any(y => y.GroupIdentifier == filter.ExcludeGroupIdentifier));

            if (filter.SessionCount.HasValue)
                query = query.Where(x => db.TUserSessions.Count(y => y.UserIdentifier == x.UserIdentifier && y.SessionIsAuthenticated) == filter.SessionCount);

            if (filter.LastAuthenticatedSince.HasValue)
                query = query.Where(x => x.LastAuthenticated >= filter.LastAuthenticatedSince);

            if (filter.LastAuthenticatedBefore.HasValue)
                query = query.Where(x => x.LastAuthenticated < filter.LastAuthenticatedBefore);

            if (filter.Phone.IsNotEmpty())
            {
                query = query.Where(x =>
                    x.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(filter.Phone)
                    || x.PhoneHome.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(filter.Phone)
                    || x.User.PhoneMobile.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(filter.Phone)
                    || x.PhoneOther.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(filter.Phone)
                    || x.PhoneWork.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(filter.Phone)
                );
            }

            if (filter.Region.IsNotEmpty())
                query = query.Where(x => x.Region.Contains(filter.Region));

            if (filter.Gender.IsNotEmpty())
                query = query.Where(x => x.Gender == filter.Gender);

            if (filter.CommentKeyword.HasValue())
            {
                var topics = db.QComments.Where(comment => comment.TopicUserIdentifier.HasValue && comment.CommentText.Contains(filter.CommentKeyword)).Select(x => x.TopicUserIdentifier.Value);
                query = query.Where(user => topics.Any(topic => user.UserIdentifier == topic));
            }

            if (filter.CloakedUsers == InclusionType.Exclude)
                query = query.Where(x => x.User.AccountCloaked == null);
            else if (filter.CloakedUsers == InclusionType.Only)
                query = query.Where(x => x.User.AccountCloaked != null);

            if (filter.CandidateIsActivelySeeking.HasValue)
                query = query.Where(x => x.CandidateIsActivelySeeking == filter.CandidateIsActivelySeeking);

            if (filter.CandidateOccupationKey.IsNotEmpty())
            {
                query = query.Where(x =>
                    x.User.PersonFields.Any(y =>
                        y.OrganizationIdentifier == x.OrganizationIdentifier
                        && y.FieldName == "Industry Interest Area"
                        && y.FieldValue == filter.CandidateOccupationKey
                    )
                );
            }

            if (filter.DayLastActive.HasValue)
                query = query.Where(x => x.Modified.Year == filter.DayLastActive.Value.Year
                    && x.Modified.Month == filter.DayLastActive.Value.Month
                    && x.Modified.Day == filter.DayLastActive.Value.Day);

            if (filter.IsCandidate.HasValue)
            {
                if (filter.IsCandidate.Value)
                    query = query.Where(
                        x => x.User.Memberships.Any(
                                m => m.Group.OrganizationIdentifier == x.OrganizationIdentifier
                                  && m.Group.GroupCategory != null && m.Group.GroupCategory == "Candidate"));
                else
                    query = query.Where(
                        x => !x.User.Memberships.Any(
                                m => m.Group.OrganizationIdentifier == x.OrganizationIdentifier
                                  && m.Group.GroupCategory != null && m.Group.GroupCategory == "Candidate"));
            }

            if (filter.GroupDepartmentIdentifiers.IsNotEmpty())
            {
                if (filter.GroupDepartmentFunctions.IsEmpty())
                    query = query.Where(x =>
                         db.Memberships.Any(y =>
                             filter.GroupDepartmentIdentifiers.Contains(y.GroupIdentifier)
                            && y.UserIdentifier == x.UserIdentifier
                             )
                         );
                else
                    query = query.Where(x =>
                         db.Memberships.Any(y =>
                             filter.GroupDepartmentIdentifiers.Contains(y.GroupIdentifier)
                          && filter.GroupDepartmentFunctions.Contains(y.MembershipType)
                          && y.UserIdentifier == x.UserIdentifier
                             )
                         );
            }

            if (filter.OccupationStandardIdentifier.HasValue)
            {
                var children = db.Standards
                    .Where(x => x.StandardIdentifier == filter.OccupationStandardIdentifier && x.ParentStandardIdentifier != null)
                    .Select(x => x.ParentStandardIdentifier.Value)
                    .ToArray();

                var occupation = db.Standards.FirstOrDefault(x => x.StandardIdentifier == filter.OccupationStandardIdentifier)?.ContentTitle;
                query = query.Where(x => x.JobTitle == occupation || children.Any(y => y == x.OccupationStandardIdentifier));
            }

            if (filter.ValidAchievementIdentifier.HasValue)
                query = query.Where(x => db.QCredentials.Any(y => y.UserIdentifier == x.UserIdentifier && y.AchievementIdentifier == filter.ValidAchievementIdentifier && y.CredentialStatus == "Valid"));

            if (filter.OccupationInterest.HasValue)
                query = query.Where(x => x.OccupationStandardIdentifier == filter.OccupationInterest.Value);

            if (filter.GroupRoleIdentifiers.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Any(y => filter.GroupRoleIdentifiers.Any(z => z == y.GroupIdentifier)));

            if (filter.MembershipReasonExpirySince.HasValue)
            {
                if (filter.EmployerGroups.IsNotEmpty())
                    query = query.Where(x => !db.QMembershipReasons.Where(r => r.Membership.UserIdentifier == x.UserIdentifier).Any(r => filter.EmployerGroups.Contains(r.Membership.GroupIdentifier))
                                          || db.QMembershipReasons.Where(r => r.Membership.UserIdentifier == x.UserIdentifier).Any(r => filter.EmployerGroups.Contains(r.Membership.GroupIdentifier) && filter.MembershipReasonExpirySince.Value < r.ReasonExpiry));
                else
                    query = query.Where(x => false);
            }

            if (filter.IsAdministrator.HasValue)
                query = query.Where(x => x.IsAdministrator == filter.IsAdministrator.Value);

            query = ApplyIssueFilter(filter.MustHaveCompletedCases, filter.IssueStatusEffectiveSince, filter.IssueType, query, db);

            return query;
        }

        private static IQueryable<Person> ApplyIssueFilter(
            bool mustHaveCompletedCases,
            DateTimeOffset? issueStatusEffectiveSince,
            string issueType,
            IQueryable<Person> query,
            InternalDbContext db
            )
        {
            if (issueType.HasValue())
                query = query.Where(x => db.QIssues.Any(y => y.TopicUserIdentifier == x.UserIdentifier && y.IssueType == issueType && y.IssueStatusCategory == "Open"));

            if (!mustHaveCompletedCases && issueStatusEffectiveSince == null)
                return query;

            var issueQuery = db.QIssues.AsQueryable();

            if (mustHaveCompletedCases)
                issueQuery = issueQuery.Where(i => db.TCaseStatuses.Where(s => s.StatusName == "Completed" && s.StatusIdentifier == i.IssueStatusIdentifier).Any());

            if (issueStatusEffectiveSince.HasValue)
                issueQuery = issueQuery.Where(i => i.IssueStatusEffective >= issueStatusEffectiveSince);

            query = query.Where(p => issueQuery.Where(i => i.TopicUserIdentifier == p.UserIdentifier).Any());

            return query;
        }

        #endregion
    }
}