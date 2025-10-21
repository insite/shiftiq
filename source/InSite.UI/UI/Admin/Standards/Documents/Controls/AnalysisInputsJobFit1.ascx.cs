using System;
using System.Linq;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class AnalysisInputsJobFit1 : BaseUserControl
    {
        #region Events

        public event AnalysisHelper.UpdateEventHandler<AnalysisHelper.ReportDataJobFit1> Update;

        private void OnUpdate(AnalysisHelper.ReportDataJobFit1 data)
        {
            Update?.Invoke(this, new AnalysisHelper.UpdateEventArgs<AnalysisHelper.ReportDataJobFit1>(data));
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnalyzeButton.Click += AnalyzeButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ProfileSelector.Filter.StandardTypes = new[] { StandardType.Document };
            ProfileSelector.Filter.DocumentType = new[] { DocumentType.CustomizedOccupationProfile };

            NosSelector.Filter.StandardTypes = new[] { StandardType.Document };
            NosSelector.Filter.DocumentType = new[] { DocumentType.NationalOccupationStandard };
        }

        #endregion

        #region Event handlers

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var lang = "en";
            var nos = StandardSearch.Select(NosSelector.Value.Value);
            var nosGacs = AnalysisHelper.GetDocumentGacs(nos.StandardIdentifier, lang);
            var profileCompetencies = AnalysisHelper.GetProfileCompetencies(ProfileSelector.Value.Value)
                .Select(x => x.StandardIdentifier)
                .ToHashSet();

            var data = AnalysisHelper.GetReportDataJobFit1(nos, nosGacs, profileCompetencies, lang, Translate);

            OnUpdate(data);
        }

        #endregion

        #region Methods (public)

        public void SetDefaultValues()
        {
            ProfileSelector.Value = null;
            NosSelector.Value = null;
        }

        #endregion
    }
}