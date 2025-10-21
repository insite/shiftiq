using System;
using System.Data;
using System.Text;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
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
        private const string EmployerSearchUrl = "/ui/admin/jobs/employers/search";

        #endregion

        #region Properties

        private Guid GroupIdentifier => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        private bool IsEmployer => Request.QueryString["employer"] == "1";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            ArchiveButton.Click += ArchiveButton_Click;
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

            BindModelToControls();

            var fileName = StringHelper.Sanitize(group.GroupName, '-') + ".csv";
            ArchiveButton.OnClientClick = $"return confirm('Are you sure you want to archive this group? The name and email address for each contact on this list will be saved to a text file named \"{fileName}\", which you can download to your computer, and then the list will be removed from the database.');";

            GroupDetail.BindGroup(group);

            return group.GroupName;
        }

        #endregion

        #region Event handlers

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var group = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier);

            if (group != null)
            {
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
            var info = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier);

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
                               StringHelper.Sanitize(info.GroupName, '-') + ".csv";

                FileHelper.Provider.Save(Organization.OrganizationIdentifier, filePath, csvText, Encoding.UTF8);
            }

            GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, GroupIdentifier);

            RedirectToFinder();
        }

        #endregion

        #region Methods (data binding)

        protected void BindModelToControls()
        {
            //Contained Contacts
            var contactsCount = ServiceLocator.PersonSearch.CountPersons(new QPersonFilter { EmployerGroupIdentifier = GroupIdentifier });
            ContactsCount.Text = contactsCount.ToString();

            //Upstream Relationships
            var upstreamCount = ServiceLocator.GroupSearch.CountParentConnections(GroupIdentifier);
            UpstreamCount.Text = upstreamCount.ToString();

            //Downstream Relationships
            var downstreamCount = ServiceLocator.GroupSearch.CountChildConnections(GroupIdentifier);
            DownstreamCount.Text = downstreamCount.ToString();

            //Permissions
            var permissionsCount = TGroupPermissionSearch.Count(new TGroupActionFilter { GroupIdentifier = GroupIdentifier });
            PermissionsCount.Text = permissionsCount.ToString();

            //Billing Customers
            var customersCount = ServiceLocator.RegistrationSearch.CountRegistrations(new QRegistrationFilter { RegistrationCustomerIdentifier = GroupIdentifier });
            CustomersCount.Text = customersCount.ToString();

            //Employers
            var employersCount = ServiceLocator.RegistrationSearch.CountRegistrations(new QRegistrationFilter { RegistrationEmployerIdentifier = GroupIdentifier });
            EmployersCount.Text = employersCount.ToString();

            //Events
            var eventsCount = ServiceLocator.EventSearch.CountEvents(new QEventFilter { VenueLocationIdentifier = new[] { GroupIdentifier } });
            EventsCount.Text = eventsCount.ToString();

            //Jobs
            var opportunityCount = TOpportunitySearch.Count(x => x.EmployerGroupIdentifier == GroupIdentifier);
            OpportunityCount.Text = $"{opportunityCount:n0}";

            //Job Applications
            var applicationCount = TApplicationSearch.Count(new TApplicationFilter { EmployerGroupIdentifier = GroupIdentifier });
            ApplicationCount.Text = $"{applicationCount:n0}";

            if (eventsCount > 0)
            {
                AdminErrorPanel.InnerHtml = $"This group is a <strong>Venue</strong> for {"event".ToQuantity(eventsCount)}. You cannot delete this group without first assigning a different venue to these events.";
                AdminErrorPanel.Visible = true;

                ConfirmPanel.Visible = false;
                DeleteButton.Visible = false;
                ArchiveButton.Visible = false;
            }
            else if (upstreamCount > 0 || downstreamCount > 0)
            {
                AdminErrorPanel.InnerHtml = $"This group has some relationships with other groups. You cannot delete this group without first unassigning them from functional parent groups.";
                AdminErrorPanel.Visible = true;

                ConfirmPanel.Visible = false;
                DeleteButton.Visible = false;
                ArchiveButton.Visible = false;
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToFinder()
        {
            var url = IsEmployer ? EmployerSearchUrl : GroupSearchUrl;

            HttpResponseHelper.Redirect(url);
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