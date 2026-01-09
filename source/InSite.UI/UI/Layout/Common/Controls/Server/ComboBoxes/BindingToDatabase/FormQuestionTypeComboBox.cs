using System.Collections.Generic;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class FormQuestionTypeComboBox : ComboBox
    {
        private class GroupItem : ListItem
        {
            public List<ListItem> Items { get; } = new List<ListItem>();

            public GroupItem(string text)
            {
                Text = text;
            }

            public void AddItem(SurveyQuestionType type, string icon)
            {
                Items.Add(new ListItem
                {
                    Value = type.GetName(),
                    Text = type.GetDescription(),
                    Icon = "fas fa-" + icon,
                });
            }
        }

        protected override BindingType ControlBinding => BindingType.Code;

        public SurveyQuestionType? ValueAsEnum
        {
            get => base.Value.IsEmpty() ? (SurveyQuestionType?)null : base.Value.ToEnum<SurveyQuestionType>();
            set => base.Value = value.HasValue ? value.Value.GetName() : null;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            {
                var group = new GroupItem("Special");

                group.AddItem(SurveyQuestionType.BreakQuestion, "question-circle");
                group.AddItem(SurveyQuestionType.BreakPage, "file");
                group.AddItem(SurveyQuestionType.Terminate, "stop-circle");

                list.Add(group);
            }

            {
                var group = new GroupItem("Quantitative");

                group.AddItem(SurveyQuestionType.RadioList, "check-circle");
                group.AddItem(SurveyQuestionType.Selection, "list-alt");
                group.AddItem(SurveyQuestionType.CheckList, "check-square");
                group.AddItem(SurveyQuestionType.Likert, "table");

                list.Add(group);
            }

            {
                var group = new GroupItem("Qualitative");

                group.AddItem(SurveyQuestionType.Comment, "comment");
                group.AddItem(SurveyQuestionType.Date, "calendar-alt");
                group.AddItem(SurveyQuestionType.Number, "hashtag");
                group.AddItem(SurveyQuestionType.Upload, "upload");

                list.Add(group);
            }

            return list;
        }

        protected override ComboBoxItem LoadItem(ListItem item)
        {
            if (item is GroupItem group)
            {
                var optionGroup = new ComboBoxOptionGroup(group.Text);

                foreach (var gi in group.Items)
                    optionGroup.Items.Add((ComboBoxOption)base.LoadItem(gi));

                return optionGroup;
            }
            else
                return base.LoadItem(item);
        }
    }
}