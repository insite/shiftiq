using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Records;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Items.Controls
{
    public partial class StandardPanel : UserControl
    {
        [Serializable]
        private class StandardItem
        {
            public Guid Identifier { get; set; }
            public string Title { get; set; }
        }

        private List<StandardItem> Standards =>
            (List<StandardItem>)(ViewState[nameof(Standards)] ?? (ViewState[nameof(Standards)] = new List<StandardItem>()));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddButton.Click += AddButton_Click;

            StandardRepeater.ItemCommand += StandardRepeater_ItemCommand;
            StandardRepeater.DataBinding += StandardRepeater_DataBinding;
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var standardIdentifier = Standard.Value.Value;

            if (!Standards.Any(x => x.Identifier == standardIdentifier))
            {
                var standard = StandardSearch.Select(standardIdentifier);

                Standards.Add(new StandardItem
                {
                    Identifier = standard.StandardIdentifier,
                    Title = standard.ContentTitle
                });

                SetStandardComboBoxFilter();

                StandardRepeater.DataBind();
            }
        }

        private void StandardRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var identifier = Guid.Parse((string)e.CommandArgument);
                var standardItem = Standards.Find(x => x.Identifier == identifier);

                if (standardItem != null)
                {
                    Standards.Remove(standardItem);

                    SetStandardComboBoxFilter();

                    StandardRepeater.DataBind();
                }
            }
        }

        private void SetStandardComboBoxFilter()
        {
            Standard.Filter.Exclusions.StandardIdentifier = Standards.Select(x => x.Identifier).ToList();
            Standard.Value = null;
        }

        private void StandardRepeater_DataBinding(object sender, EventArgs e)
        {
            StandardRepeater.DataSource = Standards;
        }

        public void SetInputValue(GradebookState data, Guid? itemKey)
        {
            if (data.Type != GradebookType.Standards && data.Type != GradebookType.ScoresAndStandards)
                return;

            Standards.Clear();

            if (itemKey.HasValue)
            {
                var item = data.FindItem(itemKey.Value);
                if (item.Competencies.IsNotEmpty())
                {
                    foreach (var identifier in item.Competencies)
                    {
                        var standard = StandardSearch.Select(identifier);

                        Standards.Add(new StandardItem
                        {
                            Identifier = standard.StandardIdentifier,
                            Title = standard.ContentTitle
                        });
                    }
                }
            }

            var framework = StandardSearch.Select(data.Framework.Value);

            Standard.Filter.RootStandardIdentifier = framework.StandardIdentifier;

            SetStandardComboBoxFilter();

            StandardRepeater.DataBind();
        }

        public Guid[] GetStandards() => Standards.Count > 0 ? Standards.Select(x => x.Identifier).ToArray() : null;
    }
}