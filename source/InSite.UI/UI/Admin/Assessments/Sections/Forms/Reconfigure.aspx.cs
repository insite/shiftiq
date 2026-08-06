using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Sections.Forms
{
    public partial class Reconfigure : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SectionID => Guid.TryParse(Request.QueryString["section"], out var value) ? value : Guid.Empty;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (IsPostBack)
                return;

            Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(SectionID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
            {
                RedirectToSearch();
                return;
            }

            var section = bank.FindSection(SectionID);
            if (section == null)
                RedirectToReader();

            var specification = section.Form.Specification;
            if (!specification.SectionsAsTabsEnabled || specification.TabNavigationEnabled)
                RedirectToReader(SectionID);

            SetInputValues(section);
        }

        private void Save()
        {
            var tabConfig = new SectionTabConfiguration();

            TabConfig.GetValues(tabConfig);

            ServiceLocator.SendCommand(new ReconfigureSection(
                BankID, SectionID,
                tabConfig.WarningOnNextTabEnabled,
                tabConfig.BreakTimerEnabled,
                tabConfig.TimeLimit,
                tabConfig.TimerType));
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Section section)
        {
            var criterion = section.Criterion;
            var form = section.Form;
            var specification = form.Specification;
            var bank = specification.Bank;

            var title =
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            // Section

            SectionNumber.Text = $"{section.Sequence} of {form.Sections.Count}";
            SectionContentTitle.InnerText = (section.Content.Title?.Default).IfNullOrEmpty("None");
            SectionContentSummary.InnerText = (section.Content.Summary?.Default).IfNullOrEmpty("None");

            // Configuration

            TabConfig.SetValues(section.TabConfiguration, specification.TabTimeLimit);

            // Criterion

            SetRepeater.DataSource = criterion.Sets;
            SetRepeater.DataBind();

            SetWeight.Text = criterion.SetWeight.ToString("n2");

            QuestionLimitField.Visible = criterion.FilterType != CriterionFilterType.Pivot;
            QuestionLimit.Text = criterion.QuestionLimit.ToString("n0") + " of " + criterion.Sets.SelectMany(x => x.Questions).Count().ToString("n0");

            var hasBasicFilter = !string.IsNullOrEmpty(criterion.TagFilter);
            var hasAdvancedFilter = !hasBasicFilter && criterion.PivotFilter != null && !criterion.PivotFilter.IsEmpty;

            FilterType.Text = hasBasicFilter ? "Question Tag Filter" : hasAdvancedFilter ? "Pivot Table Filter" : "Include All Questions";

            TagFilterField.Visible = hasBasicFilter;
            TagFilter.Text = criterion.TagFilter;

            // Other

            CancelButton.NavigateUrl = GetReaderUrl(section.Identifier);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? sectionId = null)
        {
            var url = GetReaderUrl(sectionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? sectionId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (sectionId.HasValue)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                var section = bank.FindSection(SectionID);

                url += $"&form={section.Form.Identifier}&section={section.Identifier}&tab=section";
            }

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}
