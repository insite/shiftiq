using System;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Identities.Departments.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/departments/search";

        private Guid DepartmentID => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void Open()
        {
            var entity = DepartmentSearch.Select(DepartmentID);
            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, null, entity.DepartmentName);

            DepartmentDetails.SetInputValues(entity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entityId = DepartmentID;
            var entity = DepartmentSearch.Select(entityId);

            DepartmentDetails.GetInputValues(entity);

            var commands = new ICommand[]
            {
                new RenameGroup(entityId, "Department", entity.DepartmentName),
                new DescribeGroup(entityId, null, entity.DepartmentCode, entity.DepartmentDescription, entity.DepartmentLabel),
                new ChangeGroupParent(entityId, entity.DivisionIdentifier)
            };

            ServiceLocator.SendCommands(commands);

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, DepartmentID);

            HttpResponseHelper.Redirect(SearchUrl);
        }
    }
}