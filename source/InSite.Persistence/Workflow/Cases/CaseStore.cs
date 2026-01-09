using System;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Domain.Issues;

using Shift.Common;

namespace InSite.Persistence
{
    public class CaseStore : ICaseStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void Delete(Guid issue)
        {
            using (var db = CreateContext())
            {
                var sql = @"
delete from issues.QIssue where IssueIdentifier = @Aggregate;
delete from issues.QIssueGroup where IssueIdentifier = @Aggregate;
delete from issues.QIssueAttachment where IssueIdentifier = @Aggregate;
delete from issues.QIssueFileRequirement where IssueIdentifier = @Aggregate;
delete from issues.QIssueUser where IssueIdentifier = @Aggregate;
delete from assets.QComment where IssueIdentifier = @Aggregate;
";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("Aggregate", issue));
            }
        }

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                var sql = @"
truncate table issues.QIssue;
truncate table issues.QIssueGroup;
truncate table issues.QIssueAttachment;
truncate table issues.QIssueFileRequirement;
truncate table issues.QIssueUser;
delete from assets.QComment where IssueIdentifier is not null;
";
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public void Insert(CaseOpened2 e)
        {
            using (var db = CreateContext())
            {
                var query = new QIssue
                {
                    IssueIdentifier = e.AggregateIdentifier,
                    IssueDescription = e.Description,
                    IssueOpened = e.Opened,
                    IssueNumber = e.Number,
                    IssueSource = e.Source,
                    IssueReported = e.Reported,
                    IssueStatusCategory = "Open",
                    IssueTitle = e.Title,
                    IssueType = e.Type,
                    OrganizationIdentifier = e.Organization
                };
                db.QIssues.Add(query);

                SetLastChange(query, e);

                db.SaveChanges();
            }
        }

        private void UpdateCursor(IChange e, Action<InternalDbContext, CaseState, QIssue> change)
        {
            using (var db = CreateContext())
            {
                var query = db.QIssues.Single(x => x.IssueIdentifier == e.AggregateIdentifier);

                var state = (CaseState)e.AggregateState;
                change?.Invoke(db, state, query);

                SetLastChange(query, e);

                db.SaveChanges();
            }
        }

        private void SetLastChange(QIssue entity, IChange change)
        {
            entity.LastChangeTime = change.ChangeTime;
            entity.LastChangeType = change.GetType().Name;
            entity.LastChangeUser = change.OriginUser;
        }

        public void Update(CaseAttachmentAdded e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var attachment = db.QIssueAttachments.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.FileName == e.FileName);
                if (attachment != null)
                    return;

                attachment = new QIssueAttachment
                {
                    AttachmentIdentifier = UniqueIdentifier.Create(),
                    FileName = e.FileName,
                    FileIdentifier = e.FileIdentifier,
                    FileType = e.FileType,
                    AttachmentPosted = e.Posted,
                    IssueIdentifier = e.AggregateIdentifier,
                    PosterIdentifier = e.Poster,
                    OrganizationIdentifier = e.OriginOrganization
                };
                db.QIssueAttachments.Add(attachment);

