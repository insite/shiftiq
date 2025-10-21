using System;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public partial class ConfigurationDetails : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SectionsAsTabsEnabled.AutoPostBack = true;
            SectionsAsTabsEnabled.ValueChanged += (s, a) => OnSectionsAsTabsEnabledChanged();

            TabNavigationEnabled.AutoPostBack = true;
            TabNavigationEnabled.ValueChanged += (s, a) => OnTabNavigationEnabledChanged();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            TabTimeLimit.LoadItems(
                SpecificationTabTimeLimit.Disabled,
                SpecificationTabTimeLimit.SomeTabs,
                SpecificationTabTimeLimit.AllTabs);
        }

        private void OnSectionsAsTabsEnabledChanged()
        {
            TabNavigationField.Visible = SectionsAsTabsEnabled.ValueAsBoolean == true;
            TabNavigationEnabled.ValueAsBoolean = true;

            OnTabNavigationEnabledChanged();
        }

        private void OnTabNavigationEnabledChanged()
        {
            var sectionsAsTabsEnabled = SectionsAsTabsEnabled.ValueAsBoolean == true;
            var tabNavigationEnabled = TabNavigationEnabled.ValueAsBoolean == true;

            SingleQuestionPerTabField.Visible = sectionsAsTabsEnabled && !tabNavigationEnabled;
            SingleQuestionPerTabEnabled.ValueAsBoolean = false;

            TabTimeLimitField.Visible = sectionsAsTabsEnabled && !tabNavigationEnabled;
        }

        public void SetDefaultInputValues()
        {
            var spec = new Specification
            {
                Consequence = Shift.Constant.ConsequenceType.Medium,
                FormLimit = 0,
                QuestionLimit = 0
            };

            SetInputValues(spec);
        }

        public void SetInputValues(Specification spec)
        {
            ConsequenceType.SelectedValue = spec.Consequence.GetName();
            FormLimit.ValueAsInt = spec.FormLimit;
            QuestionLimit.ValueAsInt = spec.QuestionLimit;

            var isStaticSpec = spec.Type == SpecificationType.Static;

            ScenarioFields.Visible = isStaticSpec;

            if (isStaticSpec)
            {
                SectionsAsTabsEnabled.ValueAsBoolean = spec.SectionsAsTabsEnabled;

                OnSectionsAsTabsEnabledChanged();

                TabNavigationEnabled.ValueAsBoolean = spec.TabNavigationEnabled;

                OnTabNavigationEnabledChanged();

                SingleQuestionPerTabEnabled.ValueAsBoolean = spec.SingleQuestionPerTabEnabled;
                TabTimeLimit.Value = spec.TabTimeLimit.GetName();
            }
        }

        public void GetInputValues(Specification spec)
        {
            spec.Consequence = ConsequenceType.SelectedValue.ToEnum<ConsequenceType>();
            spec.FormLimit = FormLimit.ValueAsInt.Value;
            spec.QuestionLimit = QuestionLimit.ValueAsInt.Value;
        }

        public bool? GetScenarioEnabled() => ScenarioFields.Visible ? SectionsAsTabsEnabled.ValueAsBoolean.Value : (bool?)null;

        public bool? GetTabNavigationEnabled() => TabNavigationField.Visible ? TabNavigationEnabled.ValueAsBoolean.Value : (bool?)null;

        public bool? GetSingleQuestionPerTabEnabled() => SingleQuestionPerTabField.Visible ? SingleQuestionPerTabEnabled.ValueAsBoolean.Value : (bool?)null;

        public SpecificationTabTimeLimit? GetTabTimeLimit() => TabTimeLimitField.Visible ? TabTimeLimit.Value.ToEnum<SpecificationTabTimeLimit>() : (SpecificationTabTimeLimit?)null;

        internal void SetFormLimit(bool enabled, int formLimit)
        {
            FormLimit.Enabled = enabled;
            FormLimit.ValueAsInt = formLimit;
        }
    }
}