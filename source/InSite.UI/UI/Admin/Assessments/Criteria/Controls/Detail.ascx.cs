using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Criteria.Controls
{
    public partial class Detail : BaseUserControl
    {
        #region Properties

        public Guid? CriterionID
        {
            get => (Guid?)ViewState[nameof(CriterionID)];
            set => ViewState[nameof(CriterionID)] = value;
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
            CriterionID = criterion.Identifier;

            CriterionNumber.Text = $"{criterion.Sequence} of {criterion.Specification.Criteria.Count}";

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

            EditSetFilter1.NavigateUrl = $"/ui/admin/assessments/criteria/change-filter?bank={criterion.Specification.Bank.Identifier}&criterion={criterion.Identifier}";
            EditSetFilter2.NavigateUrl = EditSetFilter1.NavigateUrl;
            EditSetFilter3.NavigateUrl = EditSetFilter1.NavigateUrl;

            DeleteCriterionLink.NavigateUrl = $"/admin/assessments/criteria/delete?bank={criterion.Specification.Bank.Identifier}&criterion={criterion.Identifier}";

            BasicFilterContainer.Visible = hasBasicFilter;
            BasicFilterOutput.InnerText = criterion.TagFilter;

            AdvancedFilterContainer.Visible = hasAdvancedFilter;
            AdvancedFilterOutput.Clear();

            if (hasAdvancedFilter)
                AdvancedFilterOutput.LoadData(criterion);

            DeleteCriterionLink.Visible = canWrite;
            EditSetFilter1.Visible = canWrite;
            EditSetFilter2.Visible = canWrite;
            EditSetFilter3.Visible = canWrite;
        }

        #endregion
    }
}