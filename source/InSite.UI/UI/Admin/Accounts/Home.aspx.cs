using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Accounts
{
    public partial class Home : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearOrganizationCache.Click += (x, y) =>
            {
                OrganizationSearch.Refresh();
                Global.InitIntegrations();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindModelToControls();

        }
        protected void BindModelToControls()
        {
            PageHelper.BindHomeHeader(Page,
                new BreadcrumbItem[] { new BreadcrumbItem("Accounts", null, null, "active") },
                new BreadcrumbItem[] {
                    new BreadcrumbItem("Organization", "/ui/admin/accounts/organizations/create"),
                    new BreadcrumbItem("Department", "/ui/admin/accounts/departments/create"),
                    new BreadcrumbItem("Developer", "/ui/admin/accounts/developers/create"),
                    new BreadcrumbItem("Division", "/ui/admin/accounts/divisions/create"),
                    new BreadcrumbItem("Permission", "/ui/admin/accounts/permissions/create"),
                    new BreadcrumbItem("Sender", "/ui/admin/accounts/senders/create")
                }
            );

            var insite = CurrentSessionState.Identity.IsOperator;
            var cmds = CurrentSessionState.Identity.IsInRole(CmdsRole.Programmers);

            var organizationCount = OrganizationSearch.Count(new OrganizationFilter());
            LoadCounter(OrganizationCounter, OrganizationCount, insite, organizationCount, OrganizationLink, "/ui/admin/accounts/organizations/search");

            var totalUserCount = UserSearch.Count(new UserFilter());
            LoadCounter(UserCounter, UserCount, insite || cmds, totalUserCount, UserLink, "/ui/admin/accounts/users/search");

            var permissionCount = TGroupPermissionSearch.Count(new TGroupActionFilter { OrganizationIdentifier = Organization.Identifier });
            LoadCounter(PermissionCounter, PermissionCount, insite, permissionCount, PermissionLink, "/ui/admin/accounts/permissions/search");

            var senderCount = TSenderSearch.Count(new TSenderFilter());
            LoadCounter(SenderCounter, SenderCount, insite, senderCount, SenderLink, "/ui/admin/accounts/senders/search");

            var departmentCount = DepartmentSearch.Count();
            LoadCounter(DepartmentCounter, DepartmentCount, insite, departmentCount, DepartmentLink, "/ui/admin/accounts/departments/search");

            var divisionCount = DivisionSearch.Count(new DivisionFilter());
            LoadCounter(DivisionCounter, DivisionCount, insite, divisionCount, DivisionLink, "/ui/admin/accounts/divisions/search");

            BindDownloadLink();
        }

        private void BindDownloadLink()
        {
            var permissions = PermissionCache.Matrix.GetPermissions(Organization.Code);

            var serialized = ServiceLocator.Serializer.Serialize(permissions);

            using (var sha = SHA256.Create())
            {
                var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(serialized));

                var checksum = BitConverter.ToString(hashed).Replace("-", "").Substring(0, 12);

                var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized));

                var href = $"data:application/json;base64,{encoded}";

                DownloadNewPermissionMatrix.Text =
                    $"<a href=\"{href}\" download=\"permission-matrix.json\" title=\"Checksum {checksum}\"><i class=\"fas fa-download me-1\"></i>Download Matrix</a>";
            }
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, bool visible, int count, HtmlAnchor link, string action)
        {
            card.Visible = visible;
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }
    }
}