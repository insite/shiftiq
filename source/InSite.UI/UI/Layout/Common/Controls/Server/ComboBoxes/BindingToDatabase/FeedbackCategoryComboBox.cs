using System.ComponentModel;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FeedbackCategoryComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FeedbackCategoryComboBoxSettings Settings { get; }

        public FeedbackCategoryComboBox()
        {
            Settings = new FeedbackCategoryComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    public class FeedbackCategoryMultiComboBox : MultiComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FeedbackCategoryComboBoxSettings Settings { get; }

        public FeedbackCategoryMultiComboBox()
        {
            Settings = new FeedbackCategoryComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FeedbackCategoryComboBoxSettings : StateBagProxy
    {
        #region Properties

        public bool IncludeNoCategory
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public FeedbackCategoryComboBoxSettings(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion

        #region Methods

        public ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            if (IncludeNoCategory)
                list.Add("No Category");

            list.Add("Need More Info");
            list.Add("Poorly Worded");
            list.Add("Poor Diagram");
            list.Add("Multiple Correct Answers");
            list.Add("No Correct Answer");
            list.Add("Typo");
            list.Add("Irrelevant");
            list.Add("Not Taught");
            list.Add("Other");

            return list;
        }


        #endregion
    }
}