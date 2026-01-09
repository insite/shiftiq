using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class GradeTreeViewNode : UserControl
    {
        [Serializable]
        public class Grade
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string ClassName { get; set; }
            public DateTimeOffset? ClassStartDate { get; set; }
            public DateTimeOffset? ClassEndDate { get; set; }
            public int Level { get; set; }
            public string ScoreValue { get; set; }
            public string Comment { get; set; }

            public List<Grade> Children { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NodeRepeater.ItemDataBound += NodeRepeater_ItemDataBound;
        }

        private void NodeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var grade = (Grade)e.Item.DataItem;
            var hasChildren = grade.Children.IsNotEmpty();

            var childNodes = e.Item.FindControl("ChildNodes");
            childNodes.Visible = hasChildren;

            if (hasChildren)
            {
                var treeViewNode = (GradeTreeViewNode)LoadControl("GradeTreeViewNode.ascx");
                childNodes.Controls.Add(treeViewNode);

                treeViewNode.LoadData(grade.Children);
            }
        }

        public void LoadData(List<Grade> grades)
        {
            NodeRepeater.DataSource = grades;
            NodeRepeater.DataBind();
        }

        protected static string GetLocalTime(object item)
        {
            if (item == null)
                return null;

            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone, nullValue: string.Empty);
        }
    }
}