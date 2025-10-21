using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Files.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Assets.Files.Controls
{
    public partial class FilePermissionList : BaseUserControl
    {
        [Serializable]
        class PermissionObject
        {
            public string ObjectType { get; set; }
            public Guid ObjectIdentifier { get; set; }
            public string ObjectName { get; set; }
        }

        private List<PermissionObject> Objects
        {
            get => (List<PermissionObject>)ViewState[nameof(Objects)];
            set => ViewState[nameof(Objects)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublicRadioButton.AutoPostBack = true;
            PublicRadioButton.CheckedChanged += (x, y) => ShowPermissionType();

            PrivateRadioButton.AutoPostBack = true;
            PrivateRadioButton.CheckedChanged += (x, y) => ShowPermissionType();

            ClaimObjectType.AutoPostBack = true;
            ClaimObjectType.ValueChanged += (x, y) => ShowClaimObject();

            GroupID.AutoPostBack = true;
            GroupID.ValueChanged += (x, y) => AddObject();

            PersonID.AutoPostBack = true;
            PersonID.ValueChanged += (x, y) => AddObject();

            PermissionList.ItemCommand += PermissionList_ItemCommand;
        }

        public void BindDefaultsToControls()
        {
            ShowPermissionType();
            BindPermissions();
        }

        public void BindModelToControls(FileStorageModel model)
        {
            var hasClaims = model.Claims != null && model.Claims.Any();

            PublicRadioButton.Checked = !hasClaims;
            PrivateRadioButton.Checked = hasClaims;

            if (hasClaims)
            {
                Objects = model.Claims
                    .Select(x => new PermissionObject
                    {
                        ObjectType = x.ObjectType == FileClaimObjectType.Group ? "Group" : "User",
                        ObjectIdentifier = x.ObjectIdentifier,
                        ObjectName = GetObjectName(x)
                    })
                    .ToList();
            }

            ShowPermissionType();
            BindPermissions();
        }

        private static string GetObjectName(FileClaim claim)
        {
            switch (claim.ObjectType)
            {
                case FileClaimObjectType.Person:
                    return UserSearch.Select(claim.ObjectIdentifier)?.FullName ?? "<Deleted>";
                case FileClaimObjectType.Group:
                    return ServiceLocator.GroupSearch.GetGroup(claim.ObjectIdentifier)?.GroupName ?? "<Deleted>";
                default:
                    throw new ArgumentException($"Object type '{claim.ObjectType}' is not supported");
            }
        }

        public List<FileClaim> GetFileClaims()
        {
            if (Objects == null || PublicRadioButton.Checked)
                return null;

            return Objects.Select(x => new FileClaim
            {
                ObjectIdentifier = x.ObjectIdentifier,
                ObjectType = x.ObjectType == "Group" ? FileClaimObjectType.Group : FileClaimObjectType.Person
            })
                .ToList();
        }

        private void PermissionList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var objectIdentifier = Guid.Parse(e.CommandArgument.ToString());
            var index = Objects.FindIndex(x => x.ObjectIdentifier == objectIdentifier);

            Objects.RemoveAt(index);

            BindPermissions();
        }

        private void ShowPermissionType()
        {
            ClaimObjectPanel.Visible = PrivateRadioButton.Checked;
            PermissionList.Visible = PrivateRadioButton.Checked;

            ShowClaimObject();
        }

        private void ShowClaimObject()
        {
            var isGroup = ClaimObjectType.Value == "Group";

            GroupID.Visible = isGroup;
            PersonID.Visible = !isGroup;
        }

        private void AddObject()
        {
            var isGroup = ClaimObjectType.Value == "Group";
            var objectId = isGroup ? GroupID.Value : PersonID.Value;

            if (objectId == null)
                return;

            GroupID.Value = null;
            PersonID.Value = null;

            var objectName = isGroup
                ? ServiceLocator.GroupSearch.GetGroup(objectId.Value).GroupName
                : UserSearch.Select(objectId.Value).FullName;

            if (Objects == null)
                Objects = new List<PermissionObject>();

            if (Objects.Any(x => x.ObjectIdentifier == objectId))
                return;

            Objects.Add(new PermissionObject
            {
                ObjectType = ClaimObjectType.Value,
                ObjectIdentifier = objectId.Value,
                ObjectName = objectName
            });

            BindPermissions();
        }

        private void BindPermissions()
        {
            if (Objects == null)
            {
                Objects = new List<PermissionObject>
                {
                    new PermissionObject
                    {
                        ObjectType = "User",
                        ObjectIdentifier = User.Identifier,
                        ObjectName = User.FullName
                    }
                };
            }

            PermissionList.DataSource = Objects;
            PermissionList.DataBind();
        }
    }
}