                query.AttachmentCount++;
            });
        }

        public void Update(CaseAttachmentFileChanged e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var attachment = db.QIssueAttachments.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.FileName == e.FileName);
                if (attachment == null)
                    return;

                attachment.FileIdentifier = e.FileIdentifier;
            });
        }

        public void Update(CaseAttachmentFileRenamed e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var attachment = db.QIssueAttachments.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.FileName == e.OldFileName);
                if (attachment == null)
                    return;

                var newAttachment = attachment.Clone();
                newAttachment.FileName = e.NewFileName;

                db.QIssueAttachments.Remove(attachment);
                db.QIssueAttachments.Add(newAttachment);
            });
        }

        public void Update(CaseAttachmentDeleted e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var attachment = db.QIssueAttachments.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.FileName == e.FileName);
                if (attachment != null)
                {
                    db.QIssueAttachments.Remove(attachment);

                    query.AttachmentCount--;
                }
            });
        }

        public void Update(CaseFileRequirementAdded e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var request = db.QIssueFileRequirements.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.RequestedFileCategory == e.RequestedFileCategory);
                if (request != null)
                    return;

                request = new QIssueFileRequirement
                {
                    IssueIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.OriginOrganization,
                    RequestedTime = e.ChangeTime,
                    RequestedUserIdentifier = e.OriginUser,
                    RequestedFrom = e.RequestedFrom ?? "Candidate",
                    RequestedFileCategory = e.RequestedFileCategory,
                    RequestedFileSubcategory = e.RequestedFileSubcategory,
                    RequestedFileDescription = e.RequestedFileDescription
                };

                db.QIssueFileRequirements.Add(request);
            });
        }

        public void Update(CaseFileRequirementModified e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var request = db.QIssueFileRequirements.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.RequestedFileCategory == e.RequestedFileCategory);
                if (request == null)
                {
                    request = new QIssueFileRequirement
                    {
                        IssueIdentifier = e.AggregateIdentifier,
                        OrganizationIdentifier = e.OriginOrganization,
                        RequestedTime = e.ChangeTime,
                        RequestedUserIdentifier = e.OriginUser,
                        RequestedFileCategory = e.RequestedFileCategory,
                    };

                    db.QIssueFileRequirements.Add(request);
                }

                request.RequestedFrom = e.RequestedFrom ?? "Candidate";
                request.RequestedFileSubcategory = e.RequestedFileSubcategory;
                request.RequestedFileDescription = e.RequestedFileDescription;
                request.RequestedFileStatus = e.RequestedFileStatus;
            });
        }

        public void Update(CaseFileRequirementCompleted e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var request = db.QIssueFileRequirements.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.RequestedFileCategory == e.RequestedFileCategory);
                if (request != null)
                    db.QIssueFileRequirements.Remove(request);
            });
        }

        public void Update(CaseFileRequirementDeleted e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var request = db.QIssueFileRequirements.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.RequestedFileCategory == e.RequestedFileCategory);
                if (request != null)
                    db.QIssueFileRequirements.Remove(request);
            });
        }

        public void Update(CaseCommentPosted e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var comment = new QComment
                {
                    ContainerIdentifier = e.AggregateIdentifier,
                    ContainerType = "Case",

                    CommentIdentifier = e.Comment,
                    CommentText = e.Text,
                    CommentCategory = e.Category,
                    CommentSubCategory = e.SubCategory,
                    CommentFlag = e.Flag,
                    CommentTag = e.Tag,

                    AuthorUserIdentifier = e.Author,
                    CommentResolvedByUserIdentifier = e.ResolvedBy,
                    CommentAssignedToUserIdentifier = e.AssignedTo,
                    AuthorUserRole = e.AuthorRole,

                    CommentPosted = e.Authored,
                    CommentFlagged = e.Flagged,
                    CommentSubmitted = e.Submitted,
                    CommentResolved = e.Resolved,

                    IssueIdentifier = e.AggregateIdentifier,

                    OrganizationIdentifier = e.OriginOrganization,

                    TimestampCreated = e.ChangeTime,
                    TimestampCreatedBy = e.OriginUser
                };
                db.QComments.Add(comment);

                query.CommentCount++;
            });
        }

        public void Update(CommentPrivacyChanged e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var comment = db.QComments.FirstOrDefault(x => x.CommentIdentifier == e.Comment);
                if (comment != null)
                {
                    comment.CommentIsPrivate = e.CommentPrivacy;
                    db.SaveChanges();
                }
            });
        }

        public void Update(CaseCommentDeleted e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var comment = db.QComments.FirstOrDefault(x => x.CommentIdentifier == e.Comment);

                if (comment != null)
                {
                    db.QComments.Remove(comment);

                    query.CommentCount--;
                }
            });
        }

        public void Update(CaseCommentModified e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var comment = db.QComments.Single(x => x.CommentIdentifier == e.Comment);

                comment.CommentText = e.Text;
                comment.CommentFlag = e.Flag;
                comment.CommentTag = e.Tag;
                comment.CommentCategory = e.Category;
                comment.CommentSubCategory = e.SubCategory;

                comment.RevisorUserIdentifier = e.Revisor;
                comment.CommentAssignedToUserIdentifier = e.AssignedTo;
                comment.CommentResolvedByUserIdentifier = e.ResolvedBy;

                comment.CommentRevised = e.Revised;
                comment.CommentFlagged = e.Flagged;
                comment.CommentSubmitted = e.Submitted;
                comment.CommentResolved = e.Resolved;

                comment.TimestampModified = e.ChangeTime;
                comment.TimestampModifiedBy = e.OriginUser;
            });
        }

        public void Update(CaseClosed e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.IssueClosed = e.ChangeTime;
                query.IssueStatusCategory = "Closed";
            });
        }

        public void Update(CaseConnectedToSurveyResponse e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.ResponseSessionIdentifier = e.Response;
            });
        }

        public void Update(CaseDescribed e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.IssueDescription = e.Description;
            });
        }

        public void Update(CaseReopened e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.IssueClosed = null;
                query.IssueOpened = e.ChangeTime;
                query.IssueStatusCategory = "Open";
            });
        }

        public void Update(CaseStatusChanged e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.IssueStatusIdentifier = e.Status;
                query.IssueStatusEffective = e.Effective;

                var status = db.TCaseStatuses.FirstOrDefault(x => x.StatusIdentifier == e.Status);
                if (status != null)
                    query.IssueStatusCategory = status.StatusCategory;
            });
        }

        public void Update(CaseTitleChanged e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.IssueTitle = e.IssueTitle;
            });
        }

        public void Update(CaseTypeChanged e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                query.IssueType = e.IssueType;
            });
        }

        public void Update(UserAssigned e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var user = db.QIssueUsers.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.UserIdentifier == e.User && x.IssueRole == e.Role);
                if (user != null)
                {
                    db.QIssueUsers.Remove(user);
                }

                user = new QIssueUser
                {
                    JoinIdentifier = UniqueIdentifier.Create(),
                    IssueIdentifier = e.AggregateIdentifier,
                    UserIdentifier = e.User,
                    IssueRole = e.Role
                };
                db.QIssueUsers.Add(user);

                if (e.Role == "Administrator")
                    query.AdministratorUserIdentifier = e.User;
                else if (e.Role == "Lawyer")
                    query.LawyerUserIdentifier = e.User;
                else if (e.Role == "Owner")
                    query.OwnerUserIdentifier = e.User;
                else if (e.Role == "Topic")
                    query.TopicUserIdentifier = e.User;

                query.PersonCount++;
            });
        }

        public void Update(UserUnassigned e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var user = db.QIssueUsers.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.UserIdentifier == e.User && x.IssueRole == e.Role);
                if (user != null)
                    db.QIssueUsers.Remove(user);

                if (e.Role == "Administrator")
                    query.AdministratorUserIdentifier = null;
                else if (e.Role == "Lawyer")
                    query.LawyerUserIdentifier = null;
                else if (e.Role == "Owner")
                    query.OwnerUserIdentifier = null;
                else if (e.Role == "Topic")
                    query.TopicUserIdentifier = null;

                query.PersonCount--;
            });
        }

        public void Update(GroupAssigned e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var group = new QIssueGroup
                {
                    JoinIdentifier = UniqueIdentifier.Create(),
                    IssueIdentifier = e.AggregateIdentifier,
                    GroupIdentifier = e.Group,
                    IssueRole = e.Role
                };
                db.QIssueGroups.Add(group);

                if (e.Role == "Employer")
                    query.EmployerGroupIdentifier = e.Group;

                query.GroupCount++;
            });
        }

        public void Update(GroupUnassigned e)
        {
            UpdateCursor(e, (db, state, query) =>
            {
                var user = db.QIssueGroups.FirstOrDefault(x => x.IssueIdentifier == e.AggregateIdentifier && x.GroupIdentifier == e.Group && x.IssueRole == e.Role);
                if (user != null) db.QIssueGroups.Remove(user);

                if (e.Role == "Employer")
                    query.EmployerGroupIdentifier = null;

                query.GroupCount--;
            });
        }
    }
}