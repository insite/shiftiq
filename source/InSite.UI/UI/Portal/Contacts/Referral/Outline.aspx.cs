using System;

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

        private PersonOutlineModel CreateModel()
        {
            var learner = PersonSearch.Select(Organization.Identifier, LearnerIdentifier, x => x.User, x => x.OccupationStandard);

            if (learner == null)
                return null;

            return new PersonOutlineModel
            {
                FullName = learner.User.FullName,
                Email = learner.User.Email,
                AccountCode = learner.PersonCode ?? "-",
                Phone = learner.Phone ?? "-",
                OccupationTitle = learner.OccupationStandard?.ContentTitle ?? "-"
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Model = CreateModel();

            if (Model == null)
                HttpResponseHelper.Redirect("/ui/portal/contacts/referral/search");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            DocumentList.BindFiles(LearnerIdentifier);
        }
    }
}
