using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common.Events;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class DepartmentChecklist : UserControl
    {
        #region Events

        public event IntValueHandler Refreshed;
        private void OnRefreshed(int count) =>
            Refreshed?.Invoke(this, new IntValueArgs(count));

        #endregion

        #region Properties

        private Guid? AchievementIdentifier
        {
            get { return (Guid?)ViewState[nameof(AchievementIdentifier)]; }
            set { ViewState[nameof(AchievementIdentifier)] = value; }
        }

        private List<Guid> ChangedList => (List<Guid>)(ViewState[nameof(ChangedList)]
            ?? (ViewState[nameof(ChangedList)] = new List<Guid>()));

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentsRepeater.ItemCreated += DepartmentsRepeater_ItemCreated;
        }

        #endregion

        #region Loading

        public int LoadAchievements(Guid achievementIdentifier)
        {
            AchievementIdentifier = achievementIdentifier;

            var data = VCmdsAchievementSearch.SelectForDepartmentChecklist(CurrentIdentityFactory.ActiveOrganizationIdentifier, achievementIdentifier);

            DepartmentsRepeater.DataSource = data;
            DepartmentsRepeater.DataBind();

            var count = 0;
            foreach (DataRow row in data.Rows)
            {
                if ((bool)row["IsChecked"])
                    count++;
            }
            return count;
        }

        #endregion

        #region Event handlers

        private void DepartmentsRepeater_ItemCreated(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var chk = (ICheckBoxControl)e.Item.FindControl("IsDepartmentSelected");
                chk.CheckedChanged += IsDepartmentSelected_CheckedChanged;
            }
        }

        private void IsDepartmentSelected_CheckedChanged(object sender, EventArgs e)
        {
            System.Web.UI.Control container = (sender as System.Web.UI.Control).NamingContainer;

            if (container != null)
            {
                var hf = (ITextControl)container.FindControl("DepartmentIdentifier");
                var departmentIdentifier = Guid.Parse(hf.Text);

                if (!ChangedList.Contains(departmentIdentifier))
                    ChangedList.Add(departmentIdentifier);
            }

            {
                var count = SaveChanges();
                OnRefreshed(count);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        #endregion

        #region Database operations

        public int SaveChanges()
        {
            var count = 0;

            foreach (DataListItem item in DepartmentsRepeater.Items)
            {
                var chk = (ICheckBoxControl)item.FindControl("IsDepartmentSelected");
                if (chk.Checked)
                    count++;
            }

            if (ChangedList.Count <= 0)
                return count;

            if (AchievementIdentifier.HasValue)
            {
                var insertList = new List<TAchievementDepartment>();
                var deleteList = new List<TAchievementDepartment>();

                foreach (DataListItem item in DepartmentsRepeater.Items)
                {
                    var hf = (ITextControl)item.FindControl("DepartmentIdentifier");
                    var departmentIdentifier = Guid.Parse(hf.Text);

                    if (ChangedList.Contains(departmentIdentifier))
                    {
                        var chk = (ICheckBoxControl)item.FindControl("IsDepartmentSelected");

                        var entity = TAchievementDepartmentSearch.SelectFirst(x => x.DepartmentIdentifier == departmentIdentifier && x.AchievementIdentifier == AchievementIdentifier.Value);

                        if (entity != null)
                        {
                            if (!chk.Checked)
                                deleteList.Add(entity);
                        }
                        else
                        {
                            if (chk.Checked)
                                insertList.Add(new TAchievementDepartment { DepartmentIdentifier = departmentIdentifier, AchievementIdentifier = AchievementIdentifier.Value });
                        }
                    }
                }

                TAchievementDepartmentStore.Delete(deleteList);
                TAchievementDepartmentStore.Insert(insertList);
            }

            ChangedList.Clear();

            return count;
        }

        #endregion
    }
}