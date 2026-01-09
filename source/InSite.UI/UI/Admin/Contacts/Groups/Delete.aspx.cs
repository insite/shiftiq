using System;
using System.Data;
using System.Text;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Contacts.Groups
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string GroupSearchUrl = "/ui/admin/contacts/groups/search";

        #endregion

        #region Properties

        private Guid GroupIdentifier => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        private bool IsEmployer => Request.QueryString["employer"] == "1";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.ServerClick += DeleteButton_Click;

            ArchiveButton.ServerClick += ArchiveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var groupName = LoadData();

                if (IsEmployer)
                {
                    PageHelper.AutoBindHeader(this, null, groupName);

                    CancelButton.NavigateUrl = $"/ui/admin/contacts/groups/edit?contact={GroupIdentifier}&employer=1";
                }
                else
                {
                    PageHelper.BindHeader(this, new BreadcrumbItem[]
                    {
                        new BreadcrumbItem("Contacts", "/ui/admin/contacts/home"),
                        new BreadcrumbItem("Groups", GroupSearchUrl),
                        new BreadcrumbItem("Edit", $"/ui/admin/contacts/groups/edit?contact={GroupIdentifier}"),
                        new BreadcrumbItem("Delete", null, null, "active"),
                    }, null, groupName);

                    CancelButton.NavigateUrl = $"/ui/admin/contacts/groups/edit?contact={GroupIdentifier}";
                }
            }
        }

        private string LoadData()
        {
            var group = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier);

            if (group == null || group.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                RedirectToFinder();
                return null;
            }

            GroupDetail.BindGroup(group);

            GroupName1.Text = GroupName2.Text = group.GroupName;

            var archiveFileName = StringHelper.Sanitize(group.GroupName, '-') + ".csv";

            ArchiveFileName.Text = archiveFileName;

            BindModelToControls(
                GroupDetail.CountEventVenues(),
                GroupDetail.CountUpstreamRelationships(),
                GroupDetail.CountDownstreamRelationships());

            return group.GroupName;
        }

        #endregion

        #region Event handlers

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var group = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier);

            if (group != null)
            {
                var isConfirmed = group.GroupName == DeleteInput.Value;

                if (!isConfirmed)
                    HttpResponseHelper.Redirect(Request.RawUrl, "status=delete-cancelled");

                try
                {
                    GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, GroupIdentifier);

                    if (!group.GroupImage.HasValue())
                        FileHelper.Provider.Delete(CurrentSessionState.Identity.Organization.Identifier, group.GroupImage);

                    RedirectToFinder();
                }
                catch (Exception)
                {
                    AdminErrorPanel.InnerHtml = $"This group is a <strong>Venue</strong>";
                    AdminErrorPanel.Visible = true;
                }
            }
        }

        private void ArchiveButton_Click(object sender, EventArgs e)
        {
            var group = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier);

            var isConfirmed = group.GroupName == ArchiveInput.Value;

            if (!isConfirmed)
                HttpResponseHelper.Redirect(Request.RawUrl, "status=archive-cancelled");

            var table = new DataTable();
            table.Columns.Add("FirstName");
            table.Columns.Add("LastName");
            table.Columns.Add("Email");

            var contacts = MembershipSearch.Bind(x => new
            {
                x.User.FirstName,
                x.User.LastName,
                x.User.Email
            }, x => x.GroupIdentifier == GroupIdentifier, "FirstName,LastName");

            if (contacts.Length > 0)
            {
                foreach (var contact in contacts)
                {
                    var row = table.NewRow();
                    row["FirstName"] = contact.FirstName;
                    row["LastName"] = contact.LastName;
                    row["Email"] = contact.Email;
                    table.Rows.Add(row);
                }

                var helper = new CsvExportHelper(table);

                helper.AddMapping("FirstName", "FirstName");
                helper.AddMapping("LastName", "LastName");
                helper.AddMapping("Email", "Email");

                var csvText = helper.GetString();

                var filePath = OrganizationRelativePath.ContactArchivePath +
                               StringHelper.Sanitize(group.GroupName, '-') + ".csv";

                FileHelper.Provider.Save(Organization.OrganizationIdentifier, filePath, csvText, Encoding.UTF8);
            }

            GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, GroupIdentifier);

            RedirectToFinder();
        }

        #endregion

        #region Methods (data binding)

        private void BindModelToControls(int eventsCount, int upstreamCount, int downstreamCount)
        {
            if (eventsCount > 0)
            {
                AdminErrorPanel.InnerHtml = $"This group is a <strong>Venue</strong> for {"event".ToQuantity(eventsCount)}. You cannot delete this group without first assigning a different venue to these events.";
                AdminErrorPanel.Visible = true;

                DeletePanel.Visible = false;
                DeleteButton.Disabled = true;
                ArchiveButton.Visible = false;
            }
            else if (upstreamCount > 0 || downstreamCount > 0)
            {
                AdminErrorPanel.InnerHtml = $"This group has some relationships with other groups. You cannot delete this group without first unassigning them from functional parent groups.";
                AdminErrorPanel.Visible = true;

                DeletePanel.Visible = false;
                DeleteButton.Disabled = true;
                ArchiveButton.Visible = false;
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToFinder()
        {
            HttpResponseHelper.Redirect(GroupSearchUrl);
        }

        #endregion

        #region IHasParentLinkParameters & IOverrideWebRouteParent

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={GroupIdentifier}"
                : null;
        }

        #endregion
    }
}