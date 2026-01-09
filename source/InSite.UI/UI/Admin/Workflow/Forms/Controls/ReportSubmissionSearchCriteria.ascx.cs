using System;
using System.Web.UI;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportSubmissionSearchCriteria : SearchCriteriaController<QResponseSessionFilter>
    {
        private Guid? DefaultSurveyId => Guid.TryParse(Request.QueryString["form"], out var guid) ? guid : (Guid?)null;

        public override QResponseSessionFilter Filter
        {
            get
            {
                var filter = new QResponseSessionFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    SurveyFormIdentifier = SurveyID.Value,
                    SurveyQuestionIdentifier = QuestionID.ValueAsGuid,
                    ResponseAnswerText = AnswerText.Value,
                    RespondentName = RespondentName.Text,
                    IsPlatformAdministrator = Identity.IsOperator,
                    StartedSince = StartedSince.Value,
                    StartedBefore = StartedBefore.Value,
                    CompletedSince = CompletedSince.Value,
                    CompletedBefore = CompletedBefore.Value,
                    IsLocked = IsLocked.ValueAsBoolean,
                    GroupIdentifier = FilterGroupIdentifier.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SurveyID.Value = value.SurveyFormIdentifier;
                if (!Page.IsPostBack && DefaultSurveyId.HasValue)
                    SurveyID.Value = DefaultSurveyId.Value;

                OnSurveySelected();

                QuestionID.ValueAsGuid = value.SurveyQuestionIdentifier;

                OnQuestionSelected();

                AnswerText.Value = value.ResponseAnswerText;

                RespondentName.Text = value.RespondentName;
                StartedSince.Value = value.StartedSince;
                StartedBefore.Value = value.StartedBefore;
                CompletedSince.Value = value.CompletedSince;
                CompletedBefore.Value = value.CompletedBefore;
                FilterGroupIdentifier.Value = value.GroupIdentifier;
                IsLocked.ValueAsBoolean = value.IsLocked;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SurveyID.AutoPostBack = true;
            SurveyID.ValueChanged += SurveyID_ValueChanged;

            QuestionID.AutoPostBack = true;
            QuestionID.ValueChanged += QuestionID_ValueChanged;

            FilterGroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LockRespondentCriteria();

            if (Page.IsPostBack)
                return;

            if (DefaultSurveyId.HasValue)
                SurveyID.Value = DefaultSurveyId.Value;

            QuestionID.SurveyIdentifier = SurveyID.Value ?? Guid.Empty;
            QuestionID.RefreshData();
        }

        private void SurveyID_ValueChanged(object sender, EventArgs e) => OnSurveySelected();

        private void QuestionID_ValueChanged(object sender, EventArgs e) => OnQuestionSelected();

        private void OnSurveySelected()
        {
            QuestionID.Value = null;
            QuestionID.SurveyIdentifier = SurveyID.Value ?? Guid.Empty;
            QuestionID.RefreshData();

            OnQuestionSelected();
        }

        private void OnQuestionSelected()
        {
            AnswerText.Value = null;
            AnswerText.SurveyQuestionIdentifier = QuestionID.ValueAsGuid;
            AnswerText.RefreshData();
        }

        public override void Clear()
        {
            SurveyID.Value = DefaultSurveyId;

            QuestionID.SurveyIdentifier = SurveyID.Value ?? Guid.Empty;
            QuestionID.RefreshData();
            QuestionID.Value = null;

            AnswerText.SurveyQuestionIdentifier = null;
            AnswerText.RefreshData();
            AnswerText.Value = null;

            RespondentName.Text = null;
            StartedSince.Value = null;
            StartedBefore.Value = null;
            CompletedSince.Value = null;
            CompletedBefore.Value = null;
            FilterGroupIdentifier.Value = null;

            IsLocked.ClearSelection();
        }

        /// <summary>
        /// If the organization account contains any forms with confidentiality enabled, then administrators are not allowed
        /// to search by respondent.
        /// </summary>
        private void LockRespondentCriteria()
        {
            if (Identity.IsOperator)
                return;

            var filter = new QSurveyFormFilter { OrganizationIdentifier = Organization.Identifier, EnableUserConfidentiality = true };
            var forms = ServiceLocator.SurveySearch.GetSurveyForms(filter);
            if (forms.Count == 0)
                return;

            RespondentName.Text = null;
            RespondentName.Enabled = false;
        }
    }
}