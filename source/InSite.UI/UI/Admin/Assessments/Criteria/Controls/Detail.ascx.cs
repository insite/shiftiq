using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Criteria.Controls
{
    public partial class Detail : BaseUserControl
    {
        #region Properties

        public Guid? CriterionId
        {
            get => (Guid?)ViewState[nameof(CriterionId)];
            private set => ViewState[nameof(CriterionId)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonHeaderLiteral.ContentKey = typeof(Detail).FullName;
            CommonFooterLiteral.ContentKey = typeof(Detail).FullName;
        }

        #endregion

        #region Methods (data binding)

        public void SetInputValues(Criterion criterion, bool canWrite)
        {
            var spec = criterion.Specification;
            var bankId = spec.Bank.Identifier;
            var criterionId = CriterionId = criterion.Identifier;

            CriterionNumber.Text = $"{criterion.Sequence} of {spec.Criteria.Count}";

            SetRepeater.DataSource = criterion.Sets;
            SetRepeater.DataBind();

            StandardRepeater.DataSource = criterion.Sets;
            StandardRepeater.DataBind();

            SetWeight.Text = criterion.SetWeight.ToString("n2");

            QuestionLimitField.Visible = criterion.FilterType != CriterionFilterType.Pivot;
            QuestionLimit.Text = criterion.QuestionLimit.ToString("n0") + " of " + criterion.Sets.SelectMany(x => x.Questions).Count().ToString("n0");

            var hasSections = criterion.Sections.IsNotEmpty();

            SectionsContainer.Visible = false;

            if (hasSections)
            {
                SectionsRepeater.DataSource = criterion.Sections.Select(x => new
                {
                    FormTitle = x.Form.Content?.Title?.Default,
                    x.Letter,
                    FieldsCount = x.Fields?.Count ?? 0,
                });
                SectionsRepeater.DataBind();
            }

            var hasBasicFilter = !string.IsNullOrEmpty(criterion.TagFilter);
            var hasAdvancedFilter = !hasBasicFilter && criterion.PivotFilter != null && !criterion.PivotFilter.IsEmpty;

            FilterType.Text = hasBasicFilter ? "Filter with Question Tags" : hasAdvancedFilter ? "Filter with Pivot Table" : "Include All Questions";

            var editCriterionUrl = $"/ui/admin/assessments/criteria/change-filter?bank={bankId}&criterion={criterionId}";
            EditCriterionLink1.NavigateUrl = editCriterionUrl;
            EditCriterionLink2.NavigateUrl = editCriterionUrl;
            EditCriterionLink3.NavigateUrl = editCriterionUrl;
            EditCriterionLink4.NavigateUrl = editCriterionUrl;
            EditCriterionLink5.NavigateUrl = editCriterionUrl;
            EditCriterionLink6.NavigateUrl = editCriterionUrl;
            EditCriterionLink7.NavigateUrl = editCriterionUrl;

            DeleteCriterionLink.NavigateUrl = $"/admin/assessments/criteria/delete?bank={bankId}&criterion={criterionId}";

            OutputContentTitle.InnerText = (criterion.Content.Title?.Default).IfNullOrEmpty("None");
            OutputContentSummary.InnerText = (criterion.Content.Summary?.Default).IfNullOrEmpty("None");

            ContentContainer.Visible = spec.Type == SpecificationType.Dynamic;
            EditContentTitle.NavigateUrl = $"/ui/admin/assessments/criteria/content?bank={bankId}&criterion={criterionId}&tab=title";
            EditContentSummary.NavigateUrl = $"/ui/admin/assessments/criteria/content?bank={bankId}&criterion={criterionId}&tab=summary";

            BasicFilterContainer.Visible = hasBasicFilter;
            BasicFilterOutput.InnerText = criterion.TagFilter;

            AdvancedFilterContainer.Visible = hasAdvancedFilter;
            AdvancedFilterOutput.Clear();

            if (hasAdvancedFilter)
                AdvancedFilterOutput.LoadData(criterion);

            DeleteCriterionLink.Visible = canWrite;
            EditCriterionLink1.Visible = canWrite;
            EditCriterionLink2.Visible = canWrite;
            EditCriterionLink3.Visible = canWrite;
            EditCriterionLink4.Visible = canWrite;
            EditCriterionLink5.Visible = canWrite;
            EditCriterionLink6.Visible = canWrite;
            EditCriterionLink7.Visible = canWrite;

            SetTabConfigInputValues(criterion);
        }

        public void SetTabConfigInputValues(Specification specification)
        {
            var id = CriterionId;
            var criterion = specification.Criteria.FirstOrDefault(x => x.Identifier == id);

            if (criterion == null)
                HttpResponseHelper.Redirect(Request.RawUrl);

            SetTabConfigInputValues(criterion);
        }

        private void SetTabConfigInputValues(Criterion criterion)
        {
            var spec = criterion.Specification;
            var tabConfig = criterion.TabConfiguration;

            TabConfigContainer.Visible = spec.Type == SpecificationType.Dynamic
                && spec.SectionsAsTabsEnabled
                && !spec.TabNavigationEnabled;

            var limitSomeTabs = spec.TabTimeLimit == SpecificationTabTimeLimit.SomeTabs;
            var limitAllTabs = spec.TabTimeLimit == SpecificationTabTimeLimit.AllTabs;

            WarningOnNextTabEnabled.Text = tabConfig.WarningOnNextTabEnabled ? "Show" : "Disabled";

            BreakTimerEnabledField.Visible = limitSomeTabs || limitAllTabs;
            BreakTimerEnabled.Text = tabConfig.BreakTimerEnabled ? "Enabled" : "Disabled";

            TimeLimitField.Visible = limitAllTabs || limitSomeTabs && tabConfig.BreakTimerEnabled;
            TimeLimit.Text = tabConfig.TimeLimit <= 0 ? "None" : $"{tabConfig.TimeLimit} minute(s)";

            TimerTypeField.Visible = limitAllTabs || limitSomeTabs && tabConfig.BreakTimerEnabled;
            TimerType.Text = tabConfig.TimerType.GetDescription();
        }

        #endregion
    }
}
