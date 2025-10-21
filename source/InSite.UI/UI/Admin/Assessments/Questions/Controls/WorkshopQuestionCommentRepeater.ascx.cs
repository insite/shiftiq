using System;
using System.Linq;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class WorkshopQuestionCommentRepeater : BaseUserControl
    {
        public Guid BankID
        {
            get => InnerBankID ?? (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        protected Guid? InnerBankID
        {
            get => (Guid?)ViewState[nameof(InnerBankID)];
            set => ViewState[nameof(InnerBankID)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(WorkshopQuestionCommentRepeater).FullName;
            CommonScript.ContentKey = typeof(WorkshopQuestionCommentRepeater).FullName;
        }

        public void LoadFormData(Guid formId, Question question, ReturnUrl returnUrl)
        {
            InnerBankID = question.Set.Bank.Identifier;

            var internalReturnUrl = returnUrl.Copy();
            internalReturnUrl.Append($"question={question.Identifier}");

            var adminComments = question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Administrator).ToArray();
            var candidateCommentsCount = question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Candidate).Count();

            CommentRepeater.LoadData(BankID, adminComments, internalReturnUrl);

            CandidateCommentaryLink.Visible = candidateCommentsCount > 0;
            CandidateCommentaryLink.InnerText = "Candidate Comments".ToQuantity(candidateCommentsCount);
            CandidateCommentaryLink.HRef =
                $"/ui/admin/assessments/bankscomments/search?bank={BankID}&form={formId}&question={question.Identifier}&role=Candidate&showAuthor=0&panel=results";
        }

        public void LoadSpecificationData(Question question, ReturnUrl returnUrl)
        {
            InnerBankID = question.Set.Bank.Identifier;

            var internalReturnUrl = returnUrl.Copy();
            internalReturnUrl.Append($"question={question.Identifier}");

            var adminComments = question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Administrator).ToArray();
            var candidateCommentsCount = question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Candidate).Count();

            CommentRepeater.LoadData(BankID, adminComments, internalReturnUrl);

            CandidateCommentaryLink.Visible = candidateCommentsCount > 0;
            CandidateCommentaryLink.InnerText = "Candidate Comments".ToQuantity(candidateCommentsCount);
            CandidateCommentaryLink.HRef =
                $"/ui/admin/assessments/bankscomments/search?bank={BankID}&question={question.Identifier}&role=Candidate&showAuthor=0&panel=results";
        }
    }
}