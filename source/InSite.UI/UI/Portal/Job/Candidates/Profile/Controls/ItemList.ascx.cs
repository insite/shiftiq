using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ItemList : UserControl
    {
        public List<string> GetItems(bool excludeEmpty)
        {
            var result = new List<string>();

            foreach (RepeaterItem item in ItemRepeater.Items)
            {
                var text = ((ITextBox)item.FindControl("Description")).Text;

                if (excludeEmpty)
                {
                    text = text.TrimEnd();

                    if (!string.IsNullOrEmpty(text))
                        result.Add(text);
                }
                else
                    result.Add(text);
            }

            return result;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemsValidator.ServerValidate += ItemsValidator_ServerValidate;

            AddButton.Click += AddButton_Click;

            ItemRepeater.ItemCommand += ItemRepeater_ItemCommand;
        }

        private void ItemsValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var count = GetItems(true).Count;

            args.IsValid = count >= 2 && count <= 10;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var items = GetItems(false);

            items.Add(Description.Text.Trim());

            LoadData(items);

            Description.Text = null;
        }

        private void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var items = GetItems(false);

            items.RemoveAt(e.Item.ItemIndex);

            LoadData(items);
        }

        public void LoadData(List<string> items)
        {
            ItemRepeater.DataSource = items;
            ItemRepeater.DataBind();
        }
    }
}