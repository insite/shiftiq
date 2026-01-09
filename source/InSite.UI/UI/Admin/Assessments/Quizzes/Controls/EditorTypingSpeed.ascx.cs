using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Quizzes.Controls
{
    public partial class EditorTypingSpeed : BaseUserControl
    {
        private List<string> TextItems
        {
            get => (List<string>)ViewState[nameof(TextItems)];
            set => ViewState[nameof(TextItems)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddItemButton.Click += AddItemButton_Click;

            ItemRepeater.DataBinding += ItemRepeater_DataBinding;
            ItemRepeater.ItemCreated += ItemRepeater_ItemCreated;
            ItemRepeater.ItemCommand += ItemRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                ReadItems();
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            for (var i = 0; i < ItemRepeater.Items.Count; i++)
                SetupItemValidator(ItemRepeater.Items[i], groupName);
        }

        private void SetupItemValidator(RepeaterItem item, string groupName)
        {
            var validator = (BaseValidator)item.FindControl("TextRequiredValidator");
            validator.ValidationGroup = groupName;
        }

        private void ReadItems()
        {
            if (TextItems == null)
                return;

            var count = ItemRepeater.Items.Count;
            for (var i = 0; i < count; i++)
            {
                var text = (ITextBox)ItemRepeater.Items[i].FindControl("Text");

                TextItems[i] = text.Text;
            }
        }

        private void ItemRepeater_DataBinding(object sender, EventArgs e)
        {
            ItemRepeater.DataSource = TextItems.Select((x, i) => new
            {
                Letter = Calculator.ToBase26(i + 1),
                Text = x
            });
        }

        private void ItemRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupItemValidator(e.Item, ValidationGroup);
        }

        private void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                TextItems.RemoveAt(e.Item.ItemIndex);
                EnsureHasItems();
                ItemRepeater.DataBind();
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            TextItems.Add(string.Empty);
            ItemRepeater.DataBind();
        }

        private void EnsureHasItems()
        {
            if (TextItems.Count == 0)
                TextItems.Add(string.Empty);
        }

        public void SetData(IEnumerable<string> items)
        {
            TextItems = new List<string>(items);
            EnsureHasItems();
            ItemRepeater.DataBind();
        }

        public IEnumerable<string> GetData()
        {
            return TextItems.ToArray();
        }
    }
}