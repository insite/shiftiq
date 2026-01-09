using System;
using System.Web;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Logs.Aggregates
{
    public partial class Outline : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Properties

        private Guid AggregateID => Guid.TryParse(Request.QueryString["aggregate"], out var value) ? value : Guid.Empty;
        private string ReturnUrl => !string.IsNullOrEmpty(Request.QueryString["returnURL"]) ? Request.QueryString["returnURL"] : "/";

        #endregion

        #region Initialization and Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                PageHelper.AutoBindHeader(Page);
            }
        }


        #endregion

        #region Database operations

        private void Open()
        {
            var aggregate = ServiceLocator.AggregateSearch.Get(AggregateID);

            if (aggregate == null)
            {
                PageHelper.AutoBindHeader(this);
                ViewerStatus.AddMessage(AlertType.Error, $"Aggregate Not Found: {AggregateID}");
                return;
            }

            if (!SetFormTitle())
            {
                PageHelper.AutoBindHeader(this);
                return;
            }

            AggregateIdentifier.Text = aggregate.AggregateIdentifier.ToString();
            AggregateType.Text = aggregate.AggregateType;
            AggregateClass.Text = aggregate.AggregateClass;
            AggregateExpires.Text = aggregate.AggregateExpires != null ? aggregate.AggregateExpires.Format(User.TimeZone, true, true) : "N/A";
            OrganizationIdentifier.Text = aggregate.OriginOrganization.ToString();

            ChangeGrid.LoadData(AggregateID);

            CancelLink.NavigateUrl = ReturnUrl;
        }

        #endregion

        #region Helper methods

        private bool SetFormTitle()
        {
            var parentAction = GetParentAction()?.ToLower();
            var organizationId = CurrentSessionState.Identity.Organization.OrganizationIdentifier;
            string title = string.Empty;

            switch (parentAction)
            {
                case "ui/admin/assessments/banks/outline":
                    var query = ServiceLocator.BankSearch.GetBank(AggregateID);
                    var bank = ServiceLocator.BankSearch.GetBankState(AggregateID);

                    if (bank == null || bank.Tenant != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/assessments/banks/search");
                        return false;
                    }

                    title = (query.BankTitle ?? query.BankName) + $" <span class='form-text'>Asset #{query.AssetNumber}</span>";

                    break;

                case "ui/admin/events/exams/outline":
                    var @event = ServiceLocator.EventSearch.GetEvent(AggregateID);

                    if (@event == null || @event.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/events/exams/search");
                        return false;
                    }

                    title = @event.EventTitle;

                    break;

                case "ui/admin/registrations/exams/edit":
                    var reg = ServiceLocator.RegistrationSearch.GetRegistration(AggregateID, x => x.Event, x => x.Candidate);

                    if (reg == null || reg.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect($"/ui/admin/events/registrations/exams/search");
                        return false;
                    }

                    title = reg.Event.EventTitle + ": " + reg.Candidate.UserFullName;

                    break;

                case "ui/admin/workflow/cases/outline":
                    var issue = ServiceLocator.IssueSearch.GetIssue(AggregateID);

                    if (issue == null || issue.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/search");
                        return false;
                    }

                    title = $"{issue.IssueTitle} <span class='form-text'>{issue.IssueType} Case #{issue.IssueNumber}</span>";

                    break;

                case "ui/admin/events/classes/outline":
                    var classEvent = ServiceLocator.EventSearch.GetEvent(AggregateID);

                    if (classEvent == null || classEvent.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect($"/ui/admin/events/classes/search");
                        return false;
                    }

                    title = classEvent.EventTitle;

                    break;

                case "ui/admin/registrations/classes/edit":
                    var registration = ServiceLocator.RegistrationSearch.GetRegistration(AggregateID, x => x.Event, x => x.Candidate);

                    if (registration == null || registration.Event.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/registrations/classes/search");
                        return false;
                    }

                    title = $"{registration.Event.EventTitle} <span class='form-text'>for </span> {registration.Candidate.UserFullName}";

                    break;

                case "ui/admin/records/gradebooks/outline":
                    var queryGradebook = ServiceLocator.RecordSearch.GetGradebook(AggregateID);

                    if (queryGradebook == null || queryGradebook.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
                        return false;
                    }

                    title = queryGradebook.GradebookTitle;

                    break;

                case "ui/admin/records/logbooks/outline":
                    var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(AggregateID);

                    if (journalSetup == null || journalSetup.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search");
                        return false;
                    }

                    title = journalSetup.JournalSetupName;

                    break;

                case "ui/admin/records/achievements/outline":
                    var achievement = ServiceLocator.AchievementSearch.GetAchievement(AggregateID);

                    if (achievement == null || achievement.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/records/achievements/search");
                        return false;
                    }

                    title = achievement.AchievementTitle;

                    break;

                case "ui/admin/records/credentials/outline":
                    var credential = ServiceLocator.AchievementSearch.GetCredential(AggregateID);

                    if (credential == null || credential.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/records/credentials/search");
                        return false;
                    }

                    title = $"{credential.AchievementTitle} <span class='form-text'>for</span> {credential.UserFullName}";

                    break;

                case "ui/admin/messages/outline":
                    var message = ServiceLocator.MessageSearch.GetMessage(AggregateID);

                    if (message == null || message.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/messages/messages/search");
                        return false;
                    }

                    title = $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>";

                    break;

                case "ui/admin/sales/invoices/outline":
                    var invoice = ServiceLocator.InvoiceSearch.GetInvoice(AggregateID);

                    if (invoice == null || invoice.OrganizationIdentifier != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/sales/invoices/search");
                        return false;
                    }

                    title = $"Invoice to {invoice.CustomerFullName}";

                    break;

                case "ui/admin/workflow/forms/outline":
                    var survey = ServiceLocator.SurveySearch.GetSurveyState(AggregateID);

                    if (survey == null || survey.Form.Tenant != organizationId)
                    {
                        HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search");
                        return false;
                    }

                    title = $"{survey.Form.Name} <span class='form-text'>Form #{survey.Form.Asset}</span>";

                    break;

                case "ui/admin/contacts/groups/edit":
                    var group = ServiceLocator.GroupSearch.GetGroup(AggregateID);
                    var membership = ServiceLocator.MembershipSearch.Select(AggregateID);

                    if (group == null && membership == null)
                        RedirectBack();

                    if (group != null)
                    {
                        if (group.OrganizationIdentifier == organizationId)
                            title = group.GroupName;
                        else
                            RedirectBack();
                    }
                    else if (membership != null)
                        title = "Membership Aggregate";
                    else
                        RedirectBack();

                    void RedirectBack()
                       => HttpResponseHelper.Redirect("/ui/admin/contacts/groups/search");

                    break;

                default:
                    title = $"Aggregate";
                    break;
            }

            PageHelper.AutoBindHeader(this, qualifier: title);

            return true;
        }

        #endregion

        #region IOverridesParent, IHasParentLinkParameters

        public new IWebRoute GetParent()
        {
            var parentAction = GetParentAction();

            return !string.IsNullOrEmpty(parentAction)
                ? WebRoute.GetWebRoute(parentAction)
                : null;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent != null && string.Equals(parent.Name, GetParentAction(), StringComparison.OrdinalIgnoreCase)
                ? GetParentActionParameters()
                : null;
        }

        private string GetParentAction()
        {
            if (ReturnUrl == "/" || !ReturnUrl.StartsWith("/"))
                return null;

            var index = ReturnUrl.IndexOf('?');

            return index > 0
                ? ReturnUrl.Substring(1, index - 1)
                : ReturnUrl.Substring(1);
        }

        private string GetParentActionParameters()
        {
            if (ReturnUrl == "/" || !ReturnUrl.StartsWith("/"))
                return null;

            var index = ReturnUrl.IndexOf('?');

            return index > 0 && index < ReturnUrl.Length
                ? ReturnUrl.Substring(index + 1)
                : null;
        }

        #endregion

        public static string GetUrl(Guid aggregate, string returnURL)
            => $"/ui/admin/logs/aggregates/outline?aggregate={aggregate}&returnURL=" + HttpUtility.UrlEncode(returnURL);
    }
}