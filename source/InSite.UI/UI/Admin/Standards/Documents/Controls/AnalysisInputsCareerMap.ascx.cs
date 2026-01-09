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
    public partial class AnalysisInputsCareerMap : BaseUserControl
    {
        #region Events

        public event AnalysisHelper.UpdateEventHandler<AnalysisHelper.ReportDataCareerMap> Update;

        private void OnUpdate(AnalysisHelper.ReportDataCareerMap data)
        {
            Update?.Invoke(this, new AnalysisHelper.UpdateEventArgs<AnalysisHelper.ReportDataCareerMap>(data));
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BenchmarkType.AutoPostBack = true;
            BenchmarkType.ValueChanged += BenchmarkType_ValueChanged;

            Benchmark.AutoPostBack = true;
            Benchmark.ValueChanged += Benchmark_ValueChanged;

            ComparisonType.AutoPostBack = true;
            ComparisonType.ValueChanged += ComparisonType_ValueChanged;

            Comparison.AutoPostBack = true;
            Comparison.ValueChanged += Comparison_ValueChanged;

            AnalyzeButton.Click += AnalyzeButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Benchmark.Filter.StandardTypes = new[] { StandardType.Document };
            Benchmark.Filter.DocumentType = new[] { DocumentType.NationalOccupationStandard };
            Benchmark.Value = null;

            Comparison.Filter.StandardTypes = new[] { StandardType.Document };
            Comparison.Filter.DocumentType = new[] { DocumentType.NationalOccupationStandard };
            Comparison.Value = null;
        }

        #endregion

        #region Event handlers

        private void BenchmarkType_ValueChanged(object sender, EventArgs e) => OnTypeSelected(BenchmarkType.Value, Benchmark, Comparison);

        private void ComparisonType_ValueChanged(object sender, EventArgs e) => OnTypeSelected(ComparisonType.Value, Comparison, Benchmark);

        private void OnTypeSelected(string type, FindStandard selector, FindStandard anotherSelector)
        {
            var oldValue = !string.IsNullOrEmpty(type) && selector.Filter.DocumentType.Length == 1
                ? null
                : selector.Value;

            selector.Filter.DocumentType = !string.IsNullOrEmpty(type) ? new[] { type } : new[] { DocumentType.NationalOccupationStandard, DocumentType.CustomizedOccupationProfile };
            selector.Value = oldValue;

            if (oldValue == null && anotherSelector.Filter.Exclusions.StandardIdentifier.Count > 0)
            {
                var anotherOldValue = anotherSelector.Value;

                anotherSelector.Filter.Exclusions.StandardIdentifier.Clear();
                anotherSelector.Value = anotherOldValue;
            }
        }

        private void Benchmark_ValueChanged(object sender, EventArgs e) => OnBenchmarkSelected();

        private void OnBenchmarkSelected()
        {
            if (Benchmark.Value == Comparison.Value)
                Comparison.Value = null;

            Comparison.Filter.Exclusions.StandardIdentifier.Clear();
            if (Benchmark.HasValue)
                Comparison.Filter.Exclusions.StandardIdentifier.Add(Benchmark.Value.Value);

            var oldValue = Comparison.Value;
            Comparison.Value = oldValue;
        }

        private void Comparison_ValueChanged(object sender, EventArgs e) => OnComparisonSelected();

        private void OnComparisonSelected()
        {
            if (Comparison.Value == Benchmark.Value)
                Benchmark.Value = null;

            Benchmark.Filter.Exclusions.StandardIdentifier.Clear();
            if (Comparison.HasValue)
                Benchmark.Filter.Exclusions.StandardIdentifier.Add(Comparison.Value.Value);

            var oldValue = Benchmark.Value;
            Benchmark.Value = oldValue;
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var lang = "en";
            var benchmark = StandardSearch.Select(Benchmark.Value.Value);
            var benchmarkGacs = AnalysisHelper.GetDocumentGacs(benchmark.StandardIdentifier, lang);
            var benchmarkCompetencies = benchmarkGacs.SelectMany(x => x.Children).Select(x => x.StandardIdentifier).ToHashSet();

            var comparison = StandardSearch.Select(Comparison.Value.Value);
            var comparisonGacs = AnalysisHelper.GetDocumentGacs(comparison.StandardIdentifier, lang);
            var comparisonCompetencies = comparisonGacs.SelectMany(x => x.Children).ToArray();

            var overlapCount = comparisonCompetencies
                .Where(x => benchmarkCompetencies.Contains(x.StandardIdentifier))
                .Count();

            var data = new AnalysisHelper.ReportDataCareerMap
            {
                Title = $"Career Map for {GetTitle(benchmark)}",
                Overlap = OutputOverlap.Checked ? (overlapCount == 0 ? 0d : ((double)overlapCount / comparisonCompetencies.Length)) : (double?)null,
                SharedCompetencies = OutputShared.Checked
                    ? comparisonGacs
                        .Select(g => new AnalysisHelper.ReportDataGac
                        {
                            Title = g.Title,
                            Competencies = g.Children
                                .Where(c => benchmarkCompetencies.Contains(c.StandardIdentifier))
                                .ToArray()
                        })
                        .Where(g => g.Competencies.Length > 0)
                        .ToArray()
                    : null,

                MissingCompetencies = OutputMissing.Checked
                    ? comparisonGacs
                        .Select(g => new AnalysisHelper.ReportDataGac
                        {
                            Title = g.Title,
                            Competencies = g.Children
                                .Where(c => !benchmarkCompetencies.Contains(c.StandardIdentifier))
                                .ToArray()
                        })
                        .Where(g => g.Competencies.Length > 0)
                        .ToArray()
                    : null
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
            Benchmark.Value = null;
            Comparison.Value = null;

            OnBenchmarkSelected();
            OnComparisonSelected();
        }

        #endregion
    }
}