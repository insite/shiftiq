using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Admin.Districts.Forms
{
    public partial class Edit : AdminBasePage
    {
        private Guid Identifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        private string OrgUrl => $"/ui/cmds/admin/organizations/edit?id={OrganizationIdentifier}";

        private Guid? OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Organizations", "/ui/cmds/admin/organizations/search"),
                    new BreadcrumbItem("Edit", OrgUrl),
                    new BreadcrumbItem("Division", null)
                },
                new BreadcrumbItem("Create", $"/ui/cmds/admin/divisions/create?organizationID={OrganizationIdentifier}"));

                CancelButton.NavigateUrl = OrgUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();

            HttpResponseHelper.Redirect(OrgUrl);
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!CheckDeleteAllowed())
                return;

            GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, Identifier);

            HttpResponseHelper.Redirect(OrgUrl);
        }

        private void Open()
        {
            var division = DivisionSearch.Select(Identifier);
            if (division == null)
                HttpResponseHelper.Redirect("/ui/cmds/admin/organizations/search", true);

            OrganizationIdentifier = division.OrganizationIdentifier;

            SetInputValues(division);
        }

        private void Save()
        {
            var division = DivisionSearch.Select(Identifier);

            GetInputValues(division);

            var id = division.DivisionIdentifier;

            var commands = new List<Command>
            {
                new RenameGroup(id, "District", division.DivisionName),
                new DescribeGroup(id, null, division.DivisionCode, division.DivisionDescription, null)
            };

            ServiceLocator.SendCommands(commands);
        }

        private void SetInputValues(Division division)
        {
            Name.Text = division.DivisionName;

            var organization = OrganizationSearch.Select(division.OrganizationIdentifier);

            PageHelper.AutoBindHeader(this, null, $"{organization.CompanyName} ({division.DivisionName})");
        }

        private void GetInputValues(Division division)
        {
            division.DivisionName = Name.Text;
        }

        private bool CheckDeleteAllowed()
        {
            var departments = Persistence.Plugin.CMDS.ContactRepository3.SelectGroupsByFromId(Identifier);

            if (departments.Count == 0)
                return true;

            var division = DivisionSearch.Select(Identifier);

            StringBuilder message = new StringBuilder();
            message.AppendFormat("You can't delete this division ({0}) because it is referenced by the following departments:<br/>", division.DivisionName);

            AddRelatedDepartments(message, departments);

            message.Append("You must remove all references to the division (from departments) before you can delete the division.");

            EditorStatus.AddMessage(AlertType.Error, message.ToString());

            return false;
        }

        private static void AddRelatedDepartments(StringBuilder message, List<Department> departments)
        {
            message.Append("<ul>");

            foreach (var info in departments)
                message.AppendFormat("<li>{0}</li>", info.DepartmentName);

            message.Append("</ul>");
        }
    }
}
