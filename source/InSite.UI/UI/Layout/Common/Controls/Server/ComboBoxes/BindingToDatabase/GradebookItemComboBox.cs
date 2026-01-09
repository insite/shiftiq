using System;
using System.Collections.Generic;

using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class GradebookItemComboBox : ComboBox
    {
        public Guid? GradebookIdentifier
        {
            get => (Guid?)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        public bool EnableOnlyWithChildScoreItems
        {
            get => (bool?)ViewState[nameof(EnableOnlyWithChildScoreItems)] ?? false;
            set => ViewState[nameof(EnableOnlyWithChildScoreItems)] = value;
        }

        public Guid? DisableItemAndSubTree
        {
            get => (Guid?)ViewState[nameof(DisableItemAndSubTree)];
            set => ViewState[nameof(DisableItemAndSubTree)] = value;
        }

        public Guid? DisableItem
        {
            get => (Guid?)ViewState[nameof(DisableItem)];
            set => ViewState[nameof(DisableItem)] = value;
        }

        public bool DisableScoreItems
        {
            get => ViewState[nameof(DisableScoreItems)] as bool? ?? false;
            set => ViewState[nameof(DisableScoreItems)] = value;
        }

        public GradebookItemComboBox()
        {
            DropDown.Size = 15;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var dataGradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier ?? Guid.Empty);
            if (dataGradebook != null)
            {
                var root = dataGradebook.RootItems;
                AddItems(list, root, 0, true);
            }

            return list;
        }

        protected override ComboBoxItem LoadItem(ListItem item)
        {
            return new ComboBoxOption
            {
                Value = item.Value,
                Text = item.Text,
                Html = item.Description,
                Enabled = item.Enabled != false
            };
        }

        private void AddItems(ListItemArray output, List<GradeItem> input, int level, bool enabled)
        {
            if (input.IsEmpty())
                return;

            foreach (var inputItem in input)
            {
                var paddingLeft = level * 10;
                var description = $"<div style='padding-left:{paddingLeft}px;'>{inputItem.Name}</div>";

                var itemEnabled = enabled
                    && (
                        !EnableOnlyWithChildScoreItems
                        || inputItem.Children != null && inputItem.Children.Find(x => x.Type == GradeItemType.Score) != null
                    )
                    && inputItem.Identifier != DisableItemAndSubTree
                    && inputItem.Identifier != DisableItem
                    && (
                        !DisableScoreItems
                        || inputItem.Type != GradeItemType.Score
                        || inputItem.Format != GradeItemFormat.Text
                    )
                    ;

                if (itemEnabled || (inputItem.Children.IsNotEmpty()))
                {
                    output.Add(inputItem.Identifier.ToString(), inputItem.Name, description).Enabled = itemEnabled;

                    AddItems(output, inputItem.Children, level + 1, enabled && inputItem.Identifier != DisableItemAndSubTree);
                }
            }
        }
    }
}