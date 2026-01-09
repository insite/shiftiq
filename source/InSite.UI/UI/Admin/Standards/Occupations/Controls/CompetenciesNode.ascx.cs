using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Occupations.Utilities.Competencies;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Standards.Occupations.Controls
{
    public partial class CompetenciesNode : BaseUserControl
    {
        #region Events

        public event CommandEventHandler Command;

        #endregion

        #region Properties

        public Guid StandardIdentifier
        {
            get => (Guid)ViewState[nameof(StandardIdentifier)];
            private set => ViewState[nameof(StandardIdentifier)] = value;
        }

        public int Depth
        {
            get => (int)ViewState[nameof(Depth)];
            private set => ViewState[nameof(Depth)] = value;
        }

        private List<Guid> LeafDataKeys
        {
            get => (List<Guid>)ViewState[nameof(LeafDataKeys)];
            set => ViewState[nameof(LeafDataKeys)] = value;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(CompetenciesNode).FullName;

            LeafRepeater.DataBinding += LeafRepeater_DataBinding;
            LeafRepeater.ItemDataBound += LeafRepeater_ItemDataBound;
            LeafRepeater.ItemCommand += LeafRepeater_ItemCommand;

            NodeRepeater.ItemCreated += NodeRepeater_ItemCreated;
            NodeRepeater.ItemDataBound += NodeRepeater_ItemDataBound;
        }

        private void LeafRepeater_DataBinding(object sender, EventArgs e)
        {
            LeafDataKeys = new List<Guid>();
        }

        private void LeafRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            LeafDataKeys.Add((Guid)DataBinder.Eval(e.Item.DataItem, "StandardIdentifier"));
        }

        private void LeafRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Command?.Invoke(this, new CommandEventArgs(e.CommandName, LeafDataKeys[e.Item.ItemIndex]));
        }

        private void NodeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var container = (DynamicControl)e.Item.FindControl("Container");
            container.ControlAdded += Container_ControlAdded;
        }

        private void Container_ControlAdded(object sender, EventArgs e)
        {
            var node = (CompetenciesNode)((DynamicControl)sender).GetControl();
            node.Command += Node_Command;
        }

        private void Node_Command(object sender, CommandEventArgs e)
        {
            Command?.Invoke(sender, e);
        }

        private void NodeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var standard = (StandardInfo)e.Item.DataItem;

            var container = (DynamicControl)e.Item.FindControl("Container");
            var node = (CompetenciesNode)container.LoadControl("~/UI/Admin/Standards/Occupations/Controls/CompetenciesNode.ascx");
            node.LoadData(standard, Depth + 1);
        }

        public void GetSelectedCompetencies(List<Guid> competencies)
        {
            var allLeavesDeleted = true;

            foreach (RepeaterItem item in LeafRepeater.Items)
            {
                var isSelectedCheckbox = (ICheckBoxControl)item.FindControl("IsSelected");

                if (isSelectedCheckbox.Checked)
                {
                    var id = LeafDataKeys[item.ItemIndex];
                    competencies.Add(id);
                }
                else
                    allLeavesDeleted = false;
            }

            if (allLeavesDeleted)
                competencies.Add(StandardIdentifier);

            foreach (RepeaterItem item in NodeRepeater.Items)
            {
                var container = (DynamicControl)item.FindControl("Container");
                var node = (CompetenciesNode)container.GetControl();

                node.GetSelectedCompetencies(competencies);
            }
        }

        public void LoadData(StandardInfo standard)
        {
            LoadData(standard, 1);
        }

        private void LoadData(StandardInfo standard, int depth)
        {
            StandardIdentifier = standard.StandardIdentifier;
            Depth = depth;

            var level = depth;
            if (level > 6)
                level = 6;

            NodeTitle.Text = $"<h{level} class='h4 mb-1'>{HttpUtility.HtmlEncode(standard.Title)}</h{level}>";

            var leaves = new List<StandardInfo>();
            var nodes = new List<StandardInfo>();

            foreach (var child in standard.Children)
            {
                if (child.Children.Count == 0)
                {
                    if (child.StandardType == StandardType.Competency)
                        leaves.Add(child);
                }
                else
                    nodes.Add(child);
            }

            LeafRepeater.Visible = leaves.Count > 0;
            LeafRepeater.DataSource = leaves.Select(c => new
            {
                c.StandardIdentifier,
                c.Title,
                c.AssetNumber,
                c.Code,
            });
            LeafRepeater.DataBind();

            NodeRepeater.Visible = nodes.Count > 0;
            NodeRepeater.DataSource = nodes;
            NodeRepeater.DataBind();
        }
    }
}