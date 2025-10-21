using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.UI.Portal.Home
{
    public partial class Contacts : PortalBasePage
    {
        private int _counter;

        protected string RowCssClass => ++_counter % 2 == 0 ? "alt1" : "alt2";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubordinateRepeater.ItemDataBound += SubordinateRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindModelToControls(User.UserIdentifier);

            var hasData = ManagersPanel.Visible
                || SupervisorsPanel.Visible
                || ValidatorsPanel.Visible
                || SubordinatesPanel.Visible
                || LearnersPanel.Visible;

            if (!hasData)
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("There are no Contacts to display related to your learner profile."));

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);
        }

        private void BindModelToControls(Guid personID)
        {
            var leaders = UserConnectionSearch.SelectCmdsDetails(personID, Organization.Identifier, true, null, null, null);
            LeaderRepeater.DataSource = leaders;
            LeaderRepeater.DataBind();
            LeadersPanel.Visible = leaders.Count > 0;

            var managers = UserConnectionSearch.SelectCmdsDetails(personID, Organization.Identifier, null, true, null, null);
            ManagerRepeater.DataSource = managers;
            ManagerRepeater.DataBind();
            ManagersPanel.Visible = managers.Count > 0;

            var supervisors = UserConnectionSearch.SelectCmdsDetails(personID, Organization.Identifier, null, null, true, null);
            SupervisorRepeater.DataSource = supervisors;
            SupervisorRepeater.DataBind();
            SupervisorsPanel.Visible = supervisors.Count > 0;

            var validators = UserConnectionSearch.SelectCmdsDetails(personID, Organization.Identifier, null, null, null, true);
            ValidatorRepeater.DataSource = validators;
            ValidatorRepeater.DataBind();
            ValidatorsPanel.Visible = validators.Count > 0;

            var filter = new PersonFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                ParentUserIdentifier = personID,
                ExcludeUserIdentifier = personID,
                RelationWithParent = new[] { RelationCategory.Manager, RelationCategory.Supervisor }
            };
            var subordinates = ContactRepository3.SelectPersons(filter, Organization.Identifier);
            SubordinateRepeater.DataSource = subordinates;
            SubordinateRepeater.DataBind();
            SubordinatesPanel.Visible = subordinates.Rows.Count > 0;

            filter.RelationWithParent = new[] { RelationCategory.Validator };
            var learners = ContactRepository3.SelectPersons(filter, Organization.Identifier);
            LearnerRepeater.DataSource = learners;
            LearnerRepeater.DataBind();
            LearnersPanel.Visible = learners.Rows.Count > 0;
        }

        private void SubordinateRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var anchor = (HtmlAnchor)e.Item.FindControl("HomeAnchor");

            var userIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "UserIdentifier");

            var href = $"{Shift.Common.Urls.CmdsHomeUrl}?id={userIdentifier}";

            anchor.HRef = href;
        }
    }
}