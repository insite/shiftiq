﻿using System;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Surveys.Responses
{
    public partial class DeleteSessions : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyIdentifier => Guid.TryParse(Request.QueryString["survey"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_OnClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            if (survey.Form.Tenant != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/admin/surveys/search", true);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Survey Form #{survey.Form.Asset}</span>");

            SurveyDetail.BindSurvey(survey, User.TimeZone);

            var responseCount = ServiceLocator.SurveySearch.CountResponseSessions(new QResponseSessionFilter { SurveyFormIdentifier = SurveyIdentifier });
            ResponseSessionCount.Text = responseCount > 0 ? $"{responseCount}" : "0";

            if (responseCount == 0)
                EditorStatus.AddMessage(AlertType.Warning, "There are no response sessions to delete.");

            DeleteButton.Visible = responseCount > 0;

            CancelButton.NavigateUrl = GetReturnUrlInternal();
        }

        private void DeleteButton_OnClick(object sender, EventArgs e)
        {
            var responses = ServiceLocator.SurveySearch.GetResponseSessions(new QResponseSessionFilter { SurveyFormIdentifier = SurveyIdentifier });

            foreach (var response in responses)
                ServiceLocator.SendCommand(new DeleteResponseSession(response.ResponseSessionIdentifier));

            RedirectBack();
        }

        private void RedirectBack()
        {
            var url = GetReturnUrlInternal();
            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReturnUrlInternal()
        {
            return GetReturnUrl()
                ?? $"/ui/admin/surveys/forms/outline?survey={SurveyIdentifier}";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"survey={SurveyIdentifier}" : null;
        }
    }
}