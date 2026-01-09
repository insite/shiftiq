using System;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Workflow.Forms.Submissions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QResponseSessionFilter>
    {
        private Guid? DefaultSurveyThumbprint => Guid.TryParse(Request.QueryString["form"], out var guid) ? guid : (Guid?)null;

        public override QResponseSessionFilter Filter
        {
            get
            {
                var filter = new QResponseSessionFilter
                {
                    CompletedBefore = CompletedBefore.Value,
                    CompletedSince = CompletedSince.Value,
                    AgencyGroupIdentifier = AgencyGroupIdentifier.Value,
                    IsLocked = IsLocked.ValueAsBoolean,
                    IsPlatformAdministrator = Identity.IsOperator,
                    OrganizationIdentifier = Organization.Identifier,
                    PeriodIdentifiers = PeriodIdentifiers.Values,
                    RespondentName = RespondentName.Text,
                    ResponseAnswerText = AnswerText.Value,
                    StartedBefore = StartedBefore.Value,
                    StartedSince = StartedSince.Value,
                    SurveyFormIdentifier = SurveyID.Value,
                    SurveyQuestionIdentifier = QuestionID.ValueAsGuid,
                };

                GetCheckedShowColumns(filter);

                AgencyGroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                SurveyID.Value = value.SurveyFormIdentifier;
                if (!Page.IsPostBack && DefaultSurveyThumbprint.HasValue)
                    SurveyID.Value = DefaultSurveyThumbprint.Value;

                OnSurveySelected();

                QuestionID.ValueAsGuid = value.SurveyQuestionIdentifier;

                OnQuestionSelected();

                AnswerText.Value = value.ResponseAnswerText;
                AgencyGroupIdentifier.Value = value.AgencyGroupIdentifier;
                PeriodIdentifiers.Values = value.PeriodIdentifiers;
                RespondentName.Text = value.RespondentName;
                StartedSince.Value = value.StartedSince;
                StartedBefore.Value = value.StartedBefore;
                CompletedSince.Value = value.CompletedSince;
                CompletedBefore.Value = value.CompletedBefore;

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
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LockRespondentCriteria();

            if (Page.IsPostBack)
                return;

            if (DefaultSurveyThumbprint.HasValue)
                SurveyID.Value = DefaultSurveyThumbprint.Value;

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
            SurveyID.Value = DefaultSurveyThumbprint;

            QuestionID.SurveyIdentifier = SurveyID.Value ?? Guid.Empty;
            QuestionID.RefreshData();
            QuestionID.Value = null;

            AnswerText.SurveyQuestionIdentifier = null;
            AnswerText.RefreshData();
            AnswerText.Value = null;

            PeriodIdentifiers.Value = null;
            AgencyGroupIdentifier.Value = null;
            RespondentName.Text = null;
            StartedSince.Value = null;
            StartedBefore.Value = null;
            CompletedSince.Value = null;
            CompletedBefore.Value = null;

            IsLocked.ClearSelection();
        }

        /// <summary>
        /// If the organization account contains any forms with confidentiality enabled, then administrators are not allowed
        /// to search by respondent.
        /// </summary>
        private void LockRespondentCriteria()
        {
            if (Identity.IsOperator || !Organization.Toolkits.Surveys.EnableUserConfidentiality)
                return;

            RespondentName.Enabled = false;
            RespondentName.Text = null;
        }
    }
}