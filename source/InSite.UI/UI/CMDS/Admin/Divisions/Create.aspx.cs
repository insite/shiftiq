using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;

using BreadcrumbItem = Shift.Contract.BreadcrumbItem;

namespace InSite.Cmds.Admin.Districts.Forms
{
    public partial class Create : AdminBasePage
    {
        private Guid? _organizationId;
        private Guid? OrganizationIdentifier
        {
            get
            {
                if (_organizationId == null && Guid.TryParse(Request["organizationID"], out var value))
                    _organizationId = value;

                return _organizationId;
            }
        }

        private string OrgUrl => string.Format("/ui/cmds/admin/organizations/edit?id={0}", OrganizationIdentifier);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (OrganizationIdentifier == null)
                    HttpResponseHelper.Redirect("/ui/cmds/admin/organizations/search");

                var organization = OrganizationSearch.Select(OrganizationIdentifier.Value);

                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Organizations", "/ui/cmds/admin/organizations/search"),
                    new BreadcrumbItem("Edit", OrgUrl),
                    new BreadcrumbItem("Division", null)
                }, null, organization.CompanyName);

                CancelButton.NavigateUrl = OrgUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var division = DivisionFactory.Create(UniqueIdentifier.Create());

            GetInputValues(division);

            var id = division.DivisionIdentifier;

            var commands = new List<Command>
            {
                new CreateGroup(id, division.OrganizationIdentifier, "District", division.DivisionName),
                new DescribeGroup(id, null, division.DivisionCode, division.DivisionDescription, null)
            };

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect(OrgUrl);
        }

        private void GetInputValues(Division district)
        {
            district.DivisionName = Name.Text;
            district.OrganizationIdentifier = OrganizationIdentifier.Value;
        }
    }
}
