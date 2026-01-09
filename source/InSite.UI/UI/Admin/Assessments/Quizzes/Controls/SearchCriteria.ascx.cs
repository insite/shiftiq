using InSite.Application.Quizzes.Read;
using InSite.Common.Web.UI;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Quizzes.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TQuizFilter>
    {
        public override TQuizFilter Filter
        {
            get
            {
                var filter = new TQuizFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    QuizType = GetQuizType(!IsPostBack, QuizType.Value),
                    QuizNameContains = QuizName.Text,
                    QuizDataContains = QuizText.Text,
                    TimeLimitFrom = TimeLimitFrom.ValueAsInt,
                    TimeLimitThru = TimeLimitThru.ValueAsInt,
                    AttemptLimitFrom = AttemptLimitFrom.ValueAsInt,
                    AttemptLimitThru = AttemptLimitThru.ValueAsInt,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                QuizType.Value = GetQuizType(!IsPostBack, value.QuizType);
                QuizName.Text = value.QuizNameContains;
                QuizText.Text = value.QuizDataContains;
                TimeLimitFrom.ValueAsInt = value.TimeLimitFrom;
                TimeLimitThru.ValueAsInt = value.TimeLimitThru;
                AttemptLimitFrom.ValueAsInt = value.AttemptLimitFrom;
                AttemptLimitThru.ValueAsInt = value.AttemptLimitThru;
            }
        }

        public override void Clear()
        {
            QuizType.Value = GetQuizType(true, null);
            QuizName.Text = null;
            QuizText.Text = null;
            TimeLimitFrom.ValueAsInt = null;
            TimeLimitThru.ValueAsInt = null;
            AttemptLimitFrom.ValueAsInt = null;
            AttemptLimitThru.ValueAsInt = null;
        }

        private string GetQuizType(bool isNeedDefault, string filterValue)
        {
            return isNeedDefault
                ? (QuizHelper.TypeFromQueryValue(Request.QueryString["type"]) ?? filterValue)
                : filterValue;
        }
    }
}