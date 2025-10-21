using System;

using InSite.Common.Web.UI;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class AnalysisInputsStandardsType1 : BaseUserControl
    {
        #region Properties

        public Guid? StandardIdentifier1 => StandardSelector1.Value;

        public Guid? StandardIdentifier2 => StandardSelector2.Value;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StandardTypeSelector1.AutoPostBack = true;
            StandardTypeSelector1.ValueChanged += StandardTypeSelector1_ValueChanged;

            StandardTypeSelector2.AutoPostBack = true;
            StandardTypeSelector2.ValueChanged += StandardTypeSelector2_ValueChanged;
        }

        #endregion

        #region Event handlers

        private void StandardTypeSelector1_ValueChanged(object sender, EventArgs e)
        {
            var hasValue = !string.IsNullOrEmpty(StandardTypeSelector1.Value);

            StandardSelector1.Value = null;
            StandardSelector1.Enabled = hasValue;

            if (!hasValue)
                return;

            StandardSelector1.Filter.StandardTypes = new[]
            {
                StandardTypeSelector1.Value
            };
        }

        private void StandardTypeSelector2_ValueChanged(object sender, EventArgs e)
        {
            var hasValue = !string.IsNullOrEmpty(StandardTypeSelector2.Value);

            StandardSelector2.Value = null;
            StandardSelector2.Enabled = hasValue;

            if (!hasValue)
                return;

            StandardSelector2.Filter.StandardTypes = new[]
            {
                StandardTypeSelector2.Value
            };
        }

        private void OnStandardType1Selected()
        {
            var hasValue = !string.IsNullOrEmpty(StandardTypeSelector1.Value);

            StandardSelector1.Value = null;
            StandardSelector1.Enabled = hasValue;

            if (!hasValue)
                return;

            StandardSelector1.Filter.StandardTypes = new[]
            {
                StandardTypeSelector1.Value
            };
        }

        private void OnStandardType2Selected()
        {
            var hasValue = !string.IsNullOrEmpty(StandardTypeSelector2.Value);

            StandardSelector2.Value = null;
            StandardSelector2.Enabled = hasValue;

            if (!hasValue)
                return;

            StandardSelector2.Filter.StandardTypes = new[]
            {
                StandardTypeSelector2.Value
            };
        }

        #endregion

        #region Methods (public)

        public void SetDefaultValues()
        {
            StandardTypeSelector1.ClearSelection();
            StandardTypeSelector2.ClearSelection();

            OnStandardType1Selected();
            OnStandardType2Selected();
        }

        #endregion
    }
}