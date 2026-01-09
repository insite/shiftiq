using System;

using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class AnalysisInputsStandardsType3 : BaseUserControl
    {
        #region Properties

        public Guid? CompetencyKey => CompetencySelector.Value;

        public string StandardType => StandardTypeSelector.Value;

        #endregion

        #region Initialization

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            CompetencySelector.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Competency };
            CompetencySelector.Value = null;
        }

        #endregion

        #region Methods (public)

        public void SetDefaultValues()
        {
            CompetencySelector.Value = null;
            StandardTypeSelector.ClearSelection();
        }

        #endregion
    }
}