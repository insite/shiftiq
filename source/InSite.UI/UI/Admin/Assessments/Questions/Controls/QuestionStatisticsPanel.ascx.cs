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
using Shift.Contract;
using Shift.Toolbox;

using Literal = System.Web.UI.WebControls.Literal;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionStatisticsPanel : BaseUserControl
    {
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
            var statistics = FormWorkshop.QuestionStatisticsCreator.Create(questions, GetBankStandardCodes(bank), Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection);

            TagsTaxonomyRepeater.DataSource = statistics.QuestionPerTaxonomyArray;
            TagsTaxonomyRepeater.DataBind();

            QuestionPerTagAndTaxonomyRepeater.DataSource = statistics.TagAndTaxonomyArray;
            QuestionPerTagAndTaxonomyRepeater.DataBind();

            QuestionPerTaxonomyRepeater.DataSource = statistics.QuestionPerTaxonomyArray;
            QuestionPerTaxonomyRepeater.DataBind();

            QuestionPerDifficultyRepeater.DataSource = statistics.QuestionPerDifficultyArray;
            QuestionPerDifficultyRepeater.DataBind();

            QuestionPerLigRepeater.DataSource = statistics.QuestionPerLIGArray;
            QuestionPerLigRepeater.DataBind();

            QuestionPerGacAndTaxonomyRepeater.DataSource = statistics.QuestionPerGACArray;
            QuestionPerGacAndTaxonomyRepeater.DataBind();

            QuestionCodeRepeater.DataSource = statistics.QuestionPerCodeArray;
            QuestionCodeRepeater.DataBind();

            StandardsTaxonomyRepeater.DataSource = statistics.Taxonomies;
            StandardsTaxonomyRepeater.DataBind();

            StandardsRepeater.DataSource = statistics.Standards;
            StandardsRepeater.DataBind();

            SubCompetenciesRepeater.Visible = statistics.SubCompetencies.Length > 0;
            SubCompetenciesRepeater.DataSource = statistics.SubCompetencies;
            SubCompetenciesRepeater.DataBind();

            return statistics.QuestionPerTagAndTaxonomyArray.Length > 0
                   || statistics.QuestionPerTaxonomyArray.Length > 0
                   || statistics.QuestionPerDifficultyArray.Length > 0
                   || statistics.QuestionPerGACArray.Length > 0
                   || statistics.QuestionPerLIGArray.Length > 0
                   || statistics.Standards.Length > 0;
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
            var statistics = FormWorkshop.QuestionStatisticsCreator.Create(questions, GetBankStandardCodes(bank), Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection);

            var sheets = new List<XlsxWorksheet>();

            AddQuestionPerTaxonomySheet(sheets, statistics.QuestionPerTaxonomyArray);
            AddQuestionPerDifficultySheet(sheets, statistics.QuestionPerDifficultyArray);
            AddQuestionPerLigSheet(sheets, statistics.QuestionPerLIGArray);
            AddQuestionPerGacAndTaxonomySheet(sheets, statistics.QuestionPerGACArray);
            AddQuestionPerCodeSheet(sheets, statistics.QuestionPerCodeArray);
            AddStandardsSheet(sheets, statistics.Taxonomies, statistics.Standards);
            AddSubCompetenciesSheet(sheets, statistics.SubCompetencies);
            AddTagsSheet(sheets, statistics.QuestionPerTaxonomyArray, statistics.TagAndTaxonomyArray);

            return XlsxWorksheet.GetBytes(sheets.ToArray());
        }

        private static void AddQuestionPerTaxonomySheet(List<XlsxWorksheet> sheets, FormWorkshop.QuestionStatistics.QuestionPerIntItem[] table)
        {
            if (table.Length == 0)
                return;

            var sheet = new XlsxWorksheet("Taxonomy");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Taxonomy", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (var row in table)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Item });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.Count, Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerDifficultySheet(List<XlsxWorksheet> sheets, FormWorkshop.QuestionStatistics.QuestionPerIntItem[] table)
        {
            if (table.Length == 0)
                return;

            var sheet = new XlsxWorksheet("Difficulties");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Difficulty", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (var row in table)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Item });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.Count, Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerLigSheet(List<XlsxWorksheet> sheets, FormWorkshop.QuestionStatistics.QuestionPerStringItem[] table)
        {
            if (table.Length == 0)
                return;

            var sheet = new XlsxWorksheet("Like Item Groups");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Like Item Group", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (var row in table)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Item });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.Count, Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerGacAndTaxonomySheet(List<XlsxWorksheet> sheets, FormWorkshop.QuestionStatistics.QuestionPerStringItem[] table)
        {
            if (table.Length == 0)
                return;

            var sheet = new XlsxWorksheet("Standards (GAC)");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Standard", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (var row in table)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Item });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.Count, Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddQuestionPerCodeSheet(List<XlsxWorksheet> sheets, FormWorkshop.QuestionStatistics.QuestionPerStringItem[] table)
        {
            if (table.Length == 0)
                return;

            var sheet = new XlsxWorksheet("Codes");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Code", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            var rowIndex = 1;

            foreach (var row in table)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Item });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.Count, Style = RightStyle });

                rowIndex++;
            }

            sheets.Add(sheet);
        }

        private static void AddStandardsSheet(List<XlsxWorksheet> sheets, int[] taxonomies, FormWorkshop.QuestionStatistics.AssessmentStandard[] standards)
        {
            if (standards.Length == 0)
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

        private static void AddSubCompetenciesSheet(List<XlsxWorksheet> sheets, FormWorkshop.QuestionStatistics.SubCompetency[] standards)
        {
            if (standards.Length == 0)
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

        private static void AddTagsSheet(
            List<XlsxWorksheet> sheets,
            FormWorkshop.QuestionStatistics.QuestionPerIntItem[] questionPerTaxonomy,
            FormWorkshop.QuestionStatistics.TagAndTaxonomy[] taxonomies
            )
        {
            if (questionPerTaxonomy.Length == 0)
                return;

            var sheet = new XlsxWorksheet("Tags");
            sheet.Columns[0].Width = 30;
            sheet.Columns[1].Width = 30;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Tag", Style = BoldStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Questions", Style = BoldRightStyle });

            for (int i = 0; i < questionPerTaxonomy.Length; i++)
            {
                var row = questionPerTaxonomy[i];

                sheet.Columns[2 + i].Width = 30;
                sheet.Cells.Add(new XlsxCell(2 + i, 0) { Value = $"Tax {row.Item}", Style = BoldRightStyle });
            }

            var rowIndex = 1;

            foreach (var item in taxonomies)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = item.Tag });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = item.CountPerTaxonomy.Sum(), Style = RightStyle });

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

            StandardsRepeater.ItemDataBound += StandardsRepeater_ItemDataBound;
        }

        #endregion

        #region Methods (data binding)

        private static Dictionary<Guid, string> GetBankStandardCodes(BankState bank)
        {
            var assets = BankStatisticHelper.GetBankAssets(bank);
            return assets.ToDictionary(x => x.Key, x => x.Value.Code);
        }

        protected string HtmlEncode(object s)
        {
            var text = s?.ToString();
            return !string.IsNullOrEmpty(text) ? HttpUtility.HtmlEncode(text) : null;
        }

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

            var item = (FormWorkshop.QuestionStatistics.TagAndTaxonomy)e.Item.DataItem;
            var questions = (Literal)e.Item.FindControl("Questions");
            var questionPerTaxonomyRepeater = (Repeater)e.Item.FindControl("QuestionPerTaxonomyRepeater");

            questions.Text = $"{item.CountPerTaxonomy.Sum():n0}";

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