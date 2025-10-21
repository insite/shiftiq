using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

namespace InSite.UI.Admin.Courses.Outlines.Controls
{
    public partial class PrivacySettingsGroups : UserControl
    {
        protected Guid ContainerIdentifier
        {
            get => ViewState[nameof(ContainerIdentifier)] as Guid? ?? Guid.Empty;
            set => ViewState[nameof(ContainerIdentifier)] = value;
        }

        protected string ContainerType
        {
            get => ViewState[nameof(ContainerType)] as string;
            set => ViewState[nameof(ContainerType)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadData(ContainerIdentifier, ContainerType);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var list = new List<Guid>();

            foreach (RepeaterItem item in ListRepeater.Items)
            {
                var privacyIdentifierLiteral = (Literal)item.FindControl("PrivacyIdentifier");
                var isSelectedCheckBox = (CheckBox)item.FindControl("IsSelected");

                if (isSelectedCheckBox.Checked)
                {
                    var privacyIdentifier = Guid.Parse(privacyIdentifierLiteral.Text);
                    list.Add(privacyIdentifier);
                }
            }

            if (list.Count > 0)
            {
                foreach (var privacyIdentifier in list)
                    TGroupPermissionStore.Delete(privacyIdentifier);

                LoadData(ContainerIdentifier, ContainerType);
            }
        }

        public void LoadData(Guid containerIdentifier, string containerType)
        {
            ContainerIdentifier = containerIdentifier;
            ContainerType = containerType;

            var list = TGroupPermissionSearch.Select(x => x.ObjectIdentifier == containerIdentifier, null, x => x.Group)
                .Select(x => new
                {
                    PrivacyIdentifier = x.PermissionIdentifier,
                    GroupName = x.Group.GroupName,
                    GroupType = x.Group.GroupType
                })
                .ToList();

            ListRepeater.DataSource = list;
            ListRepeater.DataBind();

            SelectAllButton.Visible = list.Count > 0;
            SelectAllButton.OnClientClick = $"return groupPrivacyList{ContainerType}.selectAll();";

            UnselectAllButton.Visible = list.Count > 0;
            UnselectAllButton.OnClientClick = $"return groupPrivacyList{ContainerType}.unselectAll();";

            AddButton.OnClientClick = $"return groupPrivacyList{ContainerType}.add();";

            DeleteButton.Visible = list.Count > 0;
        }
    }
}