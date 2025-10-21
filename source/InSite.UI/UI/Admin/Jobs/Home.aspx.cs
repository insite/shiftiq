using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Jobs
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var cmds = CurrentSessionState.Identity.IsInRole(CmdsRole.Programmers);
            var newCandidateUrl = new ReturnUrl("/ui/admin/jobs/candidates/search").GetRedirectUrl("/ui/admin/contacts/people/create?candidate=1");
            var newEmployerUrl = new ReturnUrl("/ui/admin/jobs/employers/search").GetRedirectUrl($"/ui/admin/contacts/groups/create?type={GroupTypes.Employer}");

            PageHelper.BindHomeHeader(Page,
                new BreadcrumbItem[] { new BreadcrumbItem("Jobs", null, null, "active") },
                new BreadcrumbItem[] {
                    new BreadcrumbItem("Candidate", newCandidateUrl),
                    new BreadcrumbItem("Employer", newEmployerUrl),
                    new BreadcrumbItem("Job listing", "/ui/admin/jobs/opportunities/create"),
                    new BreadcrumbItem("Job application", "/ui/admin/jobs/applications/create")
                }
            );

            var candidateProfilesCount = PersonCriteria.Count(new PersonFilter { OrganizationIdentifier = Organization.Identifier, IsCandidate = true });
            LoadCounter(CandidateProfilesCounter, CandidateProfilesCount, true, candidateProfilesCount, CandidateProfilesLink, "/ui/admin/jobs/candidates/search");

            var employerProfilesCount = VGroupEmployerSearch.Count(new VGroupEmployerFilter() { OrganizationIdentifier = Organization.Identifier });
            LoadCounter(EmployerProfilesCounter, EmployerProfilesCount, true || cmds, employerProfilesCount, EmployerProfilesLink, "/ui/admin/jobs/employers/search");

            var jobListingsCount = TOpportunitySearch.Count(new TOpportunityFilter() { OrganizationIdentifier = Organization.Identifier });
            LoadCounter(JobListingsCounter, JobListingsCount, true, jobListingsCount, JobListingsLink, "/ui/admin/jobs/opportunities/search");

            var jobApplicationsCount = TApplicationSearch.Count(new TApplicationFilter() { OrganizationIdentifier = Organization.Identifier });
            LoadCounter(JobApplicationsCounter, JobApplicationsCount, true, jobApplicationsCount, JobApplicationsLink, "/ui/admin/jobs/applications/search");
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, bool visible, int count, HtmlAnchor link, string action)
        {
            card.Visible = visible;
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }
    }
}