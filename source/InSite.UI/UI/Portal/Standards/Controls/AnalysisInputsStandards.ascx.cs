using System;
using System.Linq;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class AnalysisInputsStandards : BaseUserControl
    {
        #region Events

        public event AnalysisHelper.UpdateEventHandler<AnalysisHelper.IReportDataStandardAnalysis> Update;

        private void OnUpdate(AnalysisHelper.IReportDataStandardAnalysis data)
        {
            Update?.Invoke(this, new AnalysisHelper.UpdateEventArgs<AnalysisHelper.IReportDataStandardAnalysis>(data));
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ComparisonType1.AutoPostBack = true;
            ComparisonType1.CheckedChanged += ComparisonType_CheckedChanged;

            ComparisonType2.AutoPostBack = true;
            ComparisonType2.CheckedChanged += ComparisonType_CheckedChanged;

            ComparisonType3.AutoPostBack = true;
            ComparisonType3.CheckedChanged += ComparisonType_CheckedChanged;

            AnalyzeButton.Click += AnalyzeButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

        }

        #endregion

        #region Methods (public)

        public void SetDefaultValues()
        {
            ComparisonType3.Checked = false;
            ComparisonType2.Checked = false;
            ComparisonType1.Checked = true;

            OnComparisonTypeSelected();
        }

        #endregion

        #region

        private void ComparisonType_CheckedChanged(object sender, EventArgs e) => OnComparisonTypeSelected();

        private void OnComparisonTypeSelected()
        {
            InputsType1.Visible = false;
            InputsType2.Visible = false;
            InputsType3.Visible = false;
            OutputSettings.Visible = false;

            if (ComparisonType1.Checked)
            {
                InputsType1.Visible = true;
                InputsType1.SetDefaultValues();
                OutputSettings.Visible = true;
            }
            else if (ComparisonType2.Checked)
            {
                InputsType2.Visible = true;
                InputsType2.SetDefaultValues();
                OutputSettings.Visible = true;
            }
            else if (ComparisonType3.Checked)
            {
                InputsType3.Visible = true;
                InputsType3.SetDefaultValues();
            }
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var lang = "en";
            AnalysisHelper.IReportDataStandardAnalysis data = null;

            if (ComparisonType1.Checked)
            {
                var standard1 = StandardSearch.Select(InputsType1.StandardIdentifier1.Value);
                var gacs1 = AnalysisHelper.GetStandardAnalysisGacs(standard1.StandardIdentifier, lang);

                var data1 = new AnalysisHelper.ReportDataStandardAnalysis1
                {
                    Title = $"{Common.LabelHelper.GetTranslation("Standard Analysis for")} {GetTitle(standard1)}",
                };

                BindDataType1(data1, gacs1, InputsType1.StandardIdentifier2.Value, lang);

                data = data1;
            }
            else if (ComparisonType2.Checked)
            {
                var standard1 = StandardSearch.Select(InputsType2.StandardIdentifier1.Value);
                var gacs1 = AnalysisHelper.GetStandardAnalysisGacs(standard1.StandardIdentifier, lang);

                data = new AnalysisHelper.ReportDataStandardAnalysis2
                {
                    Title = $"{Common.LabelHelper.GetTranslation("Standard Analysis for")} {GetTitle(standard1)}",
                    Reports = StandardSearch
                        .Select(x => x.OrganizationIdentifier == Organization.Key && x.StandardType == InputsType2.StandardType2)
                        .OrderBy(x => GetTitle(x))
                        .Select(x =>
                        {
                            var data1 = new AnalysisHelper.ReportDataStandardAnalysis1
                            {
                                Title = GetTitle(x),
                            };

                            BindDataType1(data1, gacs1, x.StandardIdentifier, lang);

                            return data1;
                        })
                        .Where(x => x.Overlap.HasValue && x.Overlap.Value != 0 || x.Shared.IsNotEmpty() || x.Missing.IsNotEmpty())
                        .ToArray()
                };
            }
            else if (ComparisonType3.Checked)
            {
                var competency = StandardSearch.Select(InputsType3.CompetencyKey.Value);

                data = new AnalysisHelper.ReportDataStandardAnalysis3
                {
                    Title = $"{Common.LabelHelper.GetTranslation("Standard Analysis for")} {GetTitle(competency)}",
                    Standards = StandardSearch
                        .Select(x => x.OrganizationIdentifier == Organization.Key && x.StandardType == InputsType3.StandardType)
                        .OrderBy(x => GetTitle(x))
                        .Where(x =>
                        {
                            return competency.StandardIdentifier == x.StandardIdentifier
                                || StandardContainmentSearch.SelectTree(new[] { x.StandardIdentifier }).Any(y => y.ChildStandardIdentifier == competency.StandardIdentifier);
                        })
                        .Select(x => new AnalysisHelper.StandardInfo(x))
                        .ToArray()
                };
            }

            if (data != null)
                OnUpdate(data);

            string GetTitle(Standard entity)
            {
                var titles = ServiceLocator.ContentSearch.GetTitles(entity.StandardIdentifier);
                return titles.TryGetValue(lang, out var title) ? title : "N/A";
            }
        }

        private void BindDataType1(AnalysisHelper.ReportDataStandardAnalysis1 data, AnalysisHelper.StandardInfo[] gacs1, Guid key2, string lang)
        {
            var gacs2 = AnalysisHelper.GetStandardAnalysisGacs(key2, lang);
            var competencies2 = gacs2.SelectMany(x => x.Children).Select(x => x.StandardIdentifier).ToHashSet();

            if (OutputOverlap.Checked)
            {
                var competencies1 = gacs1.SelectMany(x => x.Children).ToArray();
                var overlapCount = competencies1
                    .Where(x => competencies2.Contains(x.StandardIdentifier))
                    .Count();

                data.Overlap = overlapCount == 0 ? 0d : ((double)overlapCount / competencies1.Length);
            }

            if (OutputShared.Checked)
                data.Shared = gacs1
                    .Select(g => new AnalysisHelper.ReportDataGac
                    {
                        Title = g.Title,
                        Competencies = g.Children
                            .Where(c => competencies2.Contains(c.StandardIdentifier))
                            .ToArray()
                    })
                    .Where(g => g.Competencies.Length > 0)
                    .ToArray();

            if (OutputMissing.Checked)
                data.Missing = gacs1
                    .Select(g => new AnalysisHelper.ReportDataGac
                    {
                        Title = g.Title,
                        Competencies = g.Children
                            .Where(c => !competencies2.Contains(c.StandardIdentifier))
                            .ToArray()
                    })
                    .Where(g => g.Competencies.Length > 0)
                    .ToArray();
        }

        #endregion
    }
}