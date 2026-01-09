using System;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.Infrastructure;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ArchiveRepeater.ItemCommand += ArchivesRepeater_ItemCommand;
            ArchiveRepeater.DataBinding += ArchivesRepeater_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                SetControlsVisibility();
                BindModelToControls();
            }
        }

        private void SetControlsVisibility()
        {
            DownloadXLSXSection.Visible = Identity.IsGranted(ActionName.Admin_Contacts_People_Edit_SocialInsuranceNumber);
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var peopleCount = PersonCriteria.Count(new PersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CloakedUsers = User.IsCloaked ? InclusionType.Include : InclusionType.Exclude
            });
            LoadCounter(PeopleCounter, PeopleCount, peopleCount, PeopleLink, "/ui/admin/contacts/people/search");

            var groupCount = ServiceLocator.GroupSearch.CountGroups(new QGroupFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                OrganizationIdentifiers = CurrentSessionState.Identity.Organizations.Select(x => x.Identifier).ToArray()
            });
            LoadCounter(GroupCounter, GroupCount, groupCount, GroupLink, "/ui/admin/contacts/groups/search");

            var membershipCount = MembershipSearch.Count(x => x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier);
            LoadCounter(MembershipCounter, MembershipCount, membershipCount, MembershipLink, "/ui/admin/contacts/memberships/search");

            var usersConnectionsCount = ServiceLocator.UserSearch.CountConnections(new QUserConnectionFilter() { FromUserOrganizationId = Organization.Identifier });
            LoadCounter(UsersConnectionsCounter, UsersConnectionsCount, usersConnectionsCount, UsersConnectionsLink, "/ui/admin/identity/users-connections/search");

            var membershipReasonCount = ServiceLocator.MembershipReasonSearch.Count(new QMembershipReasonFilter { GroupOrganizationIdentifiers = new[] { Organization.Identifier } });
            LoadCounter(MembershipReasonCounter, MembershipReasonCount, membershipReasonCount, MembershipReasonLink, "/ui/admin/contacts/memberships/reasons/search");
            MembershipReasonCounter.Visible = membershipReasonCount > 0;

            var groupCountByLabel = ServiceLocator.GroupSearch.CountPerLabel(Organization.OrganizationIdentifier);
            GroupLabelRepeater.Visible = Identity.IsActionAuthorized("/ui/admin/contacts/groups/search");
            GroupLabelRepeater.DataSource = GroupLabelRepeater.Visible ? groupCountByLabel : null;
            GroupLabelRepeater.DataBind();
            // GroupLabelPanel.Visible = GroupLabelRepeater.Visible && groupCountByLabel.Count > 0;

            var groupCountByType = ServiceLocator.GroupSearch.CountPerType(Organization.OrganizationIdentifier);
            GroupTypeRepeater.Visible = Identity.IsActionAuthorized("/ui/admin/contacts/groups/search");
            GroupTypeRepeater.DataSource = GroupTypeRepeater.Visible ? groupCountByType : null;
            GroupTypeRepeater.DataBind();
            // GroupTypePanel.Visible = GroupTypeRepeater.Visible && groupCountByType.Count > 0;

            PersonRecentList.LoadData(5);
            HistoryPanel.Visible = PersonRecentList.ItemCount > 0;

            ArchiveRepeater.DataBind();
        }

        private void ArchivesRepeater_DataBinding(object sender, EventArgs e)
        {
            var archiveFiles = UploadSearch.BindFolderFiles(
                Organization.OrganizationIdentifier,
                OrganizationRelativePath.ContactArchivePath,
                x => new { Path = x.NavigateUrl, x.Name },
                null,
                "Name");

            ArchivePanel.Visible = archiveFiles.Count > 0;
            ArchiveRepeater.DataSource = archiveFiles;
        }

        private void ArchivesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var path = (string)e.CommandArgument;

                FileHelper.Provider.Delete(Organization.OrganizationIdentifier, path);

                ArchiveRepeater.DataBind();
            }
            else
            {
                throw new NotImplementedException($"Unexpected command name: {e.CommandName}");
            }
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, int count, HtmlAnchor link, string action)
        {
            card.Visible = CurrentSessionState.Identity.IsActionAuthorized(action);
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }

        protected static string GetGroupLabelUrl(object obj)
        {
            var model = (CountModel)obj;
            return $"/ui/admin/contacts/groups/search?clearcriteria=1&label={model.Label}";
        }

        protected static string GetGroupTypeUrl(object obj)
        {
            var model = (CountModel)obj;
            return $"/ui/admin/contacts/groups/search?type={HttpUtility.UrlEncode(model.Type)}";
        }
    }
}