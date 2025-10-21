using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Criteria.Controls
{
    public partial class CriterionInput : BaseUserControl
    {
        #region Classes

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class DimensionsCompetencyInfo
        {
            #region Properties

            [JsonProperty(PropertyName = "id")]
            public Guid Thumbprint { get; set; }

            [JsonProperty(PropertyName = "num")]
            public int Number { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "subtype")]
            public string Subtype { get; set; }

            #endregion
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class DimensionDataItem
        {
            #region Properties

            [JsonProperty(PropertyName = "difficulty")]
            public DimensionsDataOption[] Difficulty { get; set; }

            [JsonProperty(PropertyName = "taxonomy")]
            public DimensionsDataOption[] Taxonomy { get; set; }

            [JsonProperty(PropertyName = "competency")]
            public DimensionsCompetencyInfo[] Competency { get; set; }

            #endregion
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class DimensionsDataOption
        {
            #region Properties

            [JsonProperty(PropertyName = "value")]
            public int Value { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            #endregion
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class QuestionDataItem
        {
            #region Properties

            [JsonProperty(PropertyName = "difficulty")]
            public int? Difficulty { get; set; }

            [JsonProperty(PropertyName = "taxonomy")]
            public int? Taxonomy { get; set; }

            [JsonProperty(PropertyName = "competency")]
            public int? Competency { get; set; }

            [JsonProperty(PropertyName = "count")]
            public int Count { get; set; }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class FilterConfig
        {
            #region Properties

            [JsonProperty(PropertyName = "row")]
            public FilterConfigAxis RowAxis { get; private set; }

            [JsonProperty(PropertyName = "col")]
            public FilterConfigAxis ColAxis { get; private set; }

            [JsonProperty(PropertyName = "exclusions")]
            public FilterConfigExclusion[] Exclusions { get; private set; }

            #endregion

            #region Methods

            public ICollection<int> GetExclusions(string name)
            {
                foreach (var excl in Exclusions)
                {
                    if (excl.Name == name)
                        return new HashSet<int>(excl.Values.Select(x => int.Parse(x)));
                }

                return new int[0];
            }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class FilterConfigAxis
        {
            #region Properties

            [JsonProperty(PropertyName = "fields")]
            public string[] Fields { get; private set; }

            [JsonProperty(PropertyName = "orderBy")]
            public FilterConfigAxisOrder OrderBy { get; private set; }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class FilterConfigAxisOrder
        {
            #region Properties

            [JsonProperty(PropertyName = "field")]
            public string Field { get; private set; }

            [JsonProperty(PropertyName = "order")]
            public string Order { get; private set; }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class FilterConfigExclusion
        {
            #region Properties

            [JsonProperty(PropertyName = "name")]
            public string Name { get; private set; }

            [JsonProperty(PropertyName = "values")]
            public string[] Values { get; private set; }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class FilterChange
        {
            #region Properties

            [JsonProperty(PropertyName = "row")]
            public string[] RowKey { get; private set; }

            [JsonProperty(PropertyName = "col")]
            public string[] ColumnKey { get; private set; }

            [JsonProperty(PropertyName = "value")]
            public int? Value { get; private set; }

            #endregion
        }

        #endregion

        #region Properties

        protected DimensionDataItem DimensionsData
        {
            get => (DimensionDataItem)ViewState[nameof(DimensionsData)];
            set => ViewState[nameof(DimensionsData)] = value;
        }

        protected QuestionDataItem[] QuestionsData
        {
            get => (QuestionDataItem[])ViewState[nameof(QuestionsData)];
            set => ViewState[nameof(QuestionsData)] = value;
        }

        public PivotTable RequirementsTable
        {
            get => (PivotTable)ViewState[nameof(RequirementsTable)];
            private set => ViewState[nameof(RequirementsTable)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriterionTypeNone.AutoPostBack = true;
            CriterionTypeTag.AutoPostBack = true;
            CriterionTypePivot.AutoPostBack = true;

            CriterionTypeNone.CheckedChanged += FilterTypeList_SelectedIndexChanged;
            CriterionTypeTag.CheckedChanged += FilterTypeList_SelectedIndexChanged;
            CriterionTypePivot.CheckedChanged += FilterTypeList_SelectedIndexChanged;

            // Pivot Table

            BuildFilterButton.Click += BuildFilterButton_Click;

            RequirementsPivotTableValidator.ServerValidate += RequirementsPivotTableValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ApplyChanges();
        }

        #endregion

        #region Event handlers

        private void RequirementsPivotTableValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = RequirementsTable != null && !RequirementsTable.IsEmpty;
        }

        private void BuildFilterButton_Click(object sender, EventArgs e)
        {
            PivotTable table = null;

            if (!string.IsNullOrEmpty(FilterData.Value))
            {
                var config = JsonConvert.DeserializeObject<FilterConfig>(FilterData.Value);

                table = new PivotTable();

                foreach (var row in config.RowAxis.Fields)
                    table.AddRow(row, GetDimensionUnits(row, config));

                foreach (var col in config.ColAxis.Fields)
                    table.AddColumn(col, GetDimensionUnits(col, config));

                foreach (var row in table.Rows.GetIndexes())
                {
                    foreach (var col in table.Columns.GetIndexes())
                    {
                        var cell = row.GetCell(col);
                        var questions = GetQuestions(row, col);

                        cell.Value = questions.Sum(x => x.Count);
                    }
                }
            }

            SetInputValues(table);

            if (RequirementsSection.Visible)
                RequirementsSection.IsSelected = true;

            FilterData.Value = string.Empty;

            string[] GetDimensionUnits(string name, FilterConfig config)
            {
                var exclusions = config.GetExclusions(name);

                if (name == "Difficulty")
                {
                    return DimensionsData.Difficulty
                        .Where(x => !exclusions.Contains(x.Value))
                        .OrderBy(x => x.Value)
                        .Select(x => x.Value.ToString())
                        .ToArray();
                }
                else if (name == "Taxonomy")
                {
                    return DimensionsData.Taxonomy
                        .Where(x => !exclusions.Contains(x.Value))
                        .OrderBy(x => x.Value)
                        .Select(x => x.Value.ToString())
                        .ToArray();
                }
                else if (name == "Competency")
                {
                    return DimensionsData.Competency
                        .Where(x => !exclusions.Contains(x.Number))
                        .OrderBy(x => x.Title)
                        .Select(x => x.Number.ToString())
                        .ToArray();
                }
                else
                    throw new ApplicationError("Unexpected dimension name: " + name);
            }
        }

        private void FilterTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilterTypeChanged();

            FilterTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnFilterTypeChanged()
        {
            var filterType = FilterType;
            var isTag = filterType == CriterionFilterType.Tag;
            var isPivot = filterType == CriterionFilterType.Pivot;

            FilterPanel.Visible = filterType != CriterionFilterType.All;
            FilterHeading.InnerText = isTag
                ? "Question Tag Filter"
                : isPivot
                    ? "Pivot Table Filter"
                    : "All Questions";

            TagFilter.Visible = isTag;
            PivotFilter.Visible = isPivot;
            QuestionLimitField.Visible = !isPivot;
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(List<Set> sets)
        {
            if (sets.IsEmpty())
                return;

            var standards = sets.Select(x => x.Standard);

            var competencies = StandardSearch
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
                    x => standards.Contains(x.Parent.StandardIdentifier) && x.StandardType == Shift.Constant.StandardType.Competency
                )
                .Select(y => new DimensionsCompetencyInfo
                {
                    Thumbprint = y.StandardIdentifier,
                    Number = y.AssetNumber,
                    Subtype = y.StandardType,
                    Title = $"{y.ParentCode}{y.Code}. {y.TitleEn}",
                });

            DimensionsData = new DimensionDataItem
            {
                Difficulty = TCollectionItemCache
                    .Select(new TCollectionItemFilter
                    {
                        OrganizationIdentifier = Identity.Organization.Identifier,
                        CollectionName = CollectionName.Assessments_Questions_Classification_Difficulty
                    })
                    .Select(x => new DimensionsDataOption
                    {
                        Value = x.ItemSequence,
                        Text = $"{x.ItemSequence}. {x.ItemName}"
                    })
                    .ToArray(),
                Taxonomy = TCollectionItemCache
                    .Select(new TCollectionItemFilter
                    {
                        OrganizationIdentifier = Identity.Organization.Identifier,
                        CollectionName = CollectionName.Assessments_Questions_Classification_Taxonomy
                    })
                    .Select(x => new DimensionsDataOption
                    {
                        Value = x.ItemSequence,
                        Text = $"{x.ItemSequence}. {x.ItemName}"
                    })
                    .ToArray(),
                Competency = competencies.OrderBy(x => x.Number).ToArray()
            };

            var competencyMapping = competencies.ToDictionary(x => x.Thumbprint, x => x.Number);

            QuestionsData = sets.SelectMany(x => x.Questions)
                .GroupBy(x => new { x.Standard, x.Classification.Difficulty, x.Classification.Taxonomy })
                .Select(x => new QuestionDataItem
                {
                    Difficulty = x.Key.Difficulty,
                    Taxonomy = x.Key.Taxonomy,
                    Competency = competencyMapping.ContainsKey(x.Key.Standard) ? competencyMapping[x.Key.Standard] : (int?)null,
                    Count = x.Count()
                })
                .ToArray();

            SetInputValues((PivotTable)null);
        }

        public void SetInputValues(Criterion sieve)
        {
            if (!string.IsNullOrEmpty(sieve.TagFilter))
                CriterionTypeTag.Checked = true;
            else if (sieve.PivotFilter != null && !sieve.PivotFilter.IsEmpty)
                CriterionTypePivot.Checked = true;
            else
                CriterionTypeNone.Checked = true;

            OnFilterTypeChanged();

            SetWeight.ValueAsDecimal = sieve.SetWeight;
            QuestionLimit.ValueAsInt = sieve.QuestionLimit;
            CriterionTagFilter.Text = sieve.TagFilter;

            // Pivot Table

            if (sieve.Sets.IsNotEmpty())
                SetInputValues(sieve.Sets);

            SetInputValues(sieve.PivotFilter);
        }

        private void SetInputValues(PivotTable table)
        {
            RequirementsTable = table;

            RenderRequirementsTable();
        }

        private void ApplyChanges()
        {
            var json = RequirementsChanges.Value;

            RequirementsChanges.Value = string.Empty;

            if (RequirementsTable == null || RequirementsTable.IsEmpty || string.IsNullOrEmpty(json))
                return;

            var changes = JsonConvert.DeserializeObject<FilterChange[]>(json);
            if (changes.Length == 0)
                return;

            foreach (var change in changes)
            {
                var row = new MultiKey<string>(change.RowKey);
                var col = new MultiKey<string>(change.ColumnKey);
                var cell = RequirementsTable.GetCell(row, col);

                cell.Value = change.Value;
            }

            RenderRequirementsTable();
        }

        #endregion

        #region Helper methods

        private void RenderRequirementsTable()
        {
            if (RequirementsTable == null || RequirementsTable.IsEmpty)
            {
                if (RequirementsSection.IsSelected)
                    DimensionsSection.IsSelected = true;

                RequirementsSection.Visible = false;
                RequirementsPivotTable.InnerHtml = string.Empty;

                return;
            }

            var difficultyMapping = DimensionsData.Difficulty.ToDictionary(x => x.Value, x => x);
            var taxonomyMapping = DimensionsData.Taxonomy.ToDictionary(x => x.Value, x => x);
            var competencyMapping = DimensionsData.Competency.ToDictionary(x => x.Number, x => x);

            RequirementsSection.Visible = true;
            RequirementsPivotTable.InnerHtml = RequirementsTable.ToHtml(new PivotTable.RenderOptions
            {
                RenderTotalColumn = true,
                RenderTotalRow = true,
                GetColumnHeaderName = GetHeaderName,
                GetRowHeaderName = GetHeaderName,
                GetCellValue = cell =>
                {
                    var row = cell.GetRow();
                    var col = cell.GetColumn();
                    var questions = GetQuestions(row, col);

                    var data = new
                    {
                        value = cell.Value.HasValue && cell.Value > 0 ? cell.Value : 0,
                        max = questions.Sum(x => x.Count),
                        row = cell.RowKey.Values,
                        col = cell.ColumnKey.Values,
                    };

                    return JsonHelper.SerializeJsObject(data);
                }
            });

            string GetHeaderName(IPivotDimensionNode node)
            {
                if (int.TryParse(node.Unit, out var value))
                {
                    if (node.Dimension == "Difficulty")
                    {
                        if (difficultyMapping.ContainsKey(value))
                            return difficultyMapping[value].Text;
                    }
                    else if (node.Dimension == "Taxonomy")
                    {
                        if (taxonomyMapping.ContainsKey(value))
                            return taxonomyMapping[value].Text;
                    }
                    else if (node.Dimension == "Competency")
                    {
                        if (competencyMapping.ContainsKey(value))
                            return competencyMapping[value].Title;
                    }
                }

                return node.Unit;
            }
        }

        QuestionDataItem[] GetQuestions(IPivotDimensionNode row, IPivotDimensionNode col)
        {
            var nodes = new List<IPivotDimensionNode>();

            AddNodes(nodes, row);
            AddNodes(nodes, col);

            var query = QuestionsData.AsQueryable();

            foreach (var n in nodes)
            {
                if (!int.TryParse(n.Unit, out var nValue))
                {
                    query = query.Where(x => false);
                    break;
                }

                if (n.Dimension == "Difficulty")
                    query = query.Where(x => x.Difficulty == nValue);
                else if (n.Dimension == "Taxonomy")
                    query = query.Where(x => x.Taxonomy == nValue);
                else if (n.Dimension == "Competency")
                    query = query.Where(x => x.Competency == nValue);
                else
                    throw new ApplicationError("Unexpected dimension name: " + n.Dimension);
            }

            return query.ToArray();

            void AddNodes(List<IPivotDimensionNode> list, IPivotDimensionNode node)
            {
                while (!node.IsRoot)
                {
                    list.Add(node);

                    node = node.Parent;
                }
            }
        }

        #endregion

        #region X

        #region Classes

        public class OutputModel
        {
            public CriterionFilterType FilterType { get; }
            public decimal SetWeight { get; }
            public int QuestionLimit { get; }
            public string BasicFilter { get; }

            public OutputModel(CriterionFilterType filterType, decimal setWeight, int questionLimit, string basicFilter)
            {
                FilterType = filterType;
                SetWeight = setWeight;
                QuestionLimit = questionLimit;
                BasicFilter = basicFilter;
            }
        }

        #endregion

        #region Events

        public event EventHandler FilterTypeChanged;

        #endregion

        #region Properties

        public CriterionFilterType FilterType
        {
            get
            {
                if (CriterionTypeTag.Checked) return CriterionFilterType.Tag;
                if (CriterionTypePivot.Checked) return CriterionFilterType.Pivot;
                return CriterionFilterType.All;
            }
        }

        #endregion

        #region Settings/getting input values

        public void SetDefaultInputValues()
        {
            var sieve = new Criterion
            {
                Sets = new List<Set>()
            };

            SetInputValues(sieve);
        }

        public OutputModel GetInputValues()
        {
            return new OutputModel(FilterType, SetWeight.ValueAsDecimal.Value, QuestionLimit.ValueAsInt ?? 0, CriterionTagFilter.Text);
        }

        #endregion

        #endregion
    }
}