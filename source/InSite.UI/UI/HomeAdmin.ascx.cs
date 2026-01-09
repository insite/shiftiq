using System;
using System.Linq;
using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI
{
    public partial class HomeAdmin : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Page is AdminBasePage admin)
                if (admin.Navigator != null)
                    admin.Navigator.IsCmds = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            AdminDashboardPrototype.Visible = Organization.Toolkits?.Accounts?.DisplayDashboardPrototype == true;

            if (Organization.Toolkits?.Accounts?.DisplayDashboardPrototype == true)
                PageHelper.HideTitle(Page);

            WarningCheck();
        }

        private void WarningCheck()
        {
            var mandatoryClosedSurveys = GetMandatoryClosedSurveys();
            WarningToast.Visible = !string.IsNullOrEmpty(mandatoryClosedSurveys);
            if (!WarningToast.Visible)
                return;

            WarningToast.Title = "Mandatory Closed Form";
            WarningToast.Text = mandatoryClosedSurveys;
        }

        private string GetMandatoryClosedSurveys()
        {
            var sb = new StringBuilder();

            var filter = new QGroupFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                SurveyNecessity = "Required"
            };

            var groups = ServiceLocator.GroupSearch.GetGroups(filter)
                .Where(x => x.SurveyFormIdentifier.HasValue).ToList();

            foreach (var group in groups)
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyForm(group.SurveyFormIdentifier.Value);

                if (survey?.SurveyFormClosed == null)
                    continue;

                if (survey.SurveyFormClosed < DateTimeOffset.UtcNow)
                    sb.Append($"The form <a href='/ui/admin/workflow/forms/outline?form={survey.SurveyFormIdentifier}'>{survey.SurveyFormName}</a> is now closed, but it is mandatory for the group <a href='/ui/admin/contacts/groups/edit?contact={group.GroupIdentifier}'>{group.GroupName}</a>. Group members will be unable to submit their submissions when they sign in.");
            }

            return sb.ToString();
        }
    }
}