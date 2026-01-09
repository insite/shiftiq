using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public partial class SpecificationDetails : BaseUserControl
    {
        private Guid BankIdentifier
        {
            get => (Guid)ViewState[nameof(BankIdentifier)];
            set => ViewState[nameof(BankIdentifier)] = value;
        }

        private Guid SpecificationIdentifier
        {
            get => (Guid)ViewState[nameof(SpecificationIdentifier)];
            set => ViewState[nameof(SpecificationIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DisableSectionsAsTabsButton.Click += (s, a) => ChangeSectionsAsTabs(false);
            EnableSectionsAsTabsButton.Click += (s, a) => ChangeSectionsAsTabs(true);

            DisableTabNavigationButton.Click += (s, a) => ChangeTabNavigation(false);
            EnableTabNavigationButton.Click += (s, a) => ChangeTabNavigation(true);

            DisableSingleQuestionPerTabButton.Click += (s, a) => ChangeSingleQuestionPerTab(false);
            EnableSingleQuestionPerTabButton.Click += (s, a) => ChangeSingleQuestionPerTab(true);
        }

        public void SetInputValues(Specification spec, bool canWrite)
        {
            BankIdentifier = spec.Bank.Identifier;
            SpecificationIdentifier = spec.Identifier;

            SpecificationName.Text = spec.Name;
            SpecificationType.Text = spec.Type.GetName();
            SpecificationTypeHelp.InnerText = SpecHelper.GetDescription(spec.Type);

            SetScenarioFields(spec);

            SpecificationFormLimit.Text = spec.FormLimit.ToString("n0");
            SpecificationQuestionLimit.Text = spec.QuestionLimit.ToString("n0");
            SpecificationCalculationDisclosure.Text = $"{spec.Calculation.Disclosure.GetName()}";
            SpecificationCalculationPassingScore.Text = $"{spec.Calculation.PassingScore:p0}";

            var parameters = $"bank={spec.Bank.Identifier}&spec={spec.Identifier}";
            ReconfigureLink.NavigateUrl = $"/ui/admin/assessments/specifications/reconfigure?{parameters}";
            ChangeCalculationLink.NavigateUrl = $"/ui/admin/assessments/specifications/change-calculation?{parameters}";
            RenameSpecificationLink.NavigateUrl = $"/ui/admin/assessments/specifications/rename?{parameters}";
            DeleteSpecificationLink.NavigateUrl = $"/admin/assessments/specifications/delete?{parameters}";

            DeleteSpecificationLink.Visible = canWrite;
            RenameSpecificationLink.Visible = canWrite;
            ReconfigureLink.Visible = canWrite;
            ChangeCalculationLink.Visible = canWrite;
        }

        private void SetScenarioFields()
        {
            var spec = ServiceLocator.BankSearch.GetBankState(BankIdentifier)?.FindSpecification(SpecificationIdentifier);
            if (spec != null)
                SetScenarioFields(spec);
            else
                HttpResponseHelper.Redirect(Request.RawUrl);
        }

        private void SetScenarioFields(Specification spec)
        {
            ScenarioFields.Visible = spec.Type == Shift.Constant.SpecificationType.Static;
            SectionsAsTabsOutput.Text = spec.SectionsAsTabsEnabled ? "Enabled" : "Disabled";
            DisableSectionsAsTabsButton.Visible = spec.SectionsAsTabsEnabled;
            EnableSectionsAsTabsButton.Visible = !spec.SectionsAsTabsEnabled;

            TabNavigationField.Visible = spec.SectionsAsTabsEnabled;
            TabNavigationOutput.Text = spec.TabNavigationEnabled ? "Enabled" : "Disabled";
            DisableTabNavigationButton.Visible = spec.TabNavigationEnabled;
            EnableTabNavigationButton.Visible = !spec.TabNavigationEnabled;

            SingleQuestionPerTabField.Visible = spec.SectionsAsTabsEnabled && !spec.TabNavigationEnabled;
            SingleQuestionPerTabOutput.Text = spec.SingleQuestionPerTabEnabled ? "Enabled" : "Disabled";
            DisableSingleQuestionPerTabButton.Visible = spec.SingleQuestionPerTabEnabled;
            EnableSingleQuestionPerTabButton.Visible = !spec.SingleQuestionPerTabEnabled;

            TabTimeLimitField.Visible = spec.SectionsAsTabsEnabled && !spec.TabNavigationEnabled;
            TabTimeLimitOutput.Text = spec.TabTimeLimit.GetDescription();
        }

        private void ChangeSectionsAsTabs(bool enable)
        {
            if (enable)
                ServiceLocator.SendCommand(new EnableSectionsAsTabs(BankIdentifier, SpecificationIdentifier));
            else
                ServiceLocator.SendCommand(new DisableSectionsAsTabs(BankIdentifier, SpecificationIdentifier));

            SetScenarioFields();
        }

        private void ChangeTabNavigation(bool enabled)
        {
            if (enabled)
                ServiceLocator.SendCommand(new EnableTabNavigation(BankIdentifier, SpecificationIdentifier));
            else
                ServiceLocator.SendCommand(new DisableTabNavigation(BankIdentifier, SpecificationIdentifier));

            SetScenarioFields();
        }

        private void ChangeSingleQuestionPerTab(bool enabled)
        {
            if (enabled)
                ServiceLocator.SendCommand(new EnableSingleQuestionPerTab(BankIdentifier, SpecificationIdentifier));
            else
                ServiceLocator.SendCommand(new DisableSingleQuestionPerTab(BankIdentifier, SpecificationIdentifier));

            SetScenarioFields();
        }
    }
}