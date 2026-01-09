using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Models;
using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using BoundField = System.Web.UI.WebControls.BoundField;
using Control = System.Web.UI.Control;
using TemplateField = System.Web.UI.WebControls.TemplateField;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class QuestionCompetencySummaryRepeater : SearchResultsGridViewController<QAttemptFilter>
    {
        #region Classes

        [Serializable]
        private class CandidateColumn : GroupLeaf, IComparable<CandidateColumn>
        {
            public string AreaCode { get; }
            public string AreaName { get; }
            public decimal AreaAverage { get; set; }

            public CandidateColumn(CompetencyArea area)
            {
                AreaCode = area.Code;
                AreaName = area.Title;
                AreaAverage = 0;
            }

            public int CompareTo(CandidateColumn other)
            {
                return string.Compare(AreaCode, other.AreaCode, StringComparison.Ordinal);
            }
        }

        [Serializable]
        private class QuestionCount
        {
            public string StandardCode { get; set; }
            public string QuestionCode { get; set; }

            public Guid QuestionId { get; set; }
            public Guid StandardId { get; set; }
            public int Count { get; set; }
        }

        [Serializable]
        private class CandidateRow : GroupLeaf, IComparable<CandidateRow>
        {
            public string CandidateCode { get; }
            public string CandidateName { get; }
            public int AttemptNumber { get; }
            public bool AttemptIsPassing { get; set; }
            public decimal? AttemptScore { get; set; }

            public string AttemptPassOrFail
            {
                get
                {
                    if (AttemptScore == null)
                        return string.Empty;

                    if (AttemptIsPassing)
                        return $"<span class='badge bg-success'>Pass</span>";
                    else
                        return $"<span class='badge bg-danger'>Fail</span>";
                }
            }

            public CandidateRow(AttemptAnalysis.AttemptEntity attempt, VPerson candidate)
            {
                CandidateCode = candidate.PersonCode;
                CandidateName = candidate.UserFullName;
                AttemptNumber = attempt.AttemptNumber;
                AttemptScore = attempt.AttemptScore;
                AttemptIsPassing = attempt.AttemptIsPassing;
            }

            public int CompareTo(CandidateRow other)
            {
                var result = string.Compare(CandidateCode, other.CandidateCode, StringComparison.Ordinal);

                if (result == 0)
                    result = AttemptNumber.CompareTo(other.AttemptNumber);

                return result;
            }
        }

        [Serializable]
        private class CandidateTable : GroupTable<CandidateColumn, CandidateRow, decimal?>
        {

        }

        private class HtmlField : ITemplate
        {
            public string Html { get; set; }

            public void InstantiateIn(Control container)
            {
                container.Controls.Add(new System.Web.UI.WebControls.Literal());
                container.DataBinding += Container_DataBinding;
            }

            private void Container_DataBinding(object sender, EventArgs e)
            {
                var cell = (DataControlFieldCell)sender;
                var literal = (System.Web.UI.WebControls.Literal)cell.Controls[0];

                if (Html != null)
                    literal.Text = Html;
            }
        }

        private class GradeField : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                container.Controls.Add(new System.Web.UI.WebControls.Literal { ID = "Grade" });
                container.DataBinding += Name_DataBinding;
            }

            private void Name_DataBinding(object sender, EventArgs e)
            {
                var cell = (DataControlFieldCell)sender;
                var row = (GridViewRow)cell.NamingContainer;
                if (row.DataItem == null)
                    return;

                var value = (string)DataBinder.Eval(row.DataItem, "Row.AttemptPassOrFail");

                var gradeLiteral = (System.Web.UI.WebControls.Literal)row.FindControl("Grade");
                gradeLiteral.Text = value;
            }
        }

        private class CandidateField : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                container.Controls.Add(new System.Web.UI.WebControls.Literal { ID = "Name" });
                container.DataBinding += Name_DataBinding;
            }

            private void Name_DataBinding(object sender, EventArgs e)
            {
                var cell = (DataControlFieldCell)sender;
                var row = (GridViewRow)cell.NamingContainer;
                if (row.DataItem == null)
                    return;

                var candidateName = DataBinder.Eval(row.DataItem, "Row.CandidateName");
                var candidateCode = DataBinder.Eval(row.DataItem, "Row.CandidateCode");

                var nameLiteral = (System.Web.UI.WebControls.Literal)row.FindControl("Name");
                nameLiteral.Text = $"{candidateName}&nbsp;<span class=\"form-text\">{candidateCode}</span>";
            }
        }

        public interface IQuestion : CompetencyReport.IQuestion
        {
            Guid AttemptIdentifier { get; }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        public int OccupationsCount
        {
            get => (int)(ViewState[nameof(OccupationsCount)] ?? 0);
            set => ViewState[nameof(OccupationsCount)] = value;
        }

        public int FrameworksCount
        {
            get => (int)(ViewState[nameof(FrameworksCount)] ?? 0);
            set => ViewState[nameof(FrameworksCount)] = value;
        }

        public int GacsCount
        {
            get => (int)(ViewState[nameof(GacsCount)] ?? 0);
            set => ViewState[nameof(GacsCount)] = value;
        }

        public int CompetenciesCount
        {
            get => (int)(ViewState[nameof(CompetenciesCount)] ?? 0);
            set => ViewState[nameof(CompetenciesCount)] = value;
        }

        protected StandardSummary.Occupation[] Occupations
        {
            get => (StandardSummary.Occupation[])ViewState[nameof(Occupations)];
            set => ViewState[nameof(Occupations)] = value;
        }

        private CandidateTable Table
        {
            get => (CandidateTable)ViewState[nameof(Table)];
            set => ViewState[nameof(Table)] = value;
        }

        private Form Form { get; set; }
        private List<QuestionCount> QuestionCountList { get; set; }

        private string CandidateFilter
        {
            get => (string)ViewState[nameof(CandidateFilter)];
            set => ViewState[nameof(CandidateFilter)] = value;
        }

        #endregion

        #region Fields

        private CandidateColumn[] _sortedColumns = null;
        private bool _isModifyingGridField = false;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OccupationRepeater.ItemCreated += OccupationRepeater_ItemCreated;
            OccupationRepeater.ItemDataBound += OccupationRepeater_ItemDataBound;

            Grid.RowDataBound += Grid_RowDataBound;

            DownloadButton.Click += DownloadButton_Click;

            LearnerFilterButton.Click += CandidateFilterButton_Click;

            Grid.Columns.FieldsChanged += Columns_FieldsChanged;
        }

        #endregion

        #region Event handlers

        private void Columns_FieldsChanged(object sender, EventArgs e)
        {
            if (_isModifyingGridField)
                return;

            var collection = (DataControlFieldCollection)sender;
            if (collection.Count == 0)
                return;

            _isModifyingGridField = true;

            var field = collection[collection.Count - 1];

            if (collection.Count > 2 && collection.Count <= Table.Columns.Count + 2)
                SetHeaderTemplateIfNull(() =>
                {
                    var columns = GetSortedTableColumns();
                    var column = columns[collection.Count - 3];

                    return new HtmlField { Html = $"<span title='{column.AreaName}'>{column.AreaCode}</span>" };
                });
            else if (collection.Count == 1)
                SetItemTemplateIfNull(() => new CandidateField());
            else if (collection.Count == Table.Columns.Count + 4)
                SetItemTemplateIfNull(() => new GradeField());

            _isModifyingGridField = false;

            void SetItemTemplateIfNull(Func<ITemplate> templateFactory)
            {
                var tField = (TemplateField)field;
                if (tField.ItemTemplate == null)
                    tField.ItemTemplate = templateFactory();
            }

            void SetHeaderTemplateIfNull(Func<ITemplate> templateFactory)
            {
                var tField = (TemplateField)field;
                if (tField.HeaderTemplate == null)
                    tField.HeaderTemplate = templateFactory();
            }
        }

        private void OccupationRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var frameworkRepeater = (Repeater)e.Item.FindControl("FrameworkRepeater");
            frameworkRepeater.ItemCreated += FrameworkRepeater_ItemCreated;
            frameworkRepeater.ItemDataBound += FrameworkRepeater_ItemDataBound;
        }

        private void OccupationRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var occupation = (StandardSummary.Occupation)e.Item.DataItem;
            QuestionCountList = Form != null
                ? BindStandardsRepeater(Form.Specification.Bank, Form.GetQuestions())
                : new List<QuestionCount>();

            var frameworks = occupation.Frameworks.Where(x => x.HasData).OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name);
            foreach (var framework in frameworks)
                framework.Count = QuestionCountList.Sum(x => x.Count);

            var frameworkRepeater = (Repeater)e.Item.FindControl("FrameworkRepeater");
            frameworkRepeater.DataSource = frameworks;
            frameworkRepeater.DataBind();

            FrameworksCount += frameworkRepeater.Items.Count;
        }

        private void FrameworkRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var gacRepeater = (Repeater)e.Item.FindControl("GacRepeater");
            gacRepeater.ItemDataBound += GacRepeater_ItemDataBound;
        }

        private void FrameworkRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var framework = (StandardSummary.Framework)e.Item.DataItem;

            var gacRepeater = (Repeater)e.Item.FindControl("GacRepeater");
            var listToBind = framework.Gacs.Where(x => x.HasData).OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name);

            if (Form != null)
                CountQuestionPerGAC(Form.Specification.Bank, Form.GetQuestions(), listToBind);

            //QuestionCountList = BindStandardsRepeater(Form.Specification.Bank, Form.GetQuestions());

            gacRepeater.DataSource = listToBind;
            gacRepeater.DataBind();

            GacsCount += gacRepeater.Items.Count;
        }

        private void GacRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var gac = (StandardSummary.Gac)e.Item.DataItem;
            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            var listToBind = gac.Competencies.Where(x => x.HasData).OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name);
            CountQuestionPerLIG(listToBind);
            competencyRepeater.DataSource = listToBind;
            competencyRepeater.DataBind();

            CompetenciesCount += competencyRepeater.Items.Count;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e.Row))
                return;

            List<decimal?> values = (List<decimal?>)DataBinder.Eval(e.Row.DataItem, "Values");

            for (int i = 0; i < values.Count; i++)
                e.Row.Cells[i + 2].Text = $"{values[i]:p2}";
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch);
            {
                settings.Filter = Filter.Clone();
                settings.IncludeOnlyFirstAttempt = Filter is AdHocAttemptFilter adHocFilter && adHocFilter.IncludeOnlyFirstAttempt;
            }

            var analysis = AttemptAnalysis.Create(settings);
            if (analysis.HasData)
                SendXlsx(analysis);
        }

        private void CandidateFilterButton_Click(object sender, EventArgs e)
        {
            CandidateFilter = LearnerFilterTextBox.Text;

            Search(Filter);
        }

        #endregion

        #region Methods (search results)

        protected override IListSource SelectData(QAttemptFilter filter)
        {
            var columns = GetSortedTableColumns();

            return Table.Rows
                .Where(x => string.IsNullOrEmpty(CandidateFilter)
                    || x.CandidateName.IndexOf(CandidateFilter, StringComparison.OrdinalIgnoreCase) >= 0
                    || !string.IsNullOrEmpty(x.CandidateCode) && x.CandidateCode.IndexOf(CandidateFilter, StringComparison.OrdinalIgnoreCase) >= 0
                )
                .OrderBy(x => x.CandidateName)
                .ThenBy(x => x.CandidateCode)
                .ApplyPaging(filter)
                .Select(row => new
                {
                    Row = row,
                    Values = columns.Select(col => Table.GetCell(col, row)).ToList(),
                })
                .ToList()
                .ToSearchResult();
        }

        protected override int SelectCount(QAttemptFilter filter)
        {
            return !string.IsNullOrEmpty(CandidateFilter)
                ? Table.Rows
                    .Where(x =>
                        x.CandidateName.IndexOf(CandidateFilter, StringComparison.OrdinalIgnoreCase) >= 0
                        || !string.IsNullOrEmpty(x.CandidateCode) && x.CandidateCode.IndexOf(CandidateFilter, StringComparison.OrdinalIgnoreCase) >= 0
                    )
                    .Count()
                : Table.Rows.Count;
        }

        private void ShowWarningForMissingLearnerCodes(VPerson[] learners)
        {
            var count = learners.Count(x => string.IsNullOrEmpty(x.PersonCode));
            if (count > 0)
                LearnersStatus.AddMessage(AlertType.Warning, $"There are {count} learners in your report who are not assigned a unique account code. These are excluded from the analytics here.");
        }

        #endregion

        #region Methods (data loading)

        public void LoadData(StandardSummary.Occupation[] occupations, AttemptAnalysis analysis, QAttemptFilter filter)
        {
            if (IsPostBack)
                BindOccupations();

            Filter = filter;
            Table = null;

            AddCandidateGridFields();

            Form = analysis.Forms.Count == 1
                ? analysis.Forms.Values.First()
                : null;

            OccupationsCount = 0;
            FrameworksCount = 0;
            GacsCount = 0;
            CompetenciesCount = 0;

            if (!analysis.HasData)
            {
                CompetenciesStatus.AddMessage(AlertType.Information, "No Data.");
                OccupationRepeater.Visible = false;
                return;
            }

            Occupations = occupations;

            BindOccupations();

            OccupationsCount += Occupations.Length;

            LearnersTab.Visible = true;

            _sortedColumns = null;

            Table = GetCandidatesData(analysis);

            AddCandidateGridFields();

            Search(Filter);
        }

        private void CountQuestionPerLIG(IOrderedEnumerable<StandardSummary.Competency> listToBind)
        {
            foreach (var item in listToBind)
            {
                var countItem = QuestionCountList.Where(x => x.QuestionId == item.ID).FirstOrDefault();
                if (countItem != null)
                    item.Count = countItem.Count;
            }
        }

        private void CountQuestionPerGAC(BankState bank, IEnumerable<Question> questions, IOrderedEnumerable<StandardSummary.Gac> listToBind)
        {
            var standards = new Dictionary<Guid, int>();

            foreach (var question in questions)
            {
                if (standards.ContainsKey(question.Set.Standard))
                    standards[question.Set.Standard] += 1;
                else
                    standards.Add(question.Set.Standard, 1);
            }

            foreach (var key in standards.Keys)
            {
                var item = listToBind.Where(x => x.ID == key).FirstOrDefault();
                if (item != null)
                    item.Count = standards[key];
            }
        }

        private List<QuestionCount> BindStandardsRepeater(BankState bank, IEnumerable<Question> questions)
        {
            var assets = BankStatisticHelper.GetBankAssets(bank);
            var taxonomiesIndex = new HashSet<int>();
            var groupedQuestions = new Dictionary<MultiKey<Guid, Guid>, List<Question>>();

            foreach (var question in questions)
            {
                var taxonomy = question.Classification.Taxonomy ?? 0;
                if (!taxonomiesIndex.Contains(taxonomy))
                    taxonomiesIndex.Add(taxonomy);

                var key = new MultiKey<Guid, Guid>(question.Set.Standard, question.Standard);
                if (!groupedQuestions.ContainsKey(key))
                    groupedQuestions.Add(key, new List<Question>());

                groupedQuestions[key].Add(question);
            }

            var results = groupedQuestions.Select(standardItem =>
            {
                var setStandard = assets.ContainsKey(standardItem.Key.Key1) ? assets[standardItem.Key.Key1] : null;
                var questionStandard = assets.ContainsKey(standardItem.Key.Key2) ? assets[standardItem.Key.Key2] : null;
                var questionTaxonomies = standardItem.Value.GroupBy(x => x.Classification.Taxonomy ?? 0).ToDictionary(x => x.Key, x => x.Count());

                return new QuestionCount()
                {
                    StandardCode = setStandard?.Code,
                    QuestionCode = questionStandard?.Code,
                    Count = standardItem.Value.Count,
                    StandardId = standardItem.Key.Key1,
                    QuestionId = standardItem.Key.Key2
                };
            }).OrderBy(x => x.StandardCode).ThenBy(x => x.QuestionCode).ToList();

            return results;
        }

        private void BindOccupations()
        {
            OccupationRepeater.Visible = true;
            OccupationRepeater.DataSource = Occupations;
            OccupationRepeater.DataBind();
        }

        private void AddCandidateGridFields()
        {
            Grid.Columns.Clear();

            if (Table == null)
                return;

            var columns = GetSortedTableColumns();

            var averageField = new TemplateField();
            averageField.HeaderText = "Candidate";
            averageField.FooterText = "Average";
            averageField.FooterStyle.Font.Bold = true;

            Grid.Columns.Add(averageField);
            Grid.Columns.Add(new BoundField { HeaderText = "Attempt", DataField = "Row.AttemptNumber" });

            foreach (var column in columns)
            {
                var field = new TemplateField();
                field.FooterText = $"{column.AreaAverage:p2}";
                field.HeaderStyle.CssClass = "col-right";
                field.FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                field.FooterStyle.Font.Bold = true;
                field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;

                Grid.Columns.Add(field);
            }

            var scoreField = new BoundField();
            scoreField.HeaderText = "Score";
            scoreField.DataField = "Row.AttemptScore";
            scoreField.DataFormatString = "{0:p2}";
            scoreField.HeaderStyle.CssClass = "col-right";
            scoreField.ItemStyle.HorizontalAlign = HorizontalAlign.Right;

            Grid.Columns.Add(scoreField);

            Grid.Columns.Add(new TemplateField { HeaderText = "Grade" });
        }

        private CandidateTable GetCandidatesData(AttemptAnalysis analysis)
        {
            var questions = analysis.Attempts.SelectMany(x => x.Questions).Cast<IQuestion>().ToArray();
            var learners = ServiceLocator.ContactSearch
                .GetPersons(analysis.Attempts.Select(x => x.LearnerUserIdentifier).Distinct(), Organization.OrganizationIdentifier);

            ShowWarningForMissingLearnerCodes(learners);

            var candidates = learners
                .GroupBy(x => x.UserIdentifier)
                .ToDictionary(x => x.Key, x =>
                {
                    var person = x.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier);

                    if (person == null)
                    {
                        person = x.FirstOrDefault();
                        person.PersonCode = null;
                        person.SocialInsuranceNumber = null;
                    }

                    return person;
                });

            var ordered = analysis.Attempts
                .Where(x => candidates.ContainsKey(x.LearnerUserIdentifier))
                .Select(x => new
                {
                    Attempt = x,
                    Candidate = candidates[x.LearnerUserIdentifier]
                })
                .OrderBy(x => x.Candidate.UserFullName).ThenBy(x => x.Attempt.AttemptNumber)
                .ToList();

            var table = new CandidateTable();
            var report = new CompetencyReport(questions);

            var attemptQuestionGroups = questions
                .Where(x => x.CompetencyAreaCode.HasValue())
                .GroupBy(x => x.AttemptIdentifier)
                .ToDictionary
                (
                    x => x.Key,
                    x => x
                        .GroupBy(y => y.CompetencyAreaCode)
                        .ToDictionary(y => y.Key, y => y.ToList())
                );

            foreach (var attempt in ordered)
            {
                if (!attemptQuestionGroups.TryGetValue(attempt.Attempt.AttemptIdentifier, out var attemptQuestions))
                    attemptQuestions = null;

                ProcessAttempt(attempt.Attempt, attempt.Candidate, attemptQuestions, report, table);
            }

            foreach (var col in table.Columns)
                col.AreaAverage = table.Rows.Average(row => table.GetCell(col, row) ?? 0);

            return table;
        }

        private static void ProcessAttempt(
            AttemptAnalysis.AttemptEntity attempt,
            VPerson candidate,
            Dictionary<string, List<IQuestion>> attemptQuestions,
            CompetencyReport report,
            CandidateTable table
            )
        {
            if (candidate.PersonCode.IsEmpty())
                return;

            var row = table.Rows.GetOrAdd(
                () => new CandidateRow(attempt, candidate),
                candidate.PersonCode, attempt.AttemptNumber);

            foreach (var folder in report.Folders)
            {
                if (folder.Code.IsEmpty())
                    continue;

                var column = table.Columns.GetOrAdd(
                    () => new CandidateColumn(folder),
                    folder.Code);

                decimal answerPoints = 0, questionPoints = 0;

                if (attemptQuestions != null && attemptQuestions.TryGetValue(folder.Code, out var areaAttemptQuestions))
                {
                    foreach (var a in areaAttemptQuestions)
                    {
                        answerPoints += a.AnswerPoints ?? 0;
                        questionPoints += a.QuestionPoints ?? 0;
                    }
                }

                table.AddCell(column, row, () => questionPoints != 0 ? answerPoints / questionPoints : 0);
            }
        }

        #endregion

        #region Methods (export)

        private void SendXlsx(AttemptAnalysis analysis)
        {
            using (var excel = new ExcelPackage())
            {
                #region Styles Definition

                var blueColor = ColorTranslator.FromHtml("#265f9f");
                var redColor = ColorTranslator.FromHtml("#c0392b");
                var greenColor = ColorTranslator.FromHtml("#27ae60");

                {
                    var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                    defaultStyle.Font.Name = "Calibri";
                    defaultStyle.Font.Size = 11;
                    defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Header Cell");
                    newStyle.Style.Font.Bold = true;
                    newStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Profile Cell");
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Framework Cell");
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Gac Cell");
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Competency Cell");
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Footer Cell");
                    newStyle.Style.Font.Bold = true;
                    newStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                }

                #endregion

                string filename;

                if (LearnersTab.IsSelected)
                {
                    filename = string.Format("standards-summary-candidates-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

                    AddCandidatesSheet(excel, analysis);
                }
                else
                {
                    filename = string.Format("standards-summary-competencies-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

                    AddCompetenciesSheet(excel, analysis);
                }

                excel.Workbook.Properties.Title = "Standards Summary";
                excel.Workbook.Properties.Company = CurrentSessionState.Identity.Organization.Name;
                excel.Workbook.Properties.Author = CurrentSessionState.Identity.User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                    sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                    sheet.PrinterSettings.FitToPage = true;
                    sheet.PrinterSettings.FitToWidth = 1;
                    sheet.PrinterSettings.FitToHeight = 0;
                }

                var dataBytes = excel.GetAsByteArray();

                Page.Response.SendFile(filename, "xlsx", dataBytes);
            }
        }

        private void AddCompetenciesSheet(ExcelPackage excel, AttemptAnalysis analysis)
        {
            var data = StandardSummary.GetData(analysis, Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection);
            var sheet = excel.Workbook.Worksheets.Add("Competencies");

            sheet.Column(1).Width = 80;
            sheet.Column(2).Width = 10;

            var rowNumber = 1;

            {
                var cell = sheet.Cells[rowNumber, 1, rowNumber, 2];
                cell.Merge = true;
                cell.Value = $"{analysis.Attempts.Count:n0} completed exam attempts";
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            }

            rowNumber++;

            foreach (var occupationInfo in data)
            {
                var occupationCell1 = sheet.Cells[rowNumber, 1];
                occupationCell1.StyleName = "Profile Cell";
                occupationCell1.Value = occupationInfo.Name;

                var occupationCell2 = sheet.Cells[rowNumber, 2];
                occupationCell2.StyleName = "Profile Cell";
                occupationCell2.Style.Numberformat.Format = "#0%";
                occupationCell2.Value = occupationInfo.Score;

                rowNumber++;

                foreach (var frameworkInfo in occupationInfo.Frameworks.Where(x => x.HasData).OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name))
                {
                    var frameworkCell1 = sheet.Cells[rowNumber, 1];
                    frameworkCell1.StyleName = "Framework Cell";
                    frameworkCell1.Value = frameworkInfo.Name;

                    var frameworkCell2 = sheet.Cells[rowNumber, 2];
                    frameworkCell2.StyleName = "Framework Cell";
                    frameworkCell2.Style.Numberformat.Format = "#0%";
                    frameworkCell2.Value = frameworkInfo.Score;

                    rowNumber++;

                    foreach (var gacInfo in frameworkInfo.Gacs.Where(x => x.HasData).OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name))
                    {
                        var gacCell1 = sheet.Cells[rowNumber, 1];
                        gacCell1.StyleName = "Gac Cell";
                        gacCell1.Value = gacInfo.Name;

                        var gacCell2 = sheet.Cells[rowNumber, 2];
                        gacCell2.StyleName = "Gac Cell";
                        gacCell2.Style.Numberformat.Format = "#0%";
                        gacCell2.Value = gacInfo.Score;

                        rowNumber++;

                        foreach (var competencyInfo in gacInfo.Competencies.Where(x => x.HasData).OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name))
                        {
                            var competencyCell1 = sheet.Cells[rowNumber, 1];
                            competencyCell1.StyleName = "Competency Cell";
                            competencyCell1.Value = competencyInfo.Name;

                            var competencyCell2 = sheet.Cells[rowNumber, 2];
                            competencyCell2.StyleName = "competency Cell";
                            competencyCell2.Style.Numberformat.Format = "#0%";
                            competencyCell2.Value = competencyInfo.Score;

                            rowNumber++;
                        }
                    }
                }
            }
        }

        private void AddCandidatesSheet(ExcelPackage excel, AttemptAnalysis analysis)
        {
            var table = Table;
            var columns = table.Columns.OrderBy(x => x.AreaCode).ToArray();
            var sheet = excel.Workbook.Worksheets.Add("Candidates");

            sheet.Column(1).Width = 30;
            sheet.Column(2).Width = 20;

            for (var i = 0; i < columns.Length + 2; i++)
                sheet.Column(3 + i).Width = 10;

            var rowNumber = 1;

            {
                var cell = sheet.Cells[rowNumber, 1, rowNumber, 4 + columns.Length];
                cell.Merge = true;
                cell.Value = $"{analysis.Attempts.Count:n0} completed exam attempts";
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            }

            rowNumber++;

            {
                var cell1 = sheet.Cells[rowNumber, 1];
                cell1.Value = "Candidate Name";
                cell1.StyleName = "Header Cell";

                var cell2 = sheet.Cells[rowNumber, 2];
                cell2.Value = "Candidate Code";
                cell2.StyleName = "Header Cell";

                for (var i = 0; i < columns.Length; i++)
                {
                    var cellN0 = sheet.Cells[rowNumber, 3 + i];
                    cellN0.Value = columns[i].AreaCode;
                    cellN0.StyleName = "Header Cell";
                    cellN0.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                var cellN1 = sheet.Cells[rowNumber, 2 + columns.Length + 1];
                cellN1.Value = "Score";
                cellN1.StyleName = "Header Cell";
                cellN1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                var cellN2 = sheet.Cells[rowNumber, 2 + columns.Length + 2];
                cellN2.Value = "Grade";
                cellN2.StyleName = "Header Cell";
            }

            rowNumber++;

            foreach (var row in table.Rows.OrderBy(x => x.CandidateName).ThenBy(x => x.CandidateCode))
            {
                var cell1 = sheet.Cells[rowNumber, 1];
                cell1.Value = row.CandidateName;

                var cell2 = sheet.Cells[rowNumber, 2];
                cell2.Value = row.CandidateCode;

                for (var i = 0; i < columns.Length; i++)
                {
                    var col = columns[i];
                    var value = table.GetCell(col, row);

                    var cellN0 = sheet.Cells[rowNumber, 3 + i];
                    cellN0.Value = value;
                    cellN0.Style.Numberformat.Format = "#0.00%";
                }

                var cellN1 = sheet.Cells[rowNumber, 2 + columns.Length + 1];
                cellN1.Value = row.AttemptScore;
                cellN1.Style.Numberformat.Format = "#0.00%";

                var cellN2 = sheet.Cells[rowNumber, 2 + columns.Length + 2];
                cellN2.Value = !row.AttemptScore.HasValue
                    ? string.Empty
                    : row.AttemptIsPassing
                        ? "Pass"
                        : "Fail";

                rowNumber++;
            }

            {
                var cell1_2 = sheet.Cells[rowNumber, 1, rowNumber, 2];
                cell1_2.Merge = true;
                cell1_2.StyleName = "Footer Cell";
                cell1_2.Value = "Average";

                for (var i = 0; i < columns.Length; i++)
                {
                    var col = columns[i];

                    var cellN0 = sheet.Cells[rowNumber, 3 + i];
                    cellN0.Value = col.AreaAverage;
                    cellN0.StyleName = "Footer Cell";
                    cellN0.Style.Numberformat.Format = "#0.00%";
                }

                var cellN1_2 = sheet.Cells[rowNumber, 2 + columns.Length + 1, rowNumber, 2 + columns.Length + 2];
                cellN1_2.Merge = true;
                cellN1_2.StyleName = "Footer Cell";
            }
        }

        #endregion

        #region Methods (helpers)

        protected static string GetScoreText(decimal score)
        {
            var color = score > 0.75m ? "success" : (score > 0.5m ? "warning" : "danger");
            return $"<span class='text-{color}'>{score:p0}</span>";
        }

        protected static string GetScoreLabel(decimal score)
        {
            var color = score > 0.75m ? "success" : (score > 0.50m ? "warning" : "danger");
            return $"<span class='badge bg-{color}'>{score:p0}</span>";
        }

        protected static string GetPoints(decimal answerPoints, decimal questionPoints) =>
            $"{answerPoints:n2} / {questionPoints:n2}";

        private CandidateColumn[] GetSortedTableColumns()
        {
            if (_sortedColumns == null)
                _sortedColumns = Table.Columns.OrderBy(x => x.AreaCode).ThenBy(x => x.AreaName).ToArray();

            return _sortedColumns;
        }

        #endregion
    }
}