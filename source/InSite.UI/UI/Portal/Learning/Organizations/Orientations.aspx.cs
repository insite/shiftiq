using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Helpers;
using InSite.Web.Security;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Learning.Organizations
{
    public partial class Orientations : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CardRepeater.ItemDataBound += CardRepeater_ItemDataBound;
        }

        private void CardRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (StartOrientationItem)e.Item.DataItem;

            if (item.OrganizationLogo != null)
            {
                var image = $"<img class='card-img-top' src='{item.OrganizationLogo}' alt=''>";
                var cardImage = (Literal)e.Item.FindControl("CardImage");
                cardImage.Text = image;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            ValidateUrl();

            if (!IsPostBack)
                BindModelToControls(InitModel());
        }

        private void ValidateUrl()
        {
            if (!Guid.TryParse(Request.QueryString["organization"], out Guid organizationId))
                return;

            var organization = OrganizationSearch.Select(organizationId);

            var user = User.UserIdentifier;

            PersonStore.Insert(PersonFactory.Create(user, organizationId, null, false, null));

            var group = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter { GroupName = "Skills Passport Users", OrganizationIdentifier = OrganizationIdentifiers.CMDS })
                .FirstOrDefault();

            if (MembershipPermissionHelper.CanModifyMembership(group.GroupIdentifier))
            {
                var m = MembershipSearch.SelectFirst(x => x.User.UserIdentifier == user && x.Group.GroupIdentifier == group.GroupIdentifier);
                if (m == null)
                {
                    MembershipStore.Save(MembershipFactory.Create(User.UserIdentifier, group.GroupIdentifier, organizationId, null, DateTimeOffset.UtcNow));
                    CurrentSessionState.Identity.Groups.Add(GroupAdapter.CreateGroupPacket(group));
                }
            }

            var url = "/ui/portal/home";
            HttpResponseHelper.Redirect(PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organization.OrganizationCode, url));
        }

        private List<StartOrientationItem> InitModel()
        {
            var model = new List<StartOrientationItem>();

            var organizations = OrganizationSearch.SelectAllWithOrientations();
            foreach (var organization in organizations)
            {
                var item = new StartOrientationItem
                {
                    OrganizationName = organization.CompanyName,
                    OrganizationDescription = organization.CompanySummary,
                    OrganizationLogo = organization.OrganizationLogoUrl,
                    OrganizationUrl = $"/ui/portal/learning/organizations-orientations?organization={organization.OrganizationIdentifier}"
                };
                model.Add(item);
            }

            return model;
        }

        private void BindModelToControls(List<StartOrientationItem> model)
        {
            PageHelper.AutoBindHeader(this);

            CardRepeater.DataSource = model;
            CardRepeater.DataBind();
        }
    }
}