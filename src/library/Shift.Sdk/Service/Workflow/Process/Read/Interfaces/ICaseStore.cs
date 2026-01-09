using System;

using InSite.Domain.Issues;

namespace InSite.Application.Issues.Read
{
    public interface ICaseStore
    {
        void DeleteAll();
        
        void Delete(Guid value);
        void Insert(CaseOpened2 e);

        void Update(CaseAttachmentAdded e);
        void Update(CaseAttachmentFileChanged e);
        void Update(CaseAttachmentFileRenamed e);
        void Update(CaseAttachmentDeleted e);
        void Update(CaseFileRequirementAdded e);
        void Update(CaseFileRequirementModified e);
        void Update(CaseFileRequirementCompleted e);
        void Update(CaseFileRequirementDeleted e);
        void Update(CaseCommentPosted e);
        void Update(CommentPrivacyChanged e);
        void Update(CaseCommentDeleted e);
        void Update(CaseCommentModified e);
        void Update(GroupAssigned e);
        void Update(GroupUnassigned e);
        void Update(CaseClosed e);
        void Update(CaseConnectedToSurveyResponse e);
        void Update(CaseDescribed e);
        void Update(CaseReopened e);
        void Update(CaseStatusChanged e);
        void Update(CaseTitleChanged e);
        void Update(CaseTypeChanged e);
        void Update(UserAssigned e);
        void Update(UserUnassigned e);
    }
}
