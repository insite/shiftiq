using InSite.Application.QuizAttempts.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assessments.QuizAttempts.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TQuizAttemptFilter>
    {
        public override TQuizAttemptFilter Filter
        {
            get
            {
                var filter = new TQuizAttemptFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    QuizType = QuizType.Value,
                    QuizNameContains = QuizName.Text,
                    LearnerNameContains = LearnerName.Text,
                    LearnerEmailContains = LearnerEmail.Text,
                    IsCompleted = IsCompleted.ValueAsBoolean,
                    AttemptStartedSince = AttemptStartedSince.Value,
                    AttemptStartedBefore = AttemptStartedBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                QuizType.Value = value.QuizType;
                QuizName.Text = value.QuizNameContains;
                LearnerName.Text = value.LearnerNameContains;
                LearnerEmail.Text = value.LearnerEmailContains;
                IsCompleted.ValueAsBoolean = value.IsCompleted;
                AttemptStartedSince.Value = value.AttemptStartedSince;
                AttemptStartedBefore.Value = value.AttemptStartedBefore;
            }
        }

        public override void Clear()
        {
            QuizType.Value = null;
            QuizName.Text = null;
            LearnerName.Text = null;
            LearnerEmail.Text = null;
            IsCompleted.ValueAsBoolean = null;
            AttemptStartedSince.Value = null;
            AttemptStartedBefore.Value = null;
        }
    }
}