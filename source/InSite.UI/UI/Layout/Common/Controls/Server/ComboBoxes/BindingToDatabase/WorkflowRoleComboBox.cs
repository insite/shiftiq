using System;
using System.Web.UI.WebControls;

namespace InSite.Common.Web.UI
{
    public class WorkflowRoleComboBox : System.Web.UI.WebControls.RadioButtonList
    {
        public WorkflowRoleComboBox()
        {
            RepeatDirection = RepeatDirection.Horizontal;
            RepeatLayout = RepeatLayout.Flow;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Items.Add(new ListItem("Add Group", "Group") { Selected = true });
            Items.Add(new ListItem("Add People", "Person"));
            Items.Add(new ListItem("Add Group and People", "GroupAndPerson"));
        }
    }
}