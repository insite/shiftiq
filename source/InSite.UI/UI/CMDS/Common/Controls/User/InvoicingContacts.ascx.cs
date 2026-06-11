using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Cmds.Controls.User
{
    public partial class InvoicingContacts : UserControl
    {
        #region Properties

        /// <summary>
        /// When true, each member shows a "remove from team" button that deletes the membership.
        /// Defaults to false so the control is view-only unless the container opts in.
        /// </summary>
        public bool AllowMembershipDeletion
        {
            get => ViewState[nameof(AllowMembershipDeletion)] as bool? ?? false;
            set => ViewState[nameof(AllowMembershipDeletion)] = value;
        }

        private Guid OrganizationIdentifier
        {
            get => (Guid)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        private Guid PartitionId => ServiceLocator.AppSettings.Partition.Identifier;

        #endregion

        #region Public methods

        public void LoadData(Guid organizationIdentifier)
        {
            OrganizationIdentifier = organizationIdentifier;

            BindInvoicing();
        }

        #endregion

        #region Event handlers

        protected void MembersRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!AllowMembershipDeletion)
                return;

            if (e.CommandName != "Remove")
                return;

            var parts = ((string)e.CommandArgument).Split('|');
            if (parts.Length != 2)
                return;

            if (!Guid.TryParse(parts[0], out var groupId) || !Guid.TryParse(parts[1], out var userId))
                return;

            MembershipStore.Delete(new Membership { GroupIdentifier = groupId, UserIdentifier = userId });

            BindInvoicing();
        }

        #endregion

        #region Helper methods

        private void BindInvoicing()
        {
            var organizationId = OrganizationIdentifier;

            var teams = ServiceLocator.GroupSearch
                .BindGroups(
                    x => new { x.GroupIdentifier, x.GroupName },
                    x => x.OrganizationIdentifier == PartitionId
                    && x.GroupType == "Team"
                    && x.GroupCategory == "Invoicing"
                    );

            var groupIds = teams.Select(x => x.GroupIdentifier).ToArray();

            var members = MembershipSearch.Bind(x =>
                new InvoicingTeamMember
                {
                    GroupId = x.Group.GroupIdentifier,
                    GroupName = x.Group.GroupName,
                    UserId = x.User.UserIdentifier,
                    UserName = x.User.FullName,
                    UserEmail = x.User.Email,
                    UserPhone = x.User.PhoneMobile
                },
                x => groupIds.Contains(x.GroupIdentifier) && x.OrganizationIdentifier == organizationId);

            var groups = teams
                .Select(t => new InvoicingTeam
                {
                    GroupName = t.GroupName,
                    Members = members
                        .Where(m => m.GroupId == t.GroupIdentifier)
                        .OrderBy(x => x.UserName)
                        .ToArray()
                })
                .OrderBy(x => x.GroupName)
                .ToArray();

            GroupMembershipRepeater.DataSource = groups;
            GroupMembershipRepeater.DataBind();
        }

        private class InvoicingTeamMember
        {
            public Guid GroupId { get; set; }
            public string GroupName { get; set; }
            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string UserPhone { get; set; }
        }

        private class InvoicingTeam
        {
            public string GroupName { get; set; }
            public InvoicingTeamMember[] Members { get; set; }
        }

        #endregion
    }
}
