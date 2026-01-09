using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Specifications.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class SpecificationsSection : BaseUserControl
    {
        #region Properties

        private Guid BankID
        {
            get => (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        private int SpecificationCriteriaCount
        {
            get => (int)(ViewState[nameof(SpecificationCriteriaCount)] ?? 0);
            set => ViewState[nameof(SpecificationCriteriaCount)] = value;
        }

        private bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        public Action ReloadOutline { get; internal set; }

        public Func<BankState> LoadBank { get; internal set; }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SpecificationSelector.AutoPostBack = true;
            SpecificationSelector.ValueChanged += SpecificationSelector_ValueChanged;

            CommandsDropDown.Click += CommandsDropDown_Click;
            SpecificationWorkshopButton.Click += SpecificationWorkshopButton_Click;
        }

        protected override void CreateChildControls()
        {
            if (SpecificationCriteriaNav.ItemsCount == 0)
            {
                for (var i = 0; i < SpecificationCriteriaCount; i++)
                    AddSpecificationCriterionItem(out _, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Event handlers

        private void SpecificationSelector_ValueChanged(object sender, EventArgs e) => OnSpecificationSelected();

        private void OnSpecificationSelected()
        {
            var spec = GetSelectedSpecification();

            BindSpecification(spec, CanWrite);
        }

        private void CommandsDropDown_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AddSpecification")
            {
                HttpResponseHelper.Redirect($"/ui/admin/assessments/specifications/add?bank={BankID}");
            }
            else if (e.CommandName == "AddCriterion")
            {
                var spec = GetSelectedSpecification();
                if (spec != null)
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/criteria/add?bank={BankID}&spec={spec.Identifier}");
            }
            else if (e.CommandName == "AddForm")
            {
                var spec = GetSelectedSpecification();
                if (spec != null)
                {
                    var returnUrl = new ReturnUrl($"bank&spec={spec.Identifier}");
                    var redirectUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/forms/add?bank={BankID}&spec={spec.Identifier}");

                    HttpResponseHelper.Redirect(redirectUrl);
                }
            }
        }

        private void SpecificationWorkshopButton_Click(object sender, EventArgs e)
        {
            var spec = GetSelectedSpecification();
            if (spec == null)
                return;

            HttpResponseHelper.Redirect($"/ui/admin/assessments/specifications/workshop?bank={BankID}&spec={spec.Identifier}");
        }

        #endregion

        #region Data binding

        public void LoadData(BankState bank, bool canWrite, out bool isSelected)
        {
            CanWrite = canWrite;
            BankID = bank.Identifier;

            isSelected = false;

            var hasSpecs = bank.Specifications.IsNotEmpty();

            SpecificationSelector.Visible = hasSpecs;
            SpecificationWorkshopButton.Visible = hasSpecs;
            CommandColumn.Visible = canWrite;

            foreach (var item in CommandsDropDown.Items)
            {
                if (item.Name == "AddCriterion" || item.Name == "AddForm")
                    item.Visible = hasSpecs;
            }

            if (!hasSpecs)
                return;

            Guid? sieveId = null, specificationId = null;

            if (!IsPostBack)
            {
                if (Request.QueryString["panel"] == "specifications")
                    isSelected = true;

                if (Guid.TryParse(Request["sieve"], out var sieveValue))
                {
                    sieveId = sieveValue;
                    specificationId = bank.Specifications.Where(x => x.Criteria.Any(y => y.Identifier == sieveId)).FirstOrDefault()?.Identifier;

                    if (!specificationId.HasValue)
                        sieveId = null;
                }
                else if (Guid.TryParse(Request["spec"], out var specValue))
                {
                    specificationId = specValue;
                }
            }

            SpecificationSelector.Items.Clear();

            Tuple<Specification, ComboBoxOption> selectedSpec = null;

            foreach (var spec in bank.Specifications.OrderBy(x => x.Name))
            {
                var item = new ComboBoxOption(spec.Name, spec.Identifier.ToString());

                SpecificationSelector.Items.Add(item);

                if (selectedSpec == null || specificationId.HasValue && spec.Identifier == specificationId.Value)
                    selectedSpec = new Tuple<Specification, ComboBoxOption>(spec, item);
            }

            if (selectedSpec != null)
                selectedSpec.Item2.Selected = true;

            OnSpecificationSelected();

            if (specificationId.HasValue && selectedSpec.Item1.Identifier == specificationId.Value)
            {
                isSelected = true;

                if (sieveId.HasValue)
                {
                    foreach (var item in SpecificationCriteriaNav.GetItems())
                    {
                        var details = (SpecificationCriterionDetails)item.Controls[0];

                        if (sieveId.HasValue && details.CriterionID.HasValue && details.CriterionID == sieveId.Value)
                            item.IsSelected = true;
                    }
                }
                else
                {
                    foreach (var item in SpecificationCriteriaNav.GetItems())
                    {
                        var details = (SpecificationCriterionDetails)item.Controls[0];

                        details.OpenSpecificationTab();
                    }
                }
            }
        }

        private void BindSpecification(Specification spec, bool canWrite)
        {
            CanWrite = canWrite;

            var hasCriteria = spec.Criteria.IsNotEmpty();
            var isSpecTabOpened = false;

            {
                var navItems = SpecificationCriteriaNav.GetItems();
                if (navItems.Count > 0)
                {
                    var details = (SpecificationCriterionDetails)navItems[0].Controls[0];
                    isSpecTabOpened = details.IsSpecificationTabOpened();
                }
            }

            SpecificationCriteriaCount = 0;
            SpecificationCriteriaNav.ClearItems();

            if (hasCriteria)
            {
                foreach (var criterion in spec.Criteria)
                {
                    AddSpecificationCriterionItem(out var navItem, out var details);

                    navItem.Title = criterion.Title;

                    details.SetInputValues(criterion, canWrite);

                    SpecificationCriteriaCount++;

                    if (isSpecTabOpened)
                        details.OpenSpecificationTab();
                }
            }
            else
            {
                AddSpecificationCriterionItem(out var navItem, out var details);

                navItem.Title = "All Questions";

                details.SetInputValues(spec, canWrite);

                if (isSpecTabOpened)
                    details.OpenSpecificationTab();

                SpecificationCriteriaCount = 1;
            }

            var specQuestionLimit = spec.QuestionLimit;
            if (specQuestionLimit > 0)
            {
                var specQuestionLimitSum = spec.Criteria.Sum(x => x.FilterType == CriterionFilterType.Pivot ? x.PivotFilter.CellValueSum : x.QuestionLimit);

                if (specQuestionLimitSum > specQuestionLimit)
                {
                    SpecificationWarning.AddMessage(AlertType.Warning, $"Specification limit exceeded on {specQuestionLimitSum - specQuestionLimit} questions");
                }
                else if (specQuestionLimitSum < specQuestionLimit)
                {
                    SpecificationWarning.AddMessage(AlertType.Warning, $"Specification requires {specQuestionLimit - specQuestionLimitSum} more questions");
                }
            }
        }

        #endregion

        #region Helper methods

        private void AddSpecificationCriterionItem(out NavItem navItem, out SpecificationCriterionDetails details)
        {
            SpecificationCriteriaNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(details = (SpecificationCriterionDetails)LoadControl("~/UI/Admin/Assessments/Specifications/Controls/SpecificationCriterionDetails.ascx"));
        }

        private Specification GetSelectedSpecification()
        {
            var bank = LoadBank();
            var specId = SpecificationSelector.ValueAsGuid;
            var spec = specId.HasValue ? bank.FindSpecification(specId.Value) : null;

            if (spec == null)
                ReloadOutline();

            return spec;
        }

        #endregion
    }
}