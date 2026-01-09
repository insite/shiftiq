using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Specifications.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SpecificationType.AutoPostBack = true;
            SpecificationType.SelectedIndexChanged += SpecificationType_SelectedIndexChanged;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToFinder();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SpecificationType_SelectedIndexChanged(object sender, EventArgs e)
            => SpecificationTypeHelp.InnerText = SpecHelper.GetDescription(SpecificationType.SelectedValue.ToEnum<SpecificationType>());

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        #endregion

        #region Database operations

        private BankState GetBank()
        {
            return ServiceLocator.BankSearch.GetBankState(BankID);
        }

        private void Open()
        {
            var bank = GetBank();
            if (bank == null)
                RedirectToFinder();

            SetInputValues(bank);
        }

        private void Save()
        {
            var commands = new List<Command>();

            var bank = GetBank();
            var spec = new Specification();

            GetInputValues(spec);

            commands.Add(new AddSpecification(BankID, spec.Type, spec.Consequence, spec.Identifier, spec.Name, spec.Asset, spec.FormLimit, spec.QuestionLimit, spec.Calculation));

            if (spec.SectionsAsTabsEnabled)
            {
                commands.Add(new EnableSectionsAsTabs(BankID, spec.Identifier));

                if (!spec.TabNavigationEnabled)
                {
                    commands.Add(new DisableTabNavigation(BankID, spec.Identifier));

                    if (spec.SingleQuestionPerTabEnabled)
                        commands.Add(new EnableSingleQuestionPerTab(BankID, spec.Identifier));
                }
            }

            var formIdentifier = UniqueIdentifier.Create();
            if (spec.FormLimit == 1)
            {
                var asset = Sequence.Increment(bank.Tenant, SequenceType.Asset);
                commands.Add(new AddForm(BankID, spec.Identifier, formIdentifier, spec.Name, asset, 0));
            }

            foreach (RepeaterItem item in QuestionSetRepeater.Items)
            {
                var check = (ICheckBox)item.FindControl("QuestionSetCheckBox");
                if (check.Checked)
                {
                    var setIdentifier = Guid.Parse(check.Value);
                    var sieveIdentifier = UniqueIdentifier.Create();
                    var set = bank.FindSet(setIdentifier);

                    commands.Add(new AddCriterion(BankID, spec.Identifier, new[] { setIdentifier }, sieveIdentifier, null, 1, set.Questions.Count, null, null));

                    if (spec.FormLimit == 1)
                        commands.Add(new AddSection(BankID, formIdentifier, UniqueIdentifier.Create(), sieveIdentifier));
                }
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            RedirectToBankReader(spec.Identifier);
        }

        #endregion

        #region Settin/getting input values

        private void SetInputValues(BankState bank)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{bank} <span class='form-text'>Bank Asset #{bank.Asset}</span>");

            SpecificationType.SelectedValue = Shift.Constant.SpecificationType.Static.GetName();
            SpecificationTypeHelp.InnerText = SpecHelper.GetDescription(Shift.Constant.SpecificationType.Static);

            ConfigurationDetails.SetDefaultInputValues();
            CalculationDetails.SetDefaultInputValues();
            QuestionSetRepeater.DataSource = bank.Sets;
            QuestionSetRepeater.DataBind();
            CancelButton.NavigateUrl = GetReaderUrl();
        }

        private void GetInputValues(Specification spec)
        {
            spec.Identifier = UniqueIdentifier.Create();
            spec.Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            spec.Type = SpecificationType.SelectedValue.ToEnum<SpecificationType>();
            spec.Name = SpecificationName.Text;
            spec.SectionsAsTabsEnabled = ConfigurationDetails.GetScenarioEnabled() == true;
            spec.TabNavigationEnabled = ConfigurationDetails.GetTabNavigationEnabled() == true;
            spec.SingleQuestionPerTabEnabled = ConfigurationDetails.GetSingleQuestionPerTabEnabled() == true;
            spec.TabTimeLimit = ConfigurationDetails.GetTabTimeLimit() ?? SpecificationTabTimeLimit.Disabled;

            ConfigurationDetails.GetInputValues(spec);
            CalculationDetails.GetInputValues(spec.Calculation);
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToBankReader(Guid? specificationId = null)
        {
            var url = GetReaderUrl(specificationId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? specificationId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            url += specificationId.HasValue ? $"&spec={specificationId.Value}" : "&panel=specifications";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}