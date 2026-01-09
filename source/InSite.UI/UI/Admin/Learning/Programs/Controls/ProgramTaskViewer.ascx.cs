using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Records.Programs.Controls
{
    public partial class ProgramTaskViewer : BaseUserControl
    {
        #region Properties
        private bool AutoPostBack
        {
            get => ((bool?)ViewState[nameof(AutoPostBack)]) ?? false;
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public bool Disabled
        {
            get => ((bool?)ViewState[nameof(Disabled)]) ?? false;
            set => ViewState[nameof(Disabled)] = value;
        }

        public string HelperMsg
        {
            get => (string)ViewState[nameof(HelperMsg)];
            set => ViewState[nameof(HelperMsg)] = value;
        }
        public string ObjectType
        {
            get => (string)ViewState[nameof(ObjectType)];
            set => ViewState[nameof(ObjectType)] = value;
        }

        public List<TTask> ProgramTasks
        {
            get => (List<TTask>)ViewState[nameof(ProgramTasks)];
            set => ViewState[nameof(ProgramTasks)] = value;
        }

        private Guid? ProgramIdentifier
        {
            get => (Guid?)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }
        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            TaskRepeater.ItemCreated += TaskRepeater_ItemCreated;
            TaskRepeater.ItemDataBound += TaskRepeater_ItemDataBound;
        }

        private void TaskRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!AutoPostBack)
                return;

            if (!IsContentItem(e))
                return;

            var childTaskRepeater = (Repeater)e.Item.FindControl("ChildTaskRepeater");
            childTaskRepeater.ItemDataBound += ChildTaskRepeater_ItemDataBound;
        }

        private void TaskRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (ProgramTaskItem)e.Item.DataItem;

            var name = e.Item.FindControl("TaskName");
            ((ITextControl)name).Text = item.TaskName;

            SetTaskEditLinkVisibility(name, item.ObjectType.ToString());

            var identifier = (HiddenField)e.Item.FindControl("TaskIdentifier");

            if (ProgramIdentifier.HasValue)
                identifier.Value = item.ObjectIdentifier.ToString();
            else
                identifier.Value = item.ProgramIdentifier.ToString();

            var objectType = (HiddenField)e.Item.FindControl("ObjectType");
            objectType.Value = item.ObjectType.ToString();

            if (item.ObjectType == "Assessment" && item.IsSelected)
                BindChildTaskItemRepeater(e.Item, (ProgramTaskItem)e.Item.DataItem);
        }

        private void ChildTaskRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (ProgramTaskItem)e.Item.DataItem;

            var name = (ITextControl)e.Item.FindControl("ChildTaskName");
            name.Text = item.TaskName;

            var editLink = (IconLink)e.Item.FindControl("EditLink");
            editLink.Visible = item.IsSelected;

            var identifier = (HiddenField)e.Item.FindControl("ChildTaskIdentifier");

            if (ProgramIdentifier.HasValue)
                identifier.Value = item.ObjectIdentifier.ToString();
            else
                identifier.Value = item.ProgramIdentifier.ToString();
        }

        private void BindChildTaskItemRepeater(RepeaterItem item, ProgramTaskItem parentTask)
        {
            var childTaskRepeater = (Repeater)item.FindControl("ChildTaskRepeater");

            var childItems = ServiceLocator.BankSearch.GetForms(new QBankFormFilter()
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                IncludeFormStatus = "Published",
                BankIdentifier = parentTask.ObjectIdentifier
            }).Select(x => new ProgramTaskItem()
            {
                ProgramIdentifier = ProgramIdentifier.Value,
                TaskIdentifier = ProgramTasks.FirstOrDefault(y => y.ObjectIdentifier == x.FormIdentifier) != null ?
                    ProgramTasks.FirstOrDefault(y => y.ObjectIdentifier == x.FormIdentifier).TaskIdentifier : Guid.Empty,
                ObjectIdentifier = x.FormIdentifier,
                ObjectType = "AssessmentForm",
                TaskName = x.FormName,
                IsSelected = ProgramTasks.Any(y => y.ObjectIdentifier == x.FormIdentifier)
            }).ToList();

            childTaskRepeater.DataSource = childItems.Where(x => x.IsSelected).ToList();
            childTaskRepeater.DataBind();
        }

        public void BindModelToControls(Guid? programId, bool includeEnterpriseTasks = false)
        {
            ProgramIdentifier = programId;
            AutoPostBack = true;

            var (programTasks, items) = ProgramHelper.GetTasksAndItems(programId, ObjectType, Organization.OrganizationIdentifier, (includeEnterpriseTasks ? Organization.ParentOrganizationIdentifier : null));

            ProgramTasks = programTasks;

            TaskRepeater.DataSource = items.OrderBy(x => x.TaskName);
            TaskRepeater.DataBind();

            TaskSearchLink.NavigateUrl = "/ui/admin/records/achievements/search";
            TaskSearchLink.Visible = false;

            TaskLabel.InnerText = ProgramHelper.GenerateTitle(ObjectType);

            TaskRepeater.Visible = items.Count > 0;
            NoTasks.Visible = items.Count == 0;

            HelperMsgPlaceholder.Visible = HelperMsg.HasValue() && !NoTasks.Visible;
            HelperMsgText.Text = HelperMsg;
        }

        #region Helper Methods

        private static void SetTaskEditLinkVisibility(Control item, string type)
        {
            var editLink = (IconLink)item.Parent.FindControl("EditLink");

            if (type.Equals("Assessment"))
                editLink.Visible = false;
        }

        #endregion
    }
}