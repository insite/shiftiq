using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

namespace InSite.Admin.Courses.Activities.Controls
{
    public partial class CompetenciesSelectorRepeater : BaseUserControl
    {
        #region Classes

        public interface IInputItem
        {
            int Asset { get; }
            string Code { get; }
            string Title { get; }
            string TypeIcon { get; }
            string TypeName { get; }
            bool IsSelected { get; }
            bool IsCompetency { get; }
            bool HasChildren { get; }
            IEnumerable<IInputItem> Children { get; }
        }

        public interface IOutputItem
        {
            int Index { get; }
            bool IsSelected { get; }
            CompetenciesSelectorRepeater InnerRepeater { get; }
        }

        private class OutputItem : IOutputItem
        {
            #region Properties

            public int Index => _item.ItemIndex;

            public bool IsSelected => _checkBox.Checked;

            public CompetenciesSelectorRepeater InnerRepeater => (CompetenciesSelectorRepeater)_container.GetControl();

            #endregion

            #region Fields

            private RepeaterItem _item;
            private ICheckBoxControl _checkBox;
            private DynamicControl _container;

            #endregion

            #region Construction

            public OutputItem(RepeaterItem item)
            {
                _item = item;
                _checkBox = (ICheckBoxControl)_item.FindControl("IsSelected");
                _container = (DynamicControl)_item.FindControl("Container");
            }

            #endregion
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        public void LoadData(IEnumerable<IInputItem> data)
        {
            Repeater.DataSource = data;
            Repeater.DataBind();
        }

        public IEnumerable<IOutputItem> EnumerateItems()
        {
            foreach (RepeaterItem item in Repeater.Items)
                yield return new OutputItem(item);
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var info = (IInputItem)e.Item.DataItem;
            var container = (DynamicControl)e.Item.FindControl("Container");

            if (info.HasChildren)
            {
                var repeater = (CompetenciesSelectorRepeater)container.LoadControl("~/UI/Admin/Courses/Activities/Controls/CompetenciesSelectorRepeater.ascx");
                repeater.LoadData(info.Children);
            }
            else
            {
                container.UnloadControl();
            }
        }
    }
}