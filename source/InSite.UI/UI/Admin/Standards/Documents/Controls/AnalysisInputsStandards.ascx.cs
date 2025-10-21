using System;
using System.Linq;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Standards.Documents.Controls
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

        private AnalysisHelper.IReportDataStandardAnalysis CompareTwoStandards(string lang)
        {
            var standard1 = StandardSearch.Select(InputsType1.StandardIdentifier1.Value);
            var gacs1 = AnalysisHelper.GetStandardAnalysisGacs(standard1.StandardIdentifier, lang);

            var data1 = new AnalysisHelper.ReportDataStandardAnalysis1
            {
                Title = $"Standard Analysis for {GetTitle(standard1, lang)}",
            };

            BindDataType1(data1, gacs1, InputsType1.StandardIdentifier2.Value, lang);

            return data1;
        }

        private AnalysisHelper.IReportDataStandardAnalysis CompareOneStandardWithAll(string lang)
        {

            var standard1 = StandardSearch.Select(InputsType2.StandardIdentifier1.Value);
            var gacs1 = AnalysisHelper.GetStandardAnalysisGacs(standard1.StandardIdentifier, lang);

            return new AnalysisHelper.ReportDataStandardAnalysis2
            {
                Title = $"Standard Analysis for {GetTitle(standard1, lang)}",
                Reports = StandardSearch
                        .Select(x => x.OrganizationIdentifier == Organization.Key && x.StandardType == InputsType2.StandardType2)
                        .OrderBy(x => GetTitle(x, lang))
                        .Select(x =>
                        {
                            var data1 = new AnalysisHelper.ReportDataStandardAnalysis1
                            {
                                Title = GetTitle(x, lang),
                            };

                            BindDataType1(data1, gacs1, x.StandardIdentifier, lang);

                            return data1;
                        })
                        .Where(x => x.Overlap.HasValue && x.Overlap.Value != 0 || x.Shared.IsNotEmpty() || x.Missing.IsNotEmpty())
                        .ToArray()
            };
        }

        private AnalysisHelper.IReportDataStandardAnalysis CompareSharedCompetency(string lang)
        {
            var competency = StandardSearch.Select(InputsType3.CompetencyKey.Value);

            var data = new AnalysisHelper.ReportDataStandardAnalysis3
            {
                Title = $"Standard Analysis for {GetTitle(competency, lang)}",
                Standards = StandardSearch
                        .Select(x => x.OrganizationIdentifier == Organization.Key && x.StandardType == InputsType3.StandardType)
                        .OrderBy(x => GetTitle(x, lang))
                        .Where(x =>
                        {
                            return competency.StandardIdentifier == x.StandardIdentifier
                                || StandardContainmentSearch.SelectTree(new[] { x.StandardIdentifier }).Any(y =>
                                {
                                    var comp1 = y.ChildStandardIdentifier == competency.StandardIdentifier;
                                    var titles = ServiceLocator.ContentSearch.GetTitles(y.ChildStandardIdentifier);
                                    if (titles.Count > 0 && titles.ContainsKey(lang))
                                    {
                                        var comp2 = titles[lang].Equals(competency.ContentTitle, StringComparison.OrdinalIgnoreCase);
                                        return comp1 || comp2;
                                    }
                                    return comp1;
                                });
                        }).Select(x => new AnalysisHelper.StandardInfo(x)).ToArray()
            };
            return data;
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var lang = "en";
            AnalysisHelper.IReportDataStandardAnalysis data = null;

            if (ComparisonType1.Checked)
            {
                data = CompareTwoStandards(lang);
            }
            else if (ComparisonType2.Checked)
            {
                data = CompareOneStandardWithAll(lang);
            }
            else if (ComparisonType3.Checked)
            {
                data = CompareSharedCompetency(lang);
            }

            if (data != null)
                OnUpdate(data);
        }

        private string GetTitle(Standard entity, string lang)
        {
            var titles = ServiceLocator.ContentSearch.GetTitles(entity.StandardIdentifier);
            return titles.TryGetValue(lang, out var title) ? title : "N/A";
        }

        private void BindDataType1(AnalysisHelper.ReportDataStandardAnalysis1 data, AnalysisHelper.StandardInfo[] gacs1, Guid key2, string lang)
        {
            var gacs2 = AnalysisHelper.GetStandardAnalysisGacs(key2, lang);

            var competencies2 = gacs2.SelectMany(x => x.Children);
            var competencies1 = gacs1.SelectMany(x => x.Children).ToArray();

            var competencies2Identifier = competencies2.Select(x => x.StandardIdentifier).ToHashSet();
            var competencies2Titles = competencies2.Select(x => x.Title).Distinct().ToHashSet();

            if (OutputOverlap.Checked)
            {

                var overlapCount = competencies1.Count(x => competencies2Identifier.Contains(x.StandardIdentifier)
                    || competencies2Titles.Contains(x.Title, StringComparer.OrdinalIgnoreCase));

                data.Overlap = overlapCount == 0 ? 0d : ((double)overlapCount / competencies1.Length);
            }

            if (OutputShared.Checked)
                data.Shared = gacs1
                    .Select(g => new AnalysisHelper.ReportDataGac
                    {
                        Title = g.Title,
                        Competencies = g.Children
                            .Where(c => competencies2Identifier.Contains(c.StandardIdentifier)
                                || competencies2Titles.Contains(c.Title, StringComparer.OrdinalIgnoreCase))
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
                            .Where(c => !competencies2Identifier.Contains(c.StandardIdentifier)
                                && !competencies2Titles.Contains(c.Title, StringComparer.OrdinalIgnoreCase))
                            .ToArray()
                    })
                    .Where(g => g.Competencies.Length > 0)
                    .ToArray();
        }

        #endregion
    }
}