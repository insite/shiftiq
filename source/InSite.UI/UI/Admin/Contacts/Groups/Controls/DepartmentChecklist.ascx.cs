using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common.Events;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class DepartmentChecklist : BaseUserControl
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

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(achievementIdentifier);

            var canAdd = false;
            if (achievement != null)
                canAdd = TAchievementDepartmentSearch.IsPermittedOwner(achievement.OrganizationIdentifier, Organization.Identifier);

            var data = VCmdsAchievementSearch.SelectForDepartmentChecklist(Organization.Identifier, achievementIdentifier);

            // Another organization's achievement cannot be added to this organization's
            // departments, so list only the departments that already hold it, which still lets an
            // administrator remove those links.
            if (!canAdd)
            {
                for (var i = data.Rows.Count - 1; i >= 0; i--)
                {
                    var isChecked = (bool)data.Rows[i]["IsChecked"];
                    if (!isChecked)
                        data.Rows.RemoveAt(i);
                }
            }

            OwnerNotice.Visible = !canAdd;

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
                var insertCheckBoxes = new Dictionary<Guid, ICheckBoxControl>();
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
                            {
                                insertList.Add(new TAchievementDepartment { DepartmentIdentifier = departmentIdentifier, AchievementIdentifier = AchievementIdentifier.Value });
                                insertCheckBoxes[departmentIdentifier] = chk;
                            }
                        }
                    }
                }

                var permitted = TAchievementDepartmentSearch.SelectPermitted(insertList);

                // Clear the box for links denied by the ownership rule, so the list shows what was
                // saved (regardless of ownership).
                var permittedDepartments = permitted.Select(x => x.DepartmentIdentifier).ToHashSet();
                foreach (var insert in insertList)
                {
                    if (permittedDepartments.Contains(insert.DepartmentIdentifier))
                        continue;

                    insertCheckBoxes[insert.DepartmentIdentifier].Checked = false;
                    count--;
                }

                TAchievementDepartmentStore.Delete(deleteList);
                TAchievementDepartmentStore.Insert(permitted);
            }

            ChangedList.Clear();

            return count;
        }

        #endregion
    }
}
