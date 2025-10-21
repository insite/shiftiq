using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Utilities.Actions.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/platform/routes/search";

        private Guid Identifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddPermissionButton.Click += AddPermissionButton_Click;

            PermissionRepeater.ItemCommand += PermissionRepeater_ItemCommand;
            PermissionRepeater.DataBinding += PermissionRepeater_DataBinding;
            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                CancelButton.NavigateUrl = SearchUrl;

                if (string.Equals(Request.QueryString["panel"], "permission", StringComparison.OrdinalIgnoreCase))
                    PermissionPanel.IsSelected = true;
            }
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TActionStore.Delete(Identifier);
            HttpResponseHelper.Redirect(SearchUrl);
        }

        private void Open()
        {
            var info = TActionSearch.Get(Identifier);

            if (info == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var title = info.ActionUrl;

            if (info.ActionIcon != null)
                title = $"<i class='{info.ActionIcon}'></i> " + title;

            PageHelper.AutoBindHeader(this, null, title);

            Detail.SetInputValues(info);
            PermissionRepeater.DataBind();
            LoadSubactions(info);
        }

        private void GetInputValues(TAction info)
        {
            Detail.GetInputValues(info);
        }

        private void Save()
        {
            TActionStore.Update(Identifier, x => GetInputValues(x));

            TActionSearch.Refresh();
        }

        private void AddPermissionButton_Click(object sender, EventArgs e)
        {
            HttpResponseHelper.Redirect($"/ui/admin/identity/permissions/create?action={Identifier}");
        }

        private void PermissionRepeater_DataBinding(object sender, EventArgs e)
        {
            var permissions = TGroupPermissionSearch.Bind(x => new
            {
                x.Group.GroupIdentifier,
                x.Group.GroupType,
                x.Group.GroupName,
                ActionIdentifier = x.ObjectIdentifier,
                x.AllowRead,
                x.AllowWrite,
                x.AllowCreate,
                x.AllowDelete,
                x.AllowAdministrate,
                AllowFullControl = x.AllowConfigure
            },
                x => x.ObjectIdentifier == Identifier,
                "GroupType,GroupName");

            PermissionRepeater.DataSource = permissions;
        }

        private void PermissionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                if (Guid.TryParse((string)e.CommandArgument, out Guid contactID))
                {
                    TGroupPermissionStore.Delete(contactID, Identifier);

                    PermissionRepeater.DataBind();
                }
            }
            else
            {
                throw new NotImplementedException($"Unexpected command name: {e.CommandName}");
            }
        }

        private void LoadSubactions(TAction action)
        {
            var isPermission = action.ActionType == "Permission";
            var routes = TActionSearch.Search(x => x.PermissionParentActionIdentifier == action.ActionIdentifier
                                                || x.NavigationParentActionIdentifier == action.ActionIdentifier);

            ActionRepeater.DataSource = routes.OrderBy(x => x.ActionUrl).Select(x => new
            {
                x.ActionUrl,
                Url = $"/ui/admin/platform/routes/edit?id={x.ActionIdentifier}",
                x.ActionIcon,
                x.ActionName,
                Note = isPermission && x.PermissionParentActionIdentifier != action.ActionIdentifier 
                    ? "Permission Downstream" 
                    : !isPermission && x.NavigationParentActionIdentifier != action.ActionIdentifier 
                        ? "Navigation Downstream"
                        : null
            });
            ActionRepeater.DataBind();
        }
    }
}
