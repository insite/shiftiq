using System;
using System.Linq;

using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class AdHocCriteria : SearchCriteriaController<QAttemptFilter>
    {
        #region SearchCriteriaController

        protected override bool EnableSearchValidation => true;

        public override QAttemptFilter Filter
        {
            get
            {
                var filter = new AdHocAttemptFilter
                {
                    CandidateOrganizationIdentifiers = Identity.Organizations.Select(x => x.OrganizationIdentifier).ToArray(),
                    IsCompleted = true,

                    OccupationIdentifier = ExamBankOccupationID.Value,
                    FrameworkIdentifier = ExamBankFrameworkID.Value,
                    BankIdentifier = ExamBankID.Value,
                    FormIdentifier = FormIdentifier.Value,
                    LearnerUserIdentifier = ExamCandidateID.Value,
                    EventIdentifier = ExamEventID.Value,
                    EventFormat = EventFormat.Value,
                    AttemptTag = AttemptTag.ValuesArray,
                    AttemptStartedSince = AttemptStartedSince.Value,
                    AttemptStartedBefore = AttemptStartedBefore.Value,
                    AttemptGradedSince = AttemptCompletedSince.Value,
                    AttemptGradedBefore = AttemptCompletedBefore.Value,
                    IncludeOnlyFirstAttempt = IncludeOnlyFirstAttempt.ValueAsBoolean.Value,
                    PilotAttemptsInclusion = PilotAttemptsInclusion.Value.ToEnum(InclusionType.Include),
                    CandidateType = CandidateType.ValuesArray
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                ExamCandidateID.Value = value.LearnerUserIdentifier;
                ExamEventID.Value = value.EventIdentifier;
                EventFormat.Value = value.EventFormat;

                ExamBankOccupationID.Value = value.OccupationIdentifier;
                OnExamBankOccupationSelected();

                ExamBankFrameworkID.Value = value.FrameworkIdentifier;
                OnExamBankFrameworkSelected();

                ExamBankID.Value = value.BankIdentifier;
                OnExamBankSelected();

                FormIdentifier.Value = value.FormIdentifier;

                AttemptTag.Values = value.AttemptTag;
                AttemptStartedSince.Value = value.AttemptStartedSince;
                AttemptStartedBefore.Value = value.AttemptStartedBefore;
                AttemptCompletedSince.Value = value.AttemptGradedSince;
                AttemptCompletedBefore.Value = value.AttemptGradedBefore;

                if (value is AdHocAttemptFilter internalFilter)
                    IncludeOnlyFirstAttempt.ValueAsBoolean = internalFilter.IncludeOnlyFirstAttempt;

                PilotAttemptsInclusion.Value = value.PilotAttemptsInclusion.GetName(InclusionType.Include);
                CandidateType.Values = value.CandidateType;

                SortColumns.Value = value.OrderBy;
            }
        }

        public override void Clear()
        {
            ExamBankOccupationID.Value = null;
            OnExamBankOccupationSelected();

            ExamCandidateID.Value = null;
            ExamEventID.Value = null;
            EventFormat.Value = null;

            AttemptTag.ClearSelection();
            AttemptStartedSince.Value = null;
            AttemptStartedBefore.Value = null;
            AttemptCompletedSince.Value = null;
            AttemptCompletedBefore.Value = null;

            IncludeOnlyFirstAttempt.ClearSelection();
            PilotAttemptsInclusion.ClearSelection();

            CandidateType.ClearSelection();
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ExamBankOccupationID.AutoPostBack = true;
            ExamBankOccupationID.ValueChanged += ExamBankOccupationID_ValueChanged;

            ExamBankFrameworkID.AutoPostBack = true;
            ExamBankFrameworkID.ValueChanged += ExamBankFrameworkID_ValueChanged;

            ExamBankID.AutoPostBack = true;
            ExamBankID.ValueChanged += ExamBankID_ValueChanged;
        }

        protected override void OnPreRender(EventArgs e)
        {
            AttemptTag.Enabled = AttemptTag.Items.Count > 0;

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void ExamBankOccupationID_ValueChanged(object sender, EventArgs e) => OnExamBankOccupationSelected();

        private void ExamBankFrameworkID_ValueChanged(object sender, EventArgs e) => OnExamBankFrameworkSelected();

        private void ExamBankID_ValueChanged(object sender, EventArgs e) => OnExamBankSelected();

        private void OnExamBankOccupationSelected()
        {
            var occupationId = ExamBankOccupationID.Value;

            ExamBankFrameworkID.OccupationID = occupationId;
            ExamBankFrameworkID.Value = null;

            ExamBankID.Filter.OccupationIdentifier = occupationId;
            FormIdentifier.Filter.OccupationIdentifier = occupationId;

            OnExamBankFrameworkSelected();
        }

        private void OnExamBankFrameworkSelected()
        {
            var frameworkId = ExamBankFrameworkID.Value;

            ExamBankID.Filter.FrameworkIdentifiers = frameworkId.HasValue ? new Guid[] { frameworkId.Value } : null;
            ExamBankID.Value = null;

            FormIdentifier.Filter.FrameworkIdentifier = frameworkId;

            OnExamBankSelected();
        }

        private void OnExamBankSelected()
        {
            FormIdentifier.Filter.BankIdentifier = ExamBankID.Value;
            FormIdentifier.Value = null;
        }

        #endregion
    }
}