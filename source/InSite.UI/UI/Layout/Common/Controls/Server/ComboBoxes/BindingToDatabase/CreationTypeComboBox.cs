using System.Linq;

using Humanizer;

using Shift.Common;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class CreationTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public override bool AllowBlank => false;

        public string TypeName
        {
            get => ViewState[nameof(TypeName)] as string ?? "Course";
            set => ViewState[nameof(TypeName)] = value;
        }

        public CreationTypeEnum ValueAsEnum
        {
            get => Value.ToEnum(CreationTypeEnum.None);
            set => Value = value.GetName(CreationTypeEnum.None);
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var typeNameLower = TypeName.ToLower();

            list.Add(new ListItem { Value = CreationTypeEnum.One.GetName(), Text = $"One new {typeNameLower}" });
            list.Add(new ListItem { Value = CreationTypeEnum.Duplicate.GetName(), Text = $"Duplicate copy of an existing {typeNameLower}" });
            list.Add(new ListItem { Value = CreationTypeEnum.Outline.GetName(), Text = $"Multiple new {typeNameLower.Pluralize()} from an outline" });
            list.Add(new ListItem { Value = CreationTypeEnum.Upload.GetName(), Text = $"Upload one new {typeNameLower} from a file" });
            list.Add(new ListItem { Value = CreationTypeEnum.Bulk.GetName(), Text = $"Bulk-add multiple new {typeNameLower.Pluralize()}" });

            return list;
        }

        public void SetVisibleOptions(params CreationTypeEnum[] values)
        {
            var strValues = values.Select(x => x.GetName()).ToArray();
            foreach (var option in this.FlattenOptions())
                option.Visible = strValues.Contains(option.Value);

        }

    }
}