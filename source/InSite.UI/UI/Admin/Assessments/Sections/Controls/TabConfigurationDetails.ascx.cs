using System;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Sections.Controls
{
    public partial class TabConfigurationDetails : BaseUserControl
    {
        private SpecificationTabTimeLimit TabTimeLimit
        {
            get => (SpecificationTabTimeLimit)(ViewState[nameof(TabTimeLimit)] ?? SpecificationTabTimeLimit.Disabled);
            set => ViewState[nameof(TabTimeLimit)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BreakTimer.ValueChanged += (s, args) => SetFieldsVisibility();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            TimerType.LoadItems(
                FormSectionTimeType.Optional,
                FormSectionTimeType.Enforced);
        }

        public void SetValues(SectionTabConfiguration config, SpecificationTabTimeLimit tabTimeLimit)
        {
            TabTimeLimit = tabTimeLimit;

            WarningOnNextTab.ValueAsBoolean = config.WarningOnNextTabEnabled;
            BreakTimer.ValueAsBoolean = config.BreakTimerEnabled;
            BreakTimer.AutoPostBack = tabTimeLimit == SpecificationTabTimeLimit.SomeTabs;
            TimeLimit.ValueAsInt = config.TimeLimit;
            TimerType.Value = config.TimerType.GetName();

            SetFieldsVisibility();
        }

        public void GetValues(SectionTabConfiguration config)
        {
            config.WarningOnNextTabEnabled = WarningOnNextTab.ValueAsBoolean.Value;
            config.BreakTimerEnabled = BreakTimer.ValueAsBoolean.Value;
            config.TimeLimit = TimeLimit.ValueAsInt ?? 0;
            config.TimerType = TimerType.Value.ToEnum<FormSectionTimeType>();
        }

        private void SetFieldsVisibility()
        {
            var limitSomeTabs = TabTimeLimit == SpecificationTabTimeLimit.SomeTabs;
            var limitAllTabs = TabTimeLimit == SpecificationTabTimeLimit.AllTabs;
            var breakTimerEnabled = BreakTimer.ValueAsBoolean.Value;

            BreakTimerField.Visible = limitSomeTabs || limitAllTabs;
            TimeLimitField.Visible = limitAllTabs || limitSomeTabs && breakTimerEnabled;
            TimerTypeField.Visible = limitAllTabs || limitSomeTabs && breakTimerEnabled;
        }
    }
}
