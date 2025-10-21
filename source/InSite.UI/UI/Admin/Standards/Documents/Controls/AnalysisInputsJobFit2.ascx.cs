using System;
using System.Linq;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class AnalysisInputsJobFit2 : BaseUserControl
    {
        #region Events

        public event AnalysisHelper.UpdateEventHandler<AnalysisHelper.ReportDataJobFit2> Update;

        private void OnUpdate(AnalysisHelper.ReportDataJobFit2 data)
        {
            Update?.Invoke(this, new AnalysisHelper.UpdateEventArgs<AnalysisHelper.ReportDataJobFit2>(data));
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

            ProfileSelector.Filter.StandardTypes = new[] { StandardType.Profile };
        }

        #endregion

        #region Event handlers

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var lang = "en";
            var profile = StandardSearch.Select(ProfileSelector.Value.Value);
            var nosDocs = StandardSearch
                .Select(x => x.OrganizationIdentifier == CurrentSessionState.Identity.Organization.Identifier
                          && x.StandardType == StandardType.Document
                          && x.DocumentType == DocumentType.NationalOccupationStandard)
                .Select(x => new
                {
                    Document = x,
                    Gacs = AnalysisHelper.GetDocumentGacs(x.StandardIdentifier, lang)
                })
                .ToArray();

            var profileCompetencies = AnalysisHelper.GetProfileCompetencies(profile.StandardIdentifier)
                .Select(x => x.StandardIdentifier)
                .ToHashSet();

            var data = new AnalysisHelper.ReportDataJobFit2
            {
                Title = $"Job Fit Analysis for Profile #{profile.AssetNumber}: {GetTitle(profile)}",
                NosReports = nosDocs
                    .Select(nos => AnalysisHelper.GetReportDataJobFit1(nos.Document, nos.Gacs, profileCompetencies, lang, Translate))
                    .OrderByDescending(x => x.OverlapValue).ThenBy(x => x.Title)
                    .ToArray()
            };

            OnUpdate(data);

            string GetTitle(Standard entity)
            {
                return ServiceLocator.ContentSearch.SelectContainer(entity.StandardIdentifier, ContentLabel.Title, lang)?.ContentText;
            }
        }

        #endregion

        #region Methods (public)

        public void SetDefaultValues()
        {
            ProfileSelector.Value = null;
        }

        #endregion
    }
}