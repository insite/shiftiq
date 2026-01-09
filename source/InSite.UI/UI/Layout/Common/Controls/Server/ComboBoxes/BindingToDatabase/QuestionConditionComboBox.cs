using System;
using System.Collections.ObjectModel;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class QuestionConditionComboBox : ComboBox
    {
        public static readonly ReadOnlyCollection<string> Conditions = Array.AsReadOnly(new[]
        {
            "Copy",
            "Edit",
            "New",
            "Purge",
            "Surplus",
            "Unassigned"
        });

        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var condition in Conditions)
                list.Add(condition, condition);

            return list;
        }
    }

    public class QuestionConditionMultiComboBox : MultiComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var condition in QuestionConditionComboBox.Conditions)
                list.Add(condition, condition);

            return list;
        }
    }
}