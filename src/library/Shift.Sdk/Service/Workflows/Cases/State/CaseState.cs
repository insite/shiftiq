using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseState : AggregateState
    {
        /// <summary>
        /// Uniquely identifies the issue itself.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the organization that create the issue.
        /// </summary>
        public Guid Organization { get; set; }

        /// <summary>
        /// The title text of the issue.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description text of the issue.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The current status of the issue.
        /// </summary>
        public Guid? Status { get; set; }
        public DateTimeOffset? StatusEffective { get; set; }

        /// <summary>
        /// The type of the issue.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The time when issue has been opened.
        /// </summary>
        public DateTimeOffset Opened { get; set; }
        public Guid? OpenedBy { get; set; }

        /// <summary>
        /// The time when issue has been closed.
        /// </summary>
        public DateTimeOffset? Closed { get; set; }
        public Guid? ClosedBy { get; set; }

        /// <summary>
        /// Unique identifies the administrator.
        /// </summary>
        public Guid? Administrator { get; set; }

        /// <summary>
        /// Unique identifies the topic.
        /// </summary>
        public Guid? Topic { get; set; }

        /// <summary>
        /// Unique identifies the topic.
        /// </summary>
        public Guid? Owner { get; set; }

        /// <summary>
        /// Unique identifies the lawyer.
        /// </summary>
        public Guid? Lawyer { get; set; }

        /// <summary>
        /// Issue can have any number of attached files. Each file is stored in the file system with a physical file 
        /// name that looks like this - \Data\Files\Core\UploadIdentifier.extension - and information about the 
        /// uploaded file is stored in the database table Upload. The convention that should be followed for the 
        /// Upload.NavigateUrl property (used to view/download the file) is this:
        ///    /Issues/{Issue.Identifier}/Attachments/{Upload.Name}
        /// </summary>
        public List<IssueAttachment> Attachments { get; set; }

        /// <summary>
        /// Issue can have any number of comments. Comments might be posted by instructors authoring questions,
        /// subject-matter-experts reviewing and revising questions, students submitting exams, etc.
        /// </summary>
        public List<IssueComment> Comments { get; set; }

        /// <summary>
        /// The source of the issue.
        /// </summary>
        public string Source { get; set; }

        #region Methods (event handling)

        public void When(GroupAssigned _) { }
        public void When(GroupUnassigned _) { }

        public void When(UserAssigned _) { }
        public void When(UserUnassigned _) { }

        public void When(CaseAttachmentAdded _) { }
        public void When(CaseAttachmentFileChanged _) { }
        public void When(CaseAttachmentFileRenamed _) { }
        public void When(CaseAttachmentDeleted _) { }
        public void When(CaseFileRequirementAdded _) { }
        public void When(CaseFileRequirementModified _) { }
        public void When(CaseFileRequirementCompleted _) { }
        public void When(CaseFileRequirementDeleted _) { }
        public void When(CaseCommentPosted _) { }
        public void When(CommentPrivacyChanged _) { }
        public void When(CaseCommentDeleted _) { }
        public void When(CaseCommentModified _) { }
        public void When(CaseConnectedToSurveyResponse _) { }
        public void When(CaseDeleted _) { }
        public void When(CaseDescribed _) { }
        public void When(CaseTitleChanged _) { }
        public void When(CaseTypeChanged _) { }

        public void When(CaseOpened2 e) 
        { 
            Opened = e.ChangeTime; 
            OpenedBy = e.OriginUser; 
        }

        public void When(CaseClosed e)
        { 
            Closed = e.ChangeTime; 
            ClosedBy = e.OriginUser; 
        }
        
        public void When(CaseReopened _)
        {
            StatusEffective = null;
            Status = null;

            Closed = null; 
            ClosedBy = null; 
        }
        
        public void When(CaseStatusChanged c)
        {
            Status = c.Status;
            StatusEffective = c.Effective;
        }

        public void When(SerializedChange _)
        {
            // Obsolete changes go here
        }

        #endregion
    }
}
