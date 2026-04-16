using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Cases.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class CaseSearch : ICaseSearch
    {
        private static readonly object _syncRoot = new object();

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Issue

        public int CountIssues(QIssueFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public List<VIssue> GetIssues(IEnumerable<Guid> ids)
        {
            using (var db = CreateContext())
            {
                return db.VIssues
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.IssueIdentifier))
                    .ToList();
            }
        }

        public List<ExportCase> GetExportCases(QIssueFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderByDescending(x => x.IssueOpened)
                    : query.OrderBy(filter.OrderBy);

                return query
                    .ApplyPaging(filter)
                    .Select(x => new
                    {
                        Issue = x,
                        TopicUserDepartments = db.QMemberships
                            .Where(
                                m => m.UserIdentifier == x.TopicUserIdentifier
                                  && m.MembershipFunction == "Department"
                                  && m.Group.GroupType == "Department"
                                  && m.Group.OrganizationIdentifier == filter.OrganizationIdentifier)
                            .Select(m => m.Group.GroupName)
                    })
                    .AsEnumerable()
                    .Select(x => new ExportCase
                    {
                        IssueClosedBy = x.Issue.IssueClosedBy,
                        IssueIdentifier = x.Issue.IssueIdentifier,
                        IssueOpenedBy = x.Issue.IssueOpenedBy,
                        TopicUserIdentifier = x.Issue.TopicUserIdentifier,
                        TopicUserDepartment = string.Join(", ", x.TopicUserDepartments),
                        AdministratorUserName = x.Issue.AdministratorUserName,
                        IssueDescription = x.Issue.IssueDescription,
                        IssueEmployerGroupName = x.Issue.IssueEmployerGroupName,
                        IssueEmployerGroupParentGroupName = x.Issue.IssueEmployerGroupParentGroupName,
                        IssueSource = x.Issue.IssueSource,
                        IssueStatusCategory = x.Issue.IssueStatusCategory,
                        IssueStatusName = x.Issue.IssueStatusName,
                        IssueTitle = x.Issue.IssueTitle,
                        IssueType = x.Issue.IssueType,
                        LawyerUserName = x.Issue.LawyerUserName,
                        OwnerUserEmail = x.Issue.OwnerUserEmail,
                        OwnerUserName = x.Issue.OwnerUserName,
                        TopicAccountStatus = x.Issue.TopicAccountStatus,
                        TopicEmployerGroupName = x.Issue.TopicEmployerGroupName,
                        TopicUserName = x.Issue.TopicUserName,
                        TopicUserEmail = x.Issue.TopicUserEmail,
                        IssueNumber = x.Issue.IssueNumber,
                        IssueStatusSequence = x.Issue.IssueStatusSequence,
                        IssueClosed = x.Issue.IssueClosed,
                        IssueOpened = x.Issue.IssueOpened,
                        IssueReported = x.Issue.IssueReported,
                        LastChangeTime = x.Issue.LastChangeTime,
                        LastChangeType = x.Issue.LastChangeType,
                        LastChangeUser = x.Issue.LastChangeUser,
                        LastChangeUserName = x.Issue.LastChangeUserName,
                    })
                    .ToList();
            }
        }

        public List<VIssue> GetIssues(QIssueFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                if (string.IsNullOrEmpty(filter.OrderBy))
                    query = query.OrderByDescending(x => x.IssueOpened);
                else
                    query = query.OrderBy(filter.OrderBy);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<VIssue> GetIssuesWithCommentMentions(Guid organization, Guid user)
        {
            using (var db = CreateContext())
            {
                var myIssues = db.VIssues
                    .Where(x => x.OrganizationIdentifier == organization &&
                    (x.OwnerUserIdentifier == user || x.TopicUserIdentifier == user))
                    .ToList();

                var myIssuesPublicComments = db.VComments
                    .Where(
                        x => x.CommentAssignedToUserIdentifier == user
                        && x.IssueIdentifier != null
                        && x.OrganizationIdentifier == organization
                        && x.CommentIsPrivate == false
                    )
                    .Select(x => x.IssueIdentifier)
                    .Join(db.VIssues,
                        a => a.Value,
                        b => b.IssueIdentifier,
                        (a, b) => b
                    )
                .ToList();

                return myIssues.Concat(myIssuesPublicComments).Distinct().ToList();
            }
        }

        public List<Guid> GetIssuesTopicUserIdentifiers(QIssueFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Where(x => x.TopicUserIdentifier.HasValue)
                    .Select(x => x.TopicUserIdentifier.Value)
                    .Distinct().ToList();
            }
        }

        private IQueryable<VIssue> CreateQuery(QIssueFilter filter, InternalDbContext db)
        {
            var query = db.VIssues
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.IssueNumber.HasValue)
                query = query.Where(x => x.IssueNumber == filter.IssueNumber);

            if (filter.AdministratorUserIdentifier.HasValue)
                query = query.Where(x => x.AdministratorUserIdentifier == filter.AdministratorUserIdentifier);

            if (filter.OwnerUserIdentifier.HasValue)
                query = query.Where(x => x.OwnerUserIdentifier == filter.OwnerUserIdentifier);

            if (filter.ResponseSessionIdentifier.HasValue)
                query = query.Where(x => x.ResponseSessionIdentifier == filter.ResponseSessionIdentifier);

            if (filter.TopicUserIdentifier.HasValue)
                query = query.Where(x => x.TopicUserIdentifier == filter.TopicUserIdentifier);

            if (filter.TopicUserIdentifiers != null && filter.TopicUserIdentifiers.Length > 0)
                query = query.Where(x => filter.TopicUserIdentifiers.Any(user => user == x.TopicUserIdentifier));

            if (filter.TopicUserConnectedFromUserIdentifier.HasValue)
            {
                query = query.Where(x =>
                       x.AdministratorUserIdentifier == filter.TopicUserConnectedFromUserIdentifier
                    || x.OwnerUserIdentifier == filter.TopicUserConnectedFromUserIdentifier
                    || x.TopicUserIdentifier == filter.TopicUserConnectedFromUserIdentifier
                    || db.QUserConnections.Any(y =>
                        y.FromUserIdentifier == filter.TopicUserConnectedFromUserIdentifier
                        && y.ToUserIdentifier == x.TopicUserIdentifier
                    )
                );
            }

            if (filter.IssueType.IsNotEmpty())
                query = query.Where(x => x.IssueType == filter.IssueType);

            if (filter.IssueStatusIdentifier.HasValue)
                query = query.Where(x => x.IssueStatusIdentifier == filter.IssueStatusIdentifier);

            if (filter.IssueStatusCategory.IsNotEmpty())
                query = query.Where(x => x.IssueStatusCategory == filter.IssueStatusCategory);

            if (filter.IssueTitle.IsNotEmpty())
                query = query.Where(x => x.IssueTitle.Contains(filter.IssueTitle));

            if (filter.IssueDescription.IsNotEmpty())
                query = query.Where(x => x.IssueDescription.Contains(filter.IssueDescription));

            var commentQuery = db.VComments.AsQueryable();
            var hasCommentFilter = false;

            if (filter.IssueCommentAssigneeIdentifier.HasValue)
            {
                commentQuery = commentQuery.Where(comment => comment.CommentAssignedToUserIdentifier == filter.IssueCommentAssigneeIdentifier);
                hasCommentFilter = true;
            }

            if (filter.IssueCommentCategory.IsNotEmpty())
            {
                commentQuery = commentQuery.Where(comment => comment.CommentCategory == filter.IssueCommentCategory);
                hasCommentFilter = true;
            }

            if (filter.IssueCommentSubCategory.IsNotEmpty())
            {
                commentQuery = commentQuery.Where(comment => comment.CommentSubCategory == filter.IssueCommentSubCategory);
                hasCommentFilter = true;
            }

            if (filter.IssueCommentFlag.IsNotEmpty())
            {
                commentQuery = commentQuery.Where(comment => comment.CommentFlag.Equals(filter.IssueCommentFlag));
                hasCommentFilter = true;
            }

            if (filter.IssueCommentTag.IsNotEmpty())
            {
                commentQuery = commentQuery.Where(comment => comment.CommentTag == filter.IssueCommentTag);
                hasCommentFilter = true;
            }

            if (filter.IssueCommentText.IsNotEmpty())
            {
                commentQuery = commentQuery.Where(comment => comment.CommentText.Contains(filter.IssueCommentText));
                hasCommentFilter = true;
            }

            if (hasCommentFilter)
            {
                var issues = commentQuery.Select(comment => comment.IssueIdentifier);
                query = query.Where(x => issues.Any(issue => issue == x.IssueIdentifier));
            }

            if (filter.DateReportedSince.HasValue)
                query = query.Where(x => x.IssueReported >= filter.DateReportedSince.Value);

            if (filter.DateReportedBefore.HasValue)
                query = query.Where(x => x.IssueReported < filter.DateReportedBefore.Value);

            if (filter.DateCaseStatusEffectiveSince.HasValue)
                query = query.Where(x => x.IssueStatusEffective >= filter.DateCaseStatusEffectiveSince.Value);

            if (filter.DateCaseStatusEffectiveBefore.HasValue)
                query = query.Where(x => x.IssueStatusEffective < filter.DateCaseStatusEffectiveBefore.Value);

            if (filter.DateOpenedSince.HasValue)
                query = query.Where(x => x.IssueOpened >= filter.DateOpenedSince.Value);

            if (filter.DateOpenedBefore.HasValue)
                query = query.Where(x => x.IssueOpened < filter.DateOpenedBefore.Value);

            if (filter.DateClosedSince.HasValue)
                query = query.Where(x => x.IssueClosed >= filter.DateClosedSince.Value);

            if (filter.DateClosedBefore.HasValue)
                query = query.Where(x => x.IssueClosed < filter.DateClosedBefore.Value);

            if (filter.AssigneeName.IsNotEmpty())
                query = query.Where(x => x.TopicUserName.Contains(filter.AssigneeName));

            if (filter.LawyerIdentifier.HasValue)
                query = query.Where(x => x.LawyerUserIdentifier == filter.LawyerIdentifier);

            if (filter.MembershipStatuses.IsNotEmpty())
                query = query.Where(x => x.TopicAccountStatus != null && filter.MembershipStatuses.Contains(x.TopicAccountStatus));

            if (filter.AssigneeEmployer.IsNotEmpty())
                query = query.Where(x => filter.AssigneeEmployer.Contains(x.IssueEmployerGroupIdentifier.Value));

            if (filter.PersonCode.IsNotEmpty())
            {
                var userIdentifier = db.Persons
                    .Select(x => new
                    {
                        x.PersonCode,
                        x.OrganizationIdentifier,
                        x.UserIdentifier
                    }).Where(x => x.PersonCode.Contains(filter.PersonCode)
                        && x.OrganizationIdentifier == filter.OrganizationIdentifier).Select(x => x.UserIdentifier);
                query = query.Where(x => userIdentifier.Any(user => user == x.TopicUserIdentifier));
            }

            if (filter.AssigneeOrganization.IsNotEmpty())
                query = query.Where(x => x.TopicGroupNames.Contains(filter.AssigneeOrganization));

            if (filter.TopicDepartmentIdentifier.HasValue)
            {
                query = query.Where(
                    x => db.QMemberships.Any(
                        m => m.UserIdentifier == x.TopicUserIdentifier
                          && m.GroupIdentifier == filter.TopicDepartmentIdentifier.Value
                          && m.MembershipFunction == "Department"));
            }

            query = ApplyFileRequirementFilter(filter, db, query);
            query = ApplyAttachmentFilter(filter, db, query);

            return query;
        }

        private static IQueryable<VIssue> ApplyFileRequirementFilter(QIssueFilter filter, InternalDbContext db, IQueryable<VIssue> query)
        {
            if (!filter.OnlyRequestedFiles)
                return query;

            var requirementQuery = db.QIssueFileRequirements.AsQueryable();

            query = query.Where(x => requirementQuery.Where(y => y.IssueIdentifier == x.IssueIdentifier).Any());

            return query;
        }

        private static IQueryable<VIssue> ApplyAttachmentFilter(QIssueFilter filter, InternalDbContext db, IQueryable<VIssue> query)
        {
            if (filter.OnlyRequestedFiles)
                return query;

            var fileQuery = db.TFiles.AsQueryable();
            var hasFileFilter = false;

            if (!string.IsNullOrEmpty(filter.AttachmentFileStatus))
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileStatus == filter.AttachmentFileStatus);
            }

            if (!string.IsNullOrEmpty(filter.AttachmentFileCategory))
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileCategory == filter.AttachmentFileCategory);
            }

            if (!string.IsNullOrEmpty(filter.AttachmentDocumentName))
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.DocumentName.Contains(filter.AttachmentDocumentName));
            }

            if (filter.AttachmentHasClaims.HasValue)
            {
                hasFileFilter = true;
                fileQuery = filter.AttachmentHasClaims.Value
                    ? fileQuery.Where(x => x.FileClaims.Any())
                    : fileQuery.Where(x => !x.FileClaims.Any());
            }

            if (filter.AttachmentFileExpirySince.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileExpiry >= filter.AttachmentFileExpirySince.Value);
            }

            if (filter.AttachmentFileExpiryBefore.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileExpiry < filter.AttachmentFileExpiryBefore.Value);
            }

            if (filter.AttachmentFileReceivedSince.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileReceived >= filter.AttachmentFileReceivedSince.Value);
            }

            if (filter.AttachmentFileReceivedBefore.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileReceived < filter.AttachmentFileReceivedBefore.Value);
            }

            if (filter.AttachmentFileAlternatedSince.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileAlternated >= filter.AttachmentFileAlternatedSince.Value);
            }

            if (filter.AttachmentFileAlternatedBefore.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileAlternated < filter.AttachmentFileAlternatedBefore.Value);
            }

            if (filter.AttachmentApprovedSince.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.ApprovedTime >= filter.AttachmentApprovedSince.Value);
            }

            if (filter.AttachmentApprovedBefore.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.ApprovedTime < filter.AttachmentApprovedBefore.Value);
            }

            if (filter.AttachmentUploadedSince.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileUploaded >= filter.AttachmentUploadedSince.Value);
            }

            if (filter.AttachmentUploadedBefore.HasValue)
            {
                hasFileFilter = true;
                fileQuery = fileQuery.Where(x => x.FileUploaded < filter.AttachmentUploadedBefore.Value);
            }

            if (!hasFileFilter)
                return query;

            var attachmentQuery = db.QIssueAttachments
                .Join(fileQuery,
                    attach => attach.FileIdentifier,
                    file => file.FileIdentifier,
                    (attach, file) => attach
                );

            query = query.Where(x => attachmentQuery.Where(y => y.IssueIdentifier == x.IssueIdentifier).Any());

            return query;
        }

        public VIssue GetIssue(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.VIssues
                    .AsNoTracking()
                    .Include(x => x.Lawyer)
                    .Include(x => x.Topic)
                    .Include(x => x.Administrator)
                    .FirstOrDefault(x => x.IssueIdentifier == id);
            }
        }

        public int GetNextIssueNumber(Guid organization)
        {
            lock (_syncRoot)
            {
                using (var db = CreateContext())
                {
                    var maxNum = db.QIssues
                        .Where(x => x.OrganizationIdentifier == organization)
                        .Max(x => (int?)x.IssueNumber);

                    return 1 + (maxNum ?? 0);
                }
            }
        }

        #endregion

        #region Users

        public int CountUsers(QIssueUserFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<VIssueUser> GetUsers(QIssueUserFilter filter, params Expression<Func<VIssueUser, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .Include(x => x.VIssue)
                    .ApplyIncludes(includes);

                if (string.IsNullOrEmpty(filter.OrderBy))
                    query = query.OrderByDescending(x => x.UserFullName);
                else
                    query = query.OrderBy(x => filter.OrderBy);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<Guid> GetUsersHavingRole(Guid organization, string role)
        {
            using (var db = CreateContext())
            {
                return db.QIssueUsers
                    .Where(u => db.QIssues.Any(i => i.OrganizationIdentifier == organization
                            && u.IssueIdentifier == i.IssueIdentifier
                            && u.IssueRole == role
                            && db.Persons.Any(p => p.User.UserIdentifier == u.UserIdentifier && p.Organization.OrganizationIdentifier == organization))
                    ).Select(u => u.UserIdentifier).Distinct().ToList();
            }
        }

        private IQueryable<VIssueUser> CreateQuery(QIssueUserFilter filter, InternalDbContext db)
        {
            var query = db.VIssueUsers
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.IssueOrganizationIdentifier.HasValue)
                query = query.Where(x => x.VIssue.OrganizationIdentifier == filter.IssueOrganizationIdentifier.Value);

            if (filter.Issuedentifier.HasValue)
                query = query.Where(x => x.VIssue.IssueIdentifier == filter.Issuedentifier.Value);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier.Value);

            if (filter.IssueRole.HasValue())
                query = query.Where(x => x.IssueRole == filter.IssueRole);

            return query;
        }

        #endregion

        #region Comments

        public int CountComments(QIssueCommentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public VComment GetComment(Guid comment)
        {
            using (var db = CreateContext())
            {
                return db.VComments
                    .AsNoTracking()
                    .FirstOrDefault(x => x.CommentIdentifier == comment);
            }
        }

        public List<VComment> GetComments(QIssueCommentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<VComment> CreateQuery(QIssueCommentFilter filter, InternalDbContext db)
        {
            var query = db.VComments
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ContainerType.HasValue())
                query = query.Where(x => x.ContainerType == filter.ContainerType);

            if (filter.IssueIdentifier != null)
                query = query.Where(x => x.IssueIdentifier == filter.IssueIdentifier);

            if (filter.IssueIdentifiers != null)
                query = query.Where(x => filter.IssueIdentifiers.Any(i => i == x.IssueIdentifier));

            if (filter.AuthorIdentifier != null)
                query = query.Where(x => x.AuthorUserIdentifier == filter.AuthorIdentifier);

            if (filter.DatePostedSince.HasValue)
                query = query.Where(x => x.CommentPosted >= filter.DatePostedSince.Value);

            if (filter.DatePostedBefore.HasValue)
                query = query.Where(x => x.CommentPosted < filter.DatePostedBefore.Value);

            return query;
        }

        #endregion

        #region Attachments

        public int CountAttachments(QIssueAttachmentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public List<VIssueAttachment> GetAttachments(QIssueAttachmentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<VIssueAttachment> CreateQuery(QIssueAttachmentFilter filter, InternalDbContext db)
        {
            var query = db.VIssueAttachments
                .AsNoTracking()
                .AsQueryable();

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.IssueIdentifier.HasValue && filter.IssueIdentifier != Guid.Empty)
                query = query.Where(x => x.IssueIdentifier == filter.IssueIdentifier);

            if (filter.InputterUserIdentifier.HasValue)
                query = query.Where(x => x.InputterUserIdentifier == filter.InputterUserIdentifier);

            if (filter.TopicUserIdentifier.HasValue)
                query = query.Where(x => x.TopicUserIdentifier == filter.TopicUserIdentifier);

            if (filter.TopicUserIdentifiers != null && filter.TopicUserIdentifiers.Length > 0)
                query = query.Where(x => filter.TopicUserIdentifiers.Contains(x.TopicUserIdentifier));

            return query;
        }

        public VIssueAttachment GetAttachment(Guid issue, string fileName)
        {
            using (var db = CreateContext())
            {
                return db.VIssueAttachments.AsNoTracking()
                    .Where(x => x.IssueIdentifier == issue && x.FileName == fileName)
                    .FirstOrDefault();
            }
        }

        public QIssueFileRequirement GetFileRequirement(Guid issue, string requestedFileCategory)
        {
            using (var db = CreateContext())
            {
                return db.QIssueFileRequirements
                    .AsNoTracking()
                    .Where(x => x.IssueIdentifier == issue && x.RequestedFileCategory == requestedFileCategory)
                    .FirstOrDefault();
            }
        }

        public List<VIssueFileRequirement> GetFileRequirements(Guid issue)
        {
            using (var db = CreateContext())
            {
                return db.VIssueFileRequirements
                    .AsNoTracking()
                    .Where(x => x.IssueIdentifier == issue)
                    .OrderBy(x => x.RequestedFileCategory)
                    .ToList();
            }
        }

        #endregion

        #region Statuses

        public TCaseStatus GetStatus(Guid status)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.StatusIdentifier == status)
                    .FirstOrDefault();
            }
        }

        public List<TCaseStatus> GetStatuses(Guid organization)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.OrganizationIdentifier == organization)
                    .OrderBy(x => x.StatusSequence).ThenBy(x => x.StatusName)
                    .ToList();
            }
        }

        public List<TCaseStatus> GetStatuses(Guid organization, string caseType)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.OrganizationIdentifier == organization && x.CaseType == caseType)
                    .OrderBy(x => x.StatusSequence).ThenBy(x => x.StatusName)
                    .ToList();
            }
        }

        public List<TCaseStatus> GetStatuses(Guid organization, string caseType, string statusCategory)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.OrganizationIdentifier == organization && x.CaseType == caseType && x.StatusCategory == statusCategory)
                    .OrderBy(x => x.StatusSequence).ThenBy(x => x.StatusName)
                    .ToList();
            }
        }

        public Guid? GetStatusId(Guid organization, string caseType, string statusName)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.OrganizationIdentifier == organization && x.CaseType == caseType && x.StatusName == statusName)
                    .OrderBy(x => x.StatusSequence).ThenBy(x => x.StatusName)
                    .Select(x => (Guid?)x.StatusIdentifier)
                    .FirstOrDefault();
            }
        }

        public List<string> GetStatusNamesByCategory(Guid organization, string statusCategory)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.OrganizationIdentifier == organization && x.StatusCategory == statusCategory)
                    .OrderBy(x => x.StatusSequence).ThenBy(x => x.StatusName)
                    .Select(x => x.StatusName)
                    .Distinct()
                    .ToList();
            }
        }

        public List<KeyValuePair<Guid, string>> GetStatusIdentifiersAndNamesByCategory(Guid organization, string statusCategory)
        {
            using (var db = CreateContext())
            {
                return db.TCaseStatuses
                    .Where(x => x.OrganizationIdentifier == organization && x.StatusCategory == statusCategory)
                    .OrderBy(x => x.StatusSequence).ThenBy(x => x.StatusName)
                    .Select(x => new KeyValuePair<Guid, string>(x.StatusIdentifier, x.StatusName))
                    .Distinct()
                    .ToList();
            }
        }

        #endregion
    }
}