using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Jobs.Employers.Candidates
{
    public partial class View : PortalBasePage
    {
        protected Guid? UserId => Guid.TryParse(Page.Request.QueryString["id"], out var id) ? id : (Guid?)null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!UserId.HasValue)
                HttpResponseHelper.Redirect("/ui/portal/job/employers/candidates/search", true);

            BindModelToControls(UserId.Value);
        }

        private void BindModelToControls(Guid userId)
        {
            var person = PersonSearch.Select(Organization.Identifier, userId, x => x.User, x => x.HomeAddress);

            JobInterest.BindModelToControls(person);
            LanguageAbility.BindModelToControls(person);

            Experience.BindModelToControls(person);
            Education.BindModelToControls(person);
            Document.BindModelToControls(person);
            Achievement.BindModelToControls(person);

            PageHelper.AutoBindHeader(Page, null, person.User.FullName);

            RequestContactLink.Attributes["OnClick"] = "candidateRequestContact.showRequestContact('" + UserId.ToString() + "'); return false;";
            RequestContactLink.Visible = true;
        }

    }
}