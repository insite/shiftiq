using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Standards.Standards.Controls
{
    public partial class UploadFieldRepeater : BaseUserControl
    {
        public class FieldInfo
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }
            public bool Required { get; set; }
            public IEnumerable<Shift.Common.ListItem> DataSource { get; set; }
        }

        private Dictionary<string, int> Items
        {
            get => (Dictionary<string, int>)ViewState[nameof(Items)];
            set => ViewState[nameof(Items)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FieldRepeater.DataBinding += FieldRepeater_DataBinding;
            FieldRepeater.ItemCreated += FieldRepeater_ItemCreated;
            FieldRepeater.ItemDataBound += FieldRepeater_ItemDataBound;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            for (var i = 0; i < FieldRepeater.Items.Count; i++)
                SetupFieldValidationGroup(FieldRepeater.Items[i], groupName);
        }

        private void SetupFieldValidationGroup(RepeaterItem item, string groupName)
        {
            var validator = (RequiredValidator)item.FindControl("Validator");
            validator.ValidationGroup = ValidationGroup;
        }

        private void FieldRepeater_DataBinding(object sender, EventArgs e)
        {
            Items = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        private void FieldRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupFieldValidationGroup(e.Item, ValidationGroup);
        }

        private void FieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (FieldInfo)e.Item.DataItem;

            Items.Add(dataItem.Name, e.Item.ItemIndex);

            var selector = (ComboBox)e.Item.FindControl("Selector");
            selector.LoadItems(dataItem.DataSource);
            selector.Value = dataItem.Value;

            var validator = (RequiredValidator)e.Item.FindControl("Validator");
            validator.FieldName = dataItem.Title;
            validator.Visible = dataItem.Required;
        }

        public void LoadData(IEnumerable<FieldInfo> fields)
        {
            FieldRepeater.DataSource = fields;
            FieldRepeater.DataBind();
        }

        public string GetSelectorValue(string fieldName)
        {
            if (!Items.TryGetValue(fieldName, out var fieldIndex))
                throw new ApplicationError("Field not found: " + fieldName);

            var selector = (ComboBox)FieldRepeater.Items[fieldIndex].FindControl("Selector");

            return selector.Value;
        }
    }
}