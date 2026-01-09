using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionStatisticsPanel : BaseUserControl
    {
        #region Classes

        private class TaxItem
        {
            public string Tag { get; set; }
            public int[] CountPerTaxonomy { get; set; }
            public int Questions => CountPerTaxonomy.Sum();
        }

        private class StandardItem
        {
            public string SetStandardCode { get; set; }
            public string QuestionStandardCode { get; set; }
            public int Questions { get; set; }
            public int?[] Taxonomies { get; set; }
        }

        private class SubCompetencyItem
        {
            public string SetStandardCode { get; set; }
            public string QuestionStandardCode { get; set; }
            public string QuestionSubCode { get; set; }
            public int Questions { get; set; }
        }

        #endregion

        #region Static fields

        private static readonly XlsxCellStyle BoldStyle = new XlsxCellStyle { IsBold = true };
        private static readonly XlsxCellStyle BoldRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
        private static readonly XlsxCellStyle RightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };

        #endregion

        #region Public methods

        public bool LoadData(BankState bank)
        {
            var questions = bank.Sets.SelectMany(set => set.EnumerateAllQuestions()).ToArray();

            return LoadData(bank, questions);
        }

        public bool LoadData(Form form)
        {
            return LoadData(form.Specification.Bank, form.GetQuestions());
        }

        private bool LoadData(BankState bank, IEnumerable<Question> questions)
        {
            var questionPerTagAndTaxonomy = CountQuestionPerTagAndTaxonomy(questions);
            var questionPerTaxonomy = CountQuestionPerTaxonomy(questions);
            var questionPerDifficulty = CountQuestionPerDifficulty(questions);
            var questionPerGac = CountQuestionPerGAC(bank, questions);
            var questionPerCode = CountQuestionPerCode(bank, questions);
            var questionPerLIG = CountQuestionPerLIG(questions);

            var assets = BankStatisticHelper.GetBankAssets(bank);
            var taxonomies = LoadTaxonomies(questions);
            var standards = LoadStandards(questions, assets, taxonomies);
            var subCompetencies = LoadSubCompetencies(questions, assets);

            TagsTaxonomyRepeater.DataSource = questionPerTaxonomy;
            TagsTaxonomyRepeater.DataBind();

            QuestionPerTagAndTaxonomyRepeater.DataSource = CreateTagAndTaxonomyList(questionPerTagAndTaxonomy, questionPerTaxonomy);
            QuestionPerTagAndTaxonomyRepeater.DataBind();

            QuestionPerTaxonomyRepeater.DataSource = questionPerTaxonomy;
            QuestionPerTaxonomyRepeater.DataBind();

            QuestionPerDifficultyRepeater.DataSource = questionPerDifficulty;
            QuestionPerDifficultyRepeater.DataBind();

            QuestionPerLigRepeater.DataSource = questionPerLIG;
            QuestionPerLigRepeater.DataBind();

            QuestionPerGacAndTaxonomyRepeater.DataSource = questionPerGac;
            QuestionPerGacAndTaxonomyRepeater.DataBind();

            QuestionCodeRepeater.DataSource = questionPerCode;
            QuestionCodeRepeater.DataBind();

            StandardsTaxonomyRepeater.DataSource = taxonomies;
            StandardsTaxonomyRepeater.DataBind();

            StandardsRepeater.DataSource = standards;
            StandardsRepeater.DataBind();

            SubCompetenciesRepeater.Visible = subCompetencies.Count > 0;
            SubCompetenciesRepeater.DataSource = subCompetencies;
            SubCompetenciesRepeater.DataBind();

            return questionPerTagAndTaxonomy.Rows.Count > 0
                   || questionPerTaxonomy.Rows.Count > 0
                   || questionPerDifficulty.Rows.Count > 0
                   || questionPerGac.Rows.Count > 0
                   || questionPerLIG.Rows.Count > 0
                   || standards.Count > 0;
        }

        #endregion

        #region XLSX

        public static byte[] GetXlsx(Form form)
        {
            var bank = form.Specification?.Bank;
            var questions = form.GetQuestions();

            return GetXlsxFromQuestions(bank, questions);
        }

        public static byte[] GetXlsx(BankState bank)
        {
            var questions = bank.Sets
                .SelectMany(set => set.EnumerateAllQuestions())
                .ToList();

            return GetXlsxFromQuestions(bank, questions);
        }

        private static byte[] GetXlsxFromQuestions(BankState bank, List<Question> questions)
        {
            var questionPerTagAndTaxonomy = CountQuestionPerTagAndTaxonomy(questions);
            var questionPerTaxonomy = CountQuestionPerTaxonomy(questions);
            var questionPerDifficulty = CountQuestionPerDifficulty(questions);
            var questionPerGac = CountQuestionPerGAC(bank, questions);
            var questionPerCode = CountQuestionPerCode(bank, questions);
            var questionPerLIG = CountQuestionPerLIG(questions);

            var assets = BankStatisticHelper.GetBankAssets(bank);
            var taxonomies = LoadTaxonomies(questions);
            var standards = LoadStandards(questions, assets, taxonomies);
            var subCompetencies = LoadSubCompetencies(questions, assets);

            var sheets = new List<XlsxWorksheet>();

            AddQuestionPerTaxonomySheet(sheets, questionPerTaxonomy);
            AddQuestionPerDifficultySheet(sheets, questionPerDifficulty);
            AddQuestionPerLigSheet(sheets, questionPerLIG);
            AddQuestionPerGacAndTaxonomySheet(sheets, questionPerGac);
            AddQuestionPerCodeSheet(sheets, questionPerCode);
            AddStandardsSheet(sheets, taxonomies, standards);
            AddSubCompetenciesSheet(sheets, subCompetencies);
            AddTagsSheet(sheets, questionPerTaxonomy, questionPerTagAndTaxonomy);

            return XlsxWorksheet.GetBytes(sheets.ToArray());
        }

        private static void AddQuestionPerTaxonomySheet(List<XlsxWorksheet> sheets, DataTable table)
        {
            if (table.Rows.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Taxonomy");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Taxonomy", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row["Taxonomy"] });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row["Count"], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerDifficultySheet(List<XlsxWorksheet> sheets, DataTable table)
        {
            if (table.Rows.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Difficulties");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Difficulty", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row["Difficulty"] });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row["Count"], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerLigSheet(List<XlsxWorksheet> sheets, DataTable table)
        {
            if (table.Rows.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Like Item Groups");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Like Item Group", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row["LikeItemGroup"] });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row["Count"], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerGacAndTaxonomySheet(List<XlsxWorksheet> sheets, DataTable table)
        {
            if (table.Rows.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Standards (GAC)");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Standard", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row["Standard"] });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row["Count"], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerCodeSheet(List<XlsxWorksheet> sheets, DataTable table)
        {
            if (table.Rows.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Codes");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Code", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row["Code"] });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row["Count"], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddStandardsSheet(List<XlsxWorksheet> sheets, int[] taxonomies, List<StandardItem> standards)
        {
            if (standards.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Standards (Competency)");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Standard", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            for (int i = 0; i < taxonomies.Length; i++)
            {
                sheet.Columns[2 + i].Width = 30;
                sheet.Cells.Add(new XlsxCell(2 + i, 0) { Value = $"Tax {taxonomies[i]}", Style = BoldRightStyle });
            }

            var rowIndex = 1;

            foreach (var item in standards)
            {
                var text = GetStandardCodeNoHtml(item.SetStandardCode) + GetStandardCodeNoHtml(item.QuestionStandardCode);

                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = text });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = item.Questions, Style = RightStyle });

                for (var i = 0; i < item.Taxonomies.Length; i++)
                    sheet.Cells.Add(new XlsxCell(2 + i, rowIndex) { Value = item.Taxonomies[i], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddSubCompetenciesSheet(List<XlsxWorksheet> sheets, List<SubCompetencyItem> standards)
        {
            if (standards.Count == 0)
                return;

            var sheet = new XlsxWorksheet("Standards (Sub Competency)");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Standard", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (var item in standards)
            {
                var text = GetStandardCodeNoHtml(item.SetStandardCode)
                    + GetStandardCodeNoHtml(item.QuestionStandardCode)
                    + GetStandardCodeNoHtml(item.QuestionSubCode);

                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = text });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = item.Questions, Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddTagsSheet(List<XlsxWorksheet> sheets, DataTable questionPerTaxonomy, DataTable questionPerTagAndTaxonomy)
        {
            if (questionPerTaxonomy.Rows.Count == 0)
                return;

            var taxonomies = CreateTagAndTaxonomyList(questionPerTagAndTaxonomy, questionPerTaxonomy);

            var sheet = new XlsxWorksheet("Tags");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Tag", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            for (int i = 0; i < questionPerTaxonomy.Rows.Count; i++)
            {
                var row = questionPerTaxonomy.Rows[i];

                sheet.Columns[2 + i].Width = 30;
                sheet.Cells.Add(new XlsxCell(2 + i, 0) { Value = $"Tax {row["Taxonomy"]}", Style = BoldRightStyle });
            }

            var rowIndex = 1;

            foreach (var item in taxonomies)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = item.Tag });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = item.Questions, Style = RightStyle });

                for (int i = 0; i < item.CountPerTaxonomy.Length; i++)
                    sheet.Cells.Add(new XlsxCell(2 + i, rowIndex) { Value = item.CountPerTaxonomy[i], Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionPerTagAndTaxonomyRepeater.ItemDataBound += QuestionPerTagAndTaxonomyRepeater_ItemDataBound;
            QuestionPerGacAndTaxonomyRepeater.ItemDataBound += QuestionPerGacAndTaxonomyRepeater_ItemDataBound;

            StandardsRepeater.ItemDataBound += StandardsRepeater_ItemDataBound;
        }

        #endregion

        #region Methods (build data sets)

        private static List<TaxItem> CreateTagAndTaxonomyList(DataTable questionPerTagAndTaxonomy,
            DataTable questionPerTaxonomy)
        {
            var taxonomyMap = new Dictionary<int, int>();
            for (var i = 0; i < questionPerTaxonomy.Rows.Count; i++)
                taxonomyMap.Add((int)questionPerTaxonomy.Rows[i]["Taxonomy"], taxonomyMap.Count);

            var list = new List<TaxItem>();
            for (var i = 0; i < questionPerTagAndTaxonomy.Rows.Count; i++)
            {
                var tag = (string)questionPerTagAndTaxonomy.Rows[i]["Tag"];
                var taxonomy = (int)questionPerTagAndTaxonomy.Rows[i]["Taxonomy"];
                var count = (int)questionPerTagAndTaxonomy.Rows[i]["Count"];
                var item = list.Count > 0 ? list[list.Count - 1] : null;

                if (item == null || !string.Equals(item.Tag, tag, StringComparison.OrdinalIgnoreCase))
                    list.Add(item = new TaxItem { Tag = tag, CountPerTaxonomy = new int[taxonomyMap.Count] });

                var taxonomyIndex = taxonomyMap[taxonomy];

                item.CountPerTaxonomy[taxonomyIndex] = count;
            }

            return list;
        }

        private static DataTable CountQuestionPerTaxonomy(IEnumerable<Question> questions)
        {
            var taxonomies = new SortedDictionary<int, int>();

            foreach (var question in questions)
            {
                var taxonomy = question.Classification.Taxonomy ?? 0;
                if (!taxonomies.ContainsKey(taxonomy))
                    taxonomies.Add(taxonomy, 0);
                taxonomies[taxonomy]++;
            }

            var dt = new DataTable();
            dt.Columns.Add("Taxonomy", typeof(int));
            dt.Columns.Add("Count", typeof(int));

            foreach (var key in taxonomies.Keys)
            {
                var row = dt.NewRow();

                row["Taxonomy"] = key;
                row["Count"] = taxonomies[key];

                dt.Rows.Add(row);
            }

            return dt;
        }

        private static DataTable CountQuestionPerTagAndTaxonomy(IEnumerable<Question> questions)
        {
            var tags = new SortedDictionary<string, int>();

            foreach (var question in questions)
            {
                var tagTax = $"{question.Classification.Tag ?? "None"}|{question.Classification.Taxonomy ?? 0}";
                if (!tags.ContainsKey(tagTax))
                    tags.Add(tagTax, 0);
                tags[tagTax]++;
            }

            var dt = new DataTable();
            dt.Columns.Add("Tag");
            dt.Columns.Add("Taxonomy", typeof(int));
            dt.Columns.Add("Count", typeof(int));

            foreach (var key in tags.Keys)
            {
                var row = dt.NewRow();

                row["Tag"] = key.Split('|')[0];
                row["Taxonomy"] = key.Split('|')[1];
                row["Count"] = tags[key];

                dt.Rows.Add(row);
            }

            return dt;
        }

        private static DataTable CountQuestionPerDifficulty(IEnumerable<Question> questions)
        {
            var difficulties = new SortedDictionary<int, int>();

            foreach (var question in questions)
            {
                var difficulty = question.Classification.Difficulty ?? 0;
                if (!difficulties.ContainsKey(difficulty))
                    difficulties.Add(difficulty, 0);
                difficulties[difficulty]++;
            }

            var dt = new DataTable();
            dt.Columns.Add("Difficulty", typeof(int));
            dt.Columns.Add("Count", typeof(int));

            foreach (var key in difficulties.Keys)
            {
                var row = dt.NewRow();

                row["Difficulty"] = key;
                row["Count"] = difficulties[key];

                dt.Rows.Add(row);
            }

            return dt;
        }

        private static DataTable CountQuestionPerLIG(IEnumerable<Question> questions)
        {
            var groups = new SortedDictionary<string, int>();

            foreach (var question in questions)
            {
                var group = question.Classification.LikeItemGroup ?? "None";
                if (!groups.ContainsKey(group))
                    groups.Add(group, 0);
                groups[group]++;
            }

            var dt = new DataTable();
            dt.Columns.Add("LikeItemGroup");
            dt.Columns.Add("Count", typeof(int));

            foreach (var key in groups.Keys)
            {
                var row = dt.NewRow();

                row["LikeItemGroup"] = key;
                row["Count"] = groups[key];

                dt.Rows.Add(row);
            }

            return dt;
        }

        private static DataTable CountQuestionPerGAC(BankState bank, IEnumerable<Question> questions)
        {
            var assets = BankStatisticHelper.GetBankAssets(bank);
            var standards = new SortedDictionary<string, int>();

            foreach (var question in questions)
            {
                var gac = "None";
                if (assets.TryGetValue(question.Set.Standard, out var asset) && !string.IsNullOrEmpty(asset.Code))
                    gac = asset.Code;

                if (standards.ContainsKey(gac))
                    standards[gac] += 1;
                else
                    standards.Add(gac, 1);
            }

            var dt = new DataTable();
            dt.Columns.Add("Standard");
            dt.Columns.Add("Count", typeof(int));

            foreach (var key in standards.Keys)
            {
                var row = dt.NewRow();

                row["Standard"] = key;
                row["Count"] = standards[key];

                dt.Rows.Add(row);
            }

            return dt;
        }

        private static DataTable CountQuestionPerCode(BankState bank, IEnumerable<Question> questions)
        {
            var dataSource = questions
                .Select(x => x.Classification.Code.IsEmpty() ? "None" : HttpUtility.HtmlEncode(x.Classification.Code))
                .GroupBy(x => x)
                .Select(x => (Code: x.Key, Count: x.Count()))
                .OrderBy(x => x.Code == "None" ? 0 : 1)
                .ThenBy(x => x.Code)
                .ToArray();

            var dt = new DataTable();
            dt.Columns.Add("Code");
            dt.Columns.Add("Count", typeof(int));

            foreach (var item in dataSource)
            {
                var row = dt.NewRow();

                row["Code"] = item.Code;
                row["Count"] = item.Count;

                dt.Rows.Add(row);
            }

            return dt;
        }

        private static int[] LoadTaxonomies(IEnumerable<Question> questions)
        {
            var taxonomiesIndex = new HashSet<int>();

            foreach (var question in questions)
            {
                var taxonomy = question.Classification.Taxonomy ?? 0;
                if (!taxonomiesIndex.Contains(taxonomy))
                    taxonomiesIndex.Add(taxonomy);
            }

            return taxonomiesIndex
                .OrderBy(x => x)
                .ToArray();
        }

        private static List<StandardItem> LoadStandards(
            IEnumerable<Question> questions,
            Dictionary<Guid, BankStatisticHelper.AssetInfo> assets,
            int[] taxonomies
            )
        {
            var groupedQuestions = new Dictionary<MultiKey<Guid, Guid>, List<Question>>();

            foreach (var question in questions)
            {
                var key = new MultiKey<Guid, Guid>(question.Set.Standard, question.Standard);
                if (!groupedQuestions.ContainsKey(key))
                    groupedQuestions.Add(key, new List<Question>());

                groupedQuestions[key].Add(question);
            }

            var standards = groupedQuestions.Select(standardItem =>
                {
                    var setStandard = assets.ContainsKey(standardItem.Key.Key1) ? assets[standardItem.Key.Key1] : null;
                    var questionStandard = assets.ContainsKey(standardItem.Key.Key2) ? assets[standardItem.Key.Key2] : null;
                    var questionTaxonomies = standardItem.Value.GroupBy(x => x.Classification.Taxonomy ?? 0).ToDictionary(x => x.Key, x => x.Count());

                    return new StandardItem
                    {
                        SetStandardCode = setStandard?.Code,
                        QuestionStandardCode = questionStandard?.Code,
                        Questions = standardItem.Value.Count,
                        Taxonomies = taxonomies.Select(x => questionTaxonomies.ContainsKey(x) ? questionTaxonomies[x] : (int?)null).ToArray()
                    };
                })
                .OrderBy(x => x.SetStandardCode)
                .ThenBy(x => x.QuestionStandardCode)
                .ThenBy(x => x.Questions)
                .ToList();

            return standards;
        }

        private static List<SubCompetencyItem> LoadSubCompetencies(IEnumerable<Question> questions, Dictionary<Guid, BankStatisticHelper.AssetInfo> assets)
        {
            if (!Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection)
                return new List<SubCompetencyItem>();

            var subCompetencies = new Dictionary<MultiKey<Guid, Guid, Guid>, int>();

            foreach (var question in questions)
            {
                if (question.SubStandards == null)
                    continue;

                foreach (var sub in question.SubStandards)
                {
                    var key = new MultiKey<Guid, Guid, Guid>(question.Set.Standard, question.Standard, sub);

                    subCompetencies.TryGetValue(key, out var count);

                    subCompetencies[key] = count + 1;
                }
            }

            var result = subCompetencies.Select(standardItem =>
                {
                    var setStandard = assets.ContainsKey(standardItem.Key.Key1) ? assets[standardItem.Key.Key1] : null;
                    var questionStandard = assets.ContainsKey(standardItem.Key.Key2) ? assets[standardItem.Key.Key2] : null;
                    var questionSub = assets.ContainsKey(standardItem.Key.Key3) ? assets[standardItem.Key.Key3] : null;

                    return new SubCompetencyItem
                    {
                        SetStandardCode = setStandard?.Code,
                        QuestionStandardCode = questionStandard?.Code,
                        QuestionSubCode = questionSub?.Code,
                        Questions = standardItem.Value
                    };
                })
                .OrderBy(x => x.SetStandardCode)
                .ThenBy(x => x.QuestionStandardCode)
                .ThenBy(x => x.QuestionSubCode)
                .ThenBy(x => x.Questions)
                .ToList();

            return result;
        }

        #endregion

        #region Methods (data binding)

        protected static string GetStandardCode(object value)
        {
            var str = (string)value;

            return string.IsNullOrEmpty(str) ? "<strong>?</strong>" : HttpUtility.HtmlEncode(str);
        }

        private static string GetStandardCodeNoHtml(object value)
        {
            var str = (string)value;
            return string.IsNullOrEmpty(str) ? "?" : str;
        }

        #endregion

        #region Event handlers

        private void QuestionPerTagAndTaxonomyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (TaxItem)e.Item.DataItem;
            var questionPerTaxonomyRepeater = (Repeater)e.Item.FindControl("QuestionPerTaxonomyRepeater");

            questionPerTaxonomyRepeater.DataSource = item.CountPerTaxonomy;
            questionPerTaxonomyRepeater.DataBind();
        }

        private void QuestionPerGacAndTaxonomyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        private void StandardsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var taxonomyRepeater = (Repeater)e.Item.FindControl("TaxonomyRepeater");
            taxonomyRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Taxonomies");
            taxonomyRepeater.DataBind();
        }

        #endregion
    }
}