using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Attempts.Reports.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QAttemptFilter>
    {
        protected override bool EnableSearchValidation => true;

        public override QAttemptFilter Filter
        {
            get
            {
                var organizationPersonTypes = OrganizationRole.ValuesArray.Length > 0
                    ? OrganizationRole.ValuesArray
                    : OrganizationRole.FlattenOptions().Select(x => x.Value).ToArray();

                var filter = new AttemptReportFilter
                {
                    FormOrganizationIdentifier = Identity.Organization.Identifier,
                    CandidateOrganizationIdentifiers = new[] { Identity.Organization.Identifier },
                    FrameworkIdentifier = ExamBankFrameworkID.Value,
                    BankIdentifier = !ExamBankID.HasValue && !CandidateName.Text.HasValue()
                        ? Guid.Empty
                        : ExamBankID.Value,
                    FormIdentifier = ExamFormID.Value,
                    LearnerUserIdentifier = ExamCandidateID.Value,
                    BankLevel = ExamBankLevel.Value,
                    LearnerName = CandidateName.Text,
                    AssessorName = AssessorName.Text,
                    EventIdentifier = ExamEventID.Value,
                    EventFormat = EventFormat.Value,
                    IsCompleted = AttemptCompletionStatus.ValueAsBoolean,
                    AttemptTag = AttemptTag.ValuesArray,
                    AttemptTagStatus = AttemptTagStatus.Value,
                    AttemptStartedSince = AttemptStartedSince.Value,
                    AttemptStartedBefore = AttemptStartedBefore.Value,
                    AttemptScoreFrom = AttemptScoreMinimum.ValueAsInt,
                    AttemptScoreThru = AttemptScoreMaximum.ValueAsInt,
                    AttemptGradedSince = AttemptGradedSince.Value,
                    AttemptGradedBefore = AttemptGradedBefore.Value,
                    PilotAttemptsInclusion = PilotAttemptsInclusion.Value.ToEnum(InclusionType.Include),
                    CandidateType = CandidateType.ValuesArray,
                    OrganizationPersonTypes = organizationPersonTypes,
                    HideLearnerName = HideLearnerName.Checked,
                    RubricIdentifier = Rubric.Value,
                    IsWithoutGradingAssessor = ShowEmptyGradingAssessor.Checked,
                    GradingAssessorIdentifier = GradingAssessor.Value,
                    IncludePendingAttempts = IncludePendingAttempts.Checked,
                };

                GetCheckedShowColumns(filter);

                if (HideLearnerName.Checked)
                    filter.ShowColumns.Remove("EXAM CANDIDATE");

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                ExamEventID.Value = value.EventIdentifier;
                EventFormat.Value = value.EventFormat;

                ExamBankFrameworkID.Value = value.FrameworkIdentifier;
                OnExamBankFrameworkSelected();

                ExamBankID.Value = value.BankIdentifier.HasValue && value.BankIdentifier == Guid.Empty ? null : value.BankIdentifier;
                OnExamBankSelected();

                ExamFormID.Value = value.FormIdentifier;
                OnExamFormSelected();

                ExamBankLevel.Value = value.BankLevel;

                CandidateName.Text = value.LearnerName;
                AssessorName.Text = value.AssessorName;
                ExamCandidateID.Value = value.LearnerUserIdentifier;

                AttemptCompletionStatus.ValueAsBoolean = value.IsCompleted;
                AttemptTag.Values = value.AttemptTag;
                AttemptTagStatus.Value = value.AttemptTagStatus;
                AttemptStartedSince.Value = value.AttemptStartedSince;
                AttemptStartedBefore.Value = value.AttemptStartedBefore;
                AttemptGradedSince.Value = value.AttemptGradedSince;
                AttemptGradedBefore.Value = value.AttemptGradedBefore;
                AttemptScoreMinimum.ValueAsInt = value.AttemptScoreFrom;
                AttemptScoreMaximum.ValueAsInt = value.AttemptScoreThru;

                PilotAttemptsInclusion.Value = value.PilotAttemptsInclusion.GetName(InclusionType.Include);
                Rubric.Value = value.RubricIdentifier;
                CandidateType.Values = value.CandidateType;
                OrganizationRole.Values = value.OrganizationPersonTypes;
                HideLearnerName.Checked = value.HideLearnerName;
                ShowEmptyGradingAssessor.Checked = value.IsWithoutGradingAssessor.HasValue ? value.IsWithoutGradingAssessor.Value : false;
                GradingAssessor.Value = value.GradingAssessorIdentifier;

                SortColumns.Value = value.OrderBy;

                IncludePendingAttempts.Checked = value is AttemptReportFilter reportFilter
                    && reportFilter.IncludePendingAttempts;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ExamBankFrameworkID.AutoPostBack = true;
            ExamBankFrameworkID.ValueChanged += ExamBankFrameworkID_ValueChanged;

            ExamBankID.AutoPostBack = true;
            ExamBankID.ValueChanged += ExamBankID_ValueChanged;

            ExamFormID.AutoPostBack = true;
            ExamFormID.ValueChanged += ExamFormID_ValueChanged;

            CustomValidator.ServerValidate += CustomValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCheckAll(OrganizationRole, "Organization Role");

            if (IsPostBack)
                return;

            var activeOrganization = new[] { Organization.Identifier };
            var adminOrganizations = Identity.Organizations.Select(x => x.OrganizationIdentifier).ToArray();

            ExamBankID.Filter.OrganizationIdentifiers = activeOrganization;
            ExamBankID.Filter.AttemptCandidateOrganizationIdentifiers = adminOrganizations;
            ExamBankID.Value = null;

            ExamFormID.Filter.OrganizationIdentifiers = activeOrganization;
            ExamFormID.Filter.AttemptCandidateOrganizationIdentifiers = adminOrganizations;
            ExamFormID.Value = null;

            ExamCandidateID.Filter.MustHaveAttempts = true;
            ExamCandidateID.Filter.OrganizationIdentifiers = adminOrganizations;
            ExamCandidateID.Filter.AttemptFormOrganizationIdentifiers = activeOrganization;
            ExamCandidateID.Value = null;

            var showGradinssessor = Identity.Organization.Toolkits.Assessments?.AttemptGradingAssessor ?? false;
            GradingAssessor.EmptyOnLoad = true;

            if (showGradinssessor)
            {
                GradingAssessorCriteria.Visible = true;

                var groupIds = TGroupPermissionSearch.SelectGroupFromActionPermission("Design/GradingAssessors");
                if (groupIds != null && groupIds.Length > 0)
                {
                    GradingAssessor.Filter.Groups = groupIds;
                    GradingAssessor.Filter.OrganizationIdentifier = Identity.Organization.OrganizationIdentifier;
                    GradingAssessor.EmptyOnLoad = false;
                }
            }
        }

        private void ExamBankFrameworkID_ValueChanged(object sender, EventArgs e) => OnExamBankFrameworkSelected();

        private void OnExamBankFrameworkSelected()
        {
            var frameworkId = ExamBankFrameworkID.Value;

            ExamBankID.Filter.FrameworkIdentifiers = frameworkId.HasValue ? new Guid[] { frameworkId.Value } : null;
            ExamBankID.Value = null;

            OnExamBankSelected();

            ExamFormID.Filter.FrameworkIdentifier = frameworkId;
            ExamFormID.Value = null;
        }

        protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ExamBankID.HasValue || CandidateName.Text.HasValue();
        }

        private void ExamBankID_ValueChanged(object sender, EventArgs e) => OnExamBankSelected();

        private void OnExamBankSelected()
        {
            var hasValue = ExamBankID.Value.HasValue;

            BankRelatedFields.Visible = hasValue;
            ExamFormID.Value = null;
            ExamCandidateID.Value = null;

            if (!hasValue)
                return;

            ExamFormID.Filter.BankIdentifier = ExamBankID.Value;
            ExamFormID.Value = null;

            OnExamFormSelected();

            ExamCandidateID.Filter.AttemptBankIdentifier = ExamBankID.Value;
            ExamCandidateID.Value = null;
        }

        private void ExamFormID_ValueChanged(object sender, FindEntityValueChangedEventArgs e) => OnExamFormSelected();

        private void OnExamFormSelected()
        {
            ExamCandidateID.Filter.AttemptFormIdentifier = ExamFormID.Value;
            ExamCandidateID.Value = null;
        }

        public override void Clear()
        {
            ExamBankFrameworkID.Value = null;
            OnExamBankFrameworkSelected();

            ExamBankID.Value = null;
            OnExamBankSelected();

            ExamFormID.Value = null;
            OnExamFormSelected();

            ExamBankLevel.Value = null;

            CandidateName.Text = null;
            AssessorName.Text = null;

            ExamEventID.Value = null;
            EventFormat.Value = null;

            AttemptTag.ClearSelection();

            AttemptStartedSince.Value = null;
            AttemptStartedBefore.Value = null;
            AttemptCompletionStatus.ClearSelection();
            AttemptGradedSince.Value = null;
            AttemptGradedBefore.Value = null;
            AttemptScoreMinimum.ValueAsInt = null;
            AttemptScoreMaximum.ValueAsInt = null;

            Rubric.Value = null;

            PilotAttemptsInclusion.ClearSelection();

            CandidateType.ClearSelection();

            OrganizationRole.Values = new[] { "Learner" };

            AttemptTagStatus.Value = null;

            GradingAssessor.Value = null;

            HideLearnerName.Checked = false;
            ShowEmptyGradingAssessor.Checked = false;

            IncludePendingAttempts.Checked = false;
        }
    }
}