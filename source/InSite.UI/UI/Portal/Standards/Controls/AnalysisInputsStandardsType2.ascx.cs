using System;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class AnalysisInputsStandardsType2 : BaseUserControl
    {
        #region Properties

        public Guid? StandardIdentifier1 => StandardSelector1.Value;

        public string StandardType2 => StandardTypeSelector2.Value;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StandardTypeSelector1.AutoPostBack = true;
            StandardTypeSelector1.ValueChanged += StandardTypeSelector1_ValueChanged;
        }

        #endregion

        #region Event handlers

        private void StandardTypeSelector1_ValueChanged(object sender, EventArgs e) => OnStandardType1Selected();

        private void OnStandardType1Selected()
        {
            var hasValue = StandardTypeSelector1.Value.IsNotEmpty();

            StandardSelector1.Value = null;
            StandardSelector1.Enabled = hasValue;

            if (!hasValue)
                return;

            StandardSelector1.Filter.StandardTypes = new[]
            {
                StandardTypeSelector1.Value
            };
        }

        #endregion

        #region Methods (public)

        public void SetDefaultValues()
        {
            StandardTypeSelector1.ClearSelection();
            StandardTypeSelector2.ClearSelection();

            OnStandardType1Selected();
        }

        #endregion
    }
}