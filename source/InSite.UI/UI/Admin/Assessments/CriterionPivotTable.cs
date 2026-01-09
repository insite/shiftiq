using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Assessments.Web.UI
{
    public class CriterionPivotTable : System.Web.UI.Control
    {
        #region Classes

        private class CompetencyInfo
        {
            public Guid Thumbprint { get; set; }
            public int Number { get; set; }
            public string Title { get; set; }
            public string Subtype { get; set; }
        }

        private class ControlData
        {
            public Dictionary<int, CompetencyInfo> CompetencyMapping { get; }
            public Dictionary<int, TCollectionItem> DifficultyMapping { get; }
            public Dictionary<int, TCollectionItem> TaxonomyMapping { get; }

            public ControlData(Dictionary<int, CompetencyInfo> competencies, Dictionary<int, TCollectionItem> difficulties, Dictionary<int, TCollectionItem> taxonomies)
            {
                CompetencyMapping = competencies;
                DifficultyMapping = difficulties;
                TaxonomyMapping = taxonomies;
            }
        }

        private sealed class ControlHub
        {
            #region Properties

            public static ControlHub Current
            {
                get
                {
                    var key = typeof(ControlHub) + "." + nameof(Current);
                    var value = (ControlHub)HttpContext.Current.Items[key];

                    if (value == null)
                        HttpContext.Current.Items[key] = value = new ControlHub();

                    return value;
                }
            }

            #endregion

            #region Fields

            private bool _isInited = false;
            private List<CriterionPivotTable> _controls = new List<CriterionPivotTable>();
            private Dictionary<Guid, Dictionary<int, CompetencyInfo>> _competencies = null;
            private Dictionary<int, TCollectionItem> _difficulties = null;
            private Dictionary<int, TCollectionItem> _taxonomies = null;

            #endregion

            #region Construction

            private ControlHub()
            {

            }

            #endregion

            #region Methods

            public void Register(CriterionPivotTable ctrl)
            {
                if (!_isInited)
                    _controls.Add(ctrl);
            }

            public ControlData GetData(Criterion sieve)
            {
                if (!_isInited)
                    Init();

                var set = sieve.Sets.First();

                return new ControlData(
                    _competencies.ContainsKey(set.Standard) ? _competencies[set.Standard] : new Dictionary<int, CompetencyInfo>(),
                    _difficulties,
                    _taxonomies
                );
            }

            private void Init()
            {
                var assetFilter = new HashSet<Guid>();

                foreach (var ctrl in _controls)
                {
                    if (ctrl._sieve == null)
                        continue;

                    var standards = ctrl._sieve.Sets.Where(x => x.Standard != Guid.Empty).Select(x => x.Standard);
                    foreach (var standard in standards)
                        assetFilter.Add(standard);
                }

                _difficulties = Questions.Controls.WorkshopQuestionRepeater.GetCollectionItemsAsDictionary(CollectionName.Assessments_Questions_Classification_Difficulty);
                _taxonomies = Questions.Controls.WorkshopQuestionRepeater.GetCollectionItemsAsDictionary(CollectionName.Assessments_Questions_Classification_Taxonomy);

                _competencies = StandardSearch
                    .Bind(
                        x => new
                        {
                            ParentCode = x.Parent.Code,
                            ParentThumbprint = x.Parent.StandardIdentifier,
                            x.StandardIdentifier,
                            x.AssetNumber,
                            x.StandardType,
                            x.Code,
                            TitleEn = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title),
                        },
                        x => assetFilter.Contains(x.Parent.StandardIdentifier) && x.StandardType == Shift.Constant.StandardType.Competency
                    )
                    .GroupBy(x => x.ParentThumbprint)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(y => new CompetencyInfo
                        {
                            Thumbprint = y.StandardIdentifier,
                            Number = y.AssetNumber,
                            Subtype = y.StandardType,
                            Title = $"{y.ParentCode}{y.Code}. {y.TitleEn}",
                        }).ToDictionary(y => y.Number)
                    );

                _isInited = true;
            }



            #endregion
        }

        private class QuestionCountIndex
        {
            #region Fields

            private Dictionary<MultiKey, int> _indexes;

            #endregion

            #region Constructor

            private QuestionCountIndex()
            {
                _indexes = new Dictionary<MultiKey, int>();
            }

            #endregion

            #region Methods

            public static QuestionCountIndex Create(IEnumerable<Question> questions)
            {
                var result = new QuestionCountIndex();

                foreach (var q in questions)
                {
                    var difficulty = q.Classification?.Difficulty ?? int.MinValue;
                    var taxonomy = q.Classification?.Taxonomy ?? int.MinValue;
                    var competency = q.Standard;

                    if (difficulty != int.MinValue && taxonomy != int.MinValue && competency != Guid.Empty)
                        result.AddKey(difficulty, taxonomy, competency);

                    if (taxonomy != int.MinValue && competency != Guid.Empty)
                        result.AddKey(int.MinValue, taxonomy, competency);

                    if (difficulty != int.MinValue && competency != Guid.Empty)
                        result.AddKey(difficulty, int.MinValue, competency);

                    if (difficulty != int.MinValue && taxonomy != int.MinValue)
                        result.AddKey(difficulty, taxonomy, Guid.Empty);
                }

                return result;
            }

            private void AddKey(params object[] values)
            {
                var key = new MultiKey(values);
                if (_indexes.ContainsKey(key))
                    _indexes[key] += 1;
                else
                    _indexes.Add(key, 1);
            }

            public int Count(IPivotDimensionNode row, IPivotDimensionNode col, ControlData data)
            {
                var difficulty = int.MinValue;
                var taxonomy = int.MinValue;
                var competency = Guid.Empty;

                if (row != null || col != null)
                {
                    var nodes = new List<IPivotDimensionNode>();

                    if (row != null)
                        AddNodes(nodes, row);

                    if (col != null)
                        AddNodes(nodes, col);

                    foreach (var n in nodes)
                    {
                        if (!int.TryParse(n.Unit, out var nValue))
                            return 0;

                        if (n.Dimension == "Difficulty")
                            difficulty = nValue;
                        else if (n.Dimension == "Taxonomy")
                            taxonomy = nValue;
                        else if (n.Dimension == "Competency")
                        {
                            if (data.CompetencyMapping.ContainsKey(nValue))
                                competency = data.CompetencyMapping[nValue].Thumbprint;
                            else
                                return 0;
                        }

                        else
                            throw new ApplicationError("Unexpected dimension name: " + n.Dimension);
                    }
                }

                return Count(difficulty, taxonomy, competency);

                void AddNodes(List<IPivotDimensionNode> list, IPivotDimensionNode node)
                {
                    while (!node.IsRoot)
                    {
                        list.Add(node);

                        node = node.Parent;
                    }
                }
            }

            private int Count(params object[] values)
            {
                var key = new MultiKey(values);

                return _indexes.ContainsKey(key) ? _indexes[key] : 0;
            }

            #endregion
        }

        #endregion

        #region Properties

        private string Output
        {
            get => (string)ViewState[nameof(Output)];
            set => ViewState[nameof(Output)] = value;
        }

        #endregion

        #region Fields

        private Criterion _sieve = null;
        private ControlData _data = null;
        private QuestionCountIndex _questionCount;

        #endregion

        #region Construction

        public CriterionPivotTable()
        {
            ControlHub.Current.Register(this);
        }

        #endregion

        #region Initialization

        public void LoadData(Criterion sieve)
        {
            _sieve = sieve;
        }

        public void Clear()
        {
            _sieve = null;
            Output = null;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Output == null && _sieve != null)
            {
                var questions = _sieve.Sets.SelectMany(x => x.Questions);

                _data = ControlHub.Current.GetData(_sieve);
                _questionCount = QuestionCountIndex.Create(questions);

                Output = _sieve.PivotFilter.ToHtml(new PivotTable.RenderOptions
                {
                    RenderTotalColumn = true,
                    RenderTotalRow = true,
                    GetColumnHeaderName = GetHeaderName,
                    GetRowHeaderName = GetHeaderName,
                    GetCellValue = cell =>
                    {
                        if (cell.Value.HasValue && cell.Value.Value > 0)
                        {
                            var count = _questionCount.Count(cell.GetRow(), cell.GetColumn(), _data);

                            return GetCellHtml(cell.Value.Value, count);
                        }
                        else
                            return string.Empty;
                    },
                    GetRowTotalValue = GetTotalHtml,
                    GetColumnTotalValue = GetTotalHtml,
                    GetGrandTotalValue = GetTotalHtml
                });
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Visible && !string.IsNullOrEmpty(Output))
                writer.Write(Output);
        }

        #endregion

        #region Methods

        private string GetHeaderName(IPivotDimensionNode node)
        {
            if (int.TryParse(node.Unit, out var value))
            {
                if (node.Dimension == "Difficulty")
                {
                    if (_data.DifficultyMapping.TryGetValue(value, out var difficulty))
                        return $"{difficulty.ItemSequence}. {difficulty.ItemName}";
                }
                else if (node.Dimension == "Taxonomy")
                {
                    if (_data.TaxonomyMapping.TryGetValue(value, out var taxonomy))
                        return $"{taxonomy.ItemSequence}. {taxonomy.ItemName}";
                }
                else if (node.Dimension == "Competency")
                {
                    if (_data.CompetencyMapping.TryGetValue(value, out var competency))
                        return competency.Title;
                }
            }

            return node.Unit;
        }

        private string GetCellHtml(int count, int max)
        {
            if (count > max)
                return $"<span class='text-danger'>{count:n0} of {max:n0}</span>";
            else if (count < max)
                return $"<span class='text-success'>{count:n0} of {max:n0}</span>";
            else
                return count.ToString("n0");
        }

        private string GetTotalHtml(IPivotCell[] cells)
        {
            var value = 0;
            var max = 0;

            foreach (var cell in cells)
            {
                if (!cell.Value.HasValue || cell.Value.Value <= 0)
                    continue;

                value += cell.Value.Value;
                max += _questionCount.Count(cell.GetRow(), cell.GetColumn(), _data);
            }

            return GetCellHtml(value, max);
        }

        #endregion
    }
}