using System;

using InSite.Application.Issues.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Contacts.Referral
{
    public partial class PersonOutline : PortalBasePage, IHasTitle
    {
        protected PersonOutlineModel Model { get; set; }

        private Guid LearnerIdentifier => Guid.TryParse(Request.QueryString["learner"], out Guid id) ? id : User.Identifier;

        public string GetTitle()
            => Model.FullName;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Model = CreateModel();

            if (Model == null)
                HttpResponseHelper.Redirect("/ui/portal/contacts/referral/search");

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CaseColumn.Visible = Model.HasCase;

            DocumentList.BindFiles(LearnerIdentifier);
        }

        private PersonOutlineModel CreateModel()
        {
            var learner = PersonSearch.Select(Organization.Identifier, LearnerIdentifier, x => x.User, x => x.OccupationStandard);

            if (learner == null)
                return null;

            var caseFilter = new QIssueFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                TopicUserIdentifier = LearnerIdentifier,
            };

            var cases = ServiceLocator.IssueSearch.GetIssues(caseFilter);
            var @case = cases.Count == 1 ? cases[0] : null;

            return new PersonOutlineModel
            {
                FullName = learner.User.FullName,
                Email = learner.User.Email,
                AccountCode = learner.PersonCode ?? "-",
                Phone = learner.Phone ?? "-",
                OccupationTitle = learner.OccupationStandard?.ContentTitle ?? "-",
                HasCase = @case != null,
                CaseType = @case?.IssueType,
                CaseStatus = @case?.IssueStatusName,
            };
        }
    }
}
