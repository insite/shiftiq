using System.Web;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class SubjectOutputDetails : BaseUserControl
    {
        #region Methods (data binding)

        public void LoadData(BankState bank, Comment comment)
        {
            BankName.Text = "N/A";
            SubjectType.Text = comment.Type.GetName();
            SubjectName.Text = "N/A";

            SubjectContainerField.Visible = false;

            if (bank == null)
                return;

            BankName.Text = GetSubjectName(bank);

            if (comment.Type == CommentType.Bank)
            {
                SubjectName.Text = GetSubjectName(bank);

                BankField.Visible = false;
            }
            else if (comment.Type == CommentType.Set)
            {
                var set = bank.FindSet(comment.Subject);
                if (set != null)
                    SubjectName.Text = GetSubjectName(set);
            }
            else if (comment.Type == CommentType.Question)
            {
                var question = bank.FindQuestion(comment.Subject);
                if (question != null)
                {
                    SubjectName.Text = GetSubjectName($"{question.BankIndex + 1}. <span style='white-space:pre-wrap;'>{HttpUtility.HtmlEncode(question.Content.Title?.Default)}</span>", question.Asset);

                    SubjectContainerField.Visible = true;
                    SubjectContainerType.Text = "Set";
                    SubjectContainerName.Text = GetSubjectName(question.Set);
                }
            }
            else if (comment.Type == CommentType.Field)
            {
                var field = bank.FindField(comment.Subject);
                if (field != null)
                {
                    SubjectName.Text = GetSubjectName($"{field.FormSequence}. <span style='white-space:pre-wrap;'>{HttpUtility.HtmlEncode(field.Question.Content.Title?.Default)}</span>", field.Question.Asset);

                    SubjectContainerField.Visible = true;
                    SubjectContainerType.Text = "Form";
                    SubjectContainerName.Text = GetSubjectName(field.Section.Form);
                }
            }
            else if (comment.Type == CommentType.Criterion)
            {
                var sieve = bank.FindCriterion(comment.Subject);
                if (sieve != null)
                    SubjectName.Text = sieve.ToString();
            }
            else if (comment.Type == CommentType.Specification)
            {
                var spec = bank.FindSpecification(comment.Subject);
                if (spec != null)
                    SubjectName.Text = GetSubjectName(spec);
            }
            else if (comment.Type == CommentType.Form)
            {
                var form = bank.FindForm(comment.Subject);
                if (form != null)
                {
                    SubjectName.Text = GetSubjectName(form);

                    SubjectContainerField.Visible = true;
                    SubjectContainerType.Text = "Specification";
                    SubjectContainerName.Text = GetSubjectName(form.Specification);
                }
            }
            else
                throw new ApplicationError("Unexpected comment type: " + comment.Type.GetName());
        }

        #endregion

        #region Methods (helpers)

        private static string GetSubjectName(string name) =>
            $"{name ?? "<strong>(Untitled)</strong>"}";

        private static string GetSubjectName(string name, int asset) =>
            $"{name ?? "<strong>(Untitled)</strong>"} <span class='form-text'>Asset #{asset}</span>";

        private static string GetSubjectName(BankState bank) =>
            GetSubjectName(HttpUtility.HtmlEncode(bank.Name), bank.Asset);

        private static string GetSubjectName(Set set) =>
            GetSubjectName(HttpUtility.HtmlEncode(set.Name));

        private static string GetSubjectName(Specification spec) =>
            GetSubjectName(HttpUtility.HtmlEncode(spec.Name), spec.Asset);

        private static string GetSubjectName(Form form) =>
            GetSubjectName(HttpUtility.HtmlEncode(form.Name), form.Asset);

        #endregion
    }
}