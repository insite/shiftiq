using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using BoundField = System.Web.UI.WebControls.BoundField;

namespace InSite.Cmds.Actions.Reports
{
    public partial class CompetencyCountPerCategory : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        private class GroupItem
        {
            public string GroupName { get; set; }
            public List<CompetencyCategory> List { get; set; }
        }

        #endregion

        #region Constants

        private static readonly ICollection<string> AllowedFilterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "keyword",
            "category",
            "profile"
        };
        private readonly string DefaultFilterName = "keyword";

        #endregion

        #region Properties

        private string Keyword
        {
            get => (string)ViewState[nameof(Keyword)];
            set => ViewState[nameof(Keyword)] = value;
        }

        private string Category
        {
            get => (string)ViewState[nameof(Category)];
            set => ViewState[nameof(Category)] = value;
        }

        private Guid? ProfileStandardIdentifier
        {
            get => (Guid?)ViewState[nameof(ProfileStandardIdentifier)];
            set => ViewState[nameof(ProfileStandardIdentifier)] = value;
        }

        private string ProfileNumber
        {
            get => (string)ViewState[nameof(ProfileNumber)];
            set => ViewState[nameof(ProfileNumber)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
                InitControls();

            AdvancedSearchUpdatePanel.Request += AdvancedSearchUpdatePanel_Request;

            SearchButton.Click += (s, a) => InitData();

            SearchAdvancedButton.Click += SearchAdvancedButton_Click;

            DownloadXlsx.Click += (s, a) => SendXlsx();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            InitData();
        }

        #endregion

        #region Event handlers

        private void AdvancedSearchUpdatePanel_Request(object sender, StringValueArgs e)
        {
            CategorySelector.Value = Category;
            KeywordText.Text = Keyword;
            ProfileSelector.Value = ProfileStandardIdentifier;
        }

        private void SearchAdvancedButton_Click(object sender, EventArgs e)
        {
            Category = CategorySelector.Value;
            Keyword = KeywordText.Text;
            ProfileStandardIdentifier = ProfileSelector.Value;
            ProfileNumber = ProfileStandardIdentifier.HasValue ? ProfileRepository.Select(ProfileStandardIdentifier.Value).ProfileNumber : null;

            SearchText.Text = GetFilterText();

            InitData();
        }

        #endregion

        #region Helper methods

        private void InitData()
        {
            InitFilter();

            var groups = CreateGroups();

            foreach (var group in groups)
            {
                var header = new HtmlGenericControl();
                header.TagName = "h3";
                header.InnerHtml = group.GroupName;
                ReportOutput.Controls.Add(header);

                foreach (var item in group.List)
                {
                    if (!string.IsNullOrEmpty(item.CompetencyKnowledge))
                        item.CompetencyKnowledge = Markdown.ToHtml(item.CompetencyKnowledge);

                    if (!string.IsNullOrEmpty(item.CompetencySkills))
                        item.CompetencySkills = Markdown.ToHtml(item.CompetencySkills);

                    if (!string.IsNullOrEmpty(item.CompetencyAchievements))
                        item.CompetencyAchievements = Markdown.ToHtml(item.CompetencyAchievements);
                }

                var gv = new GridView()
                {
                    AutoGenerateColumns = false,
                    CssClass = "table table-striped table-report",
                };

                var competencyNumberField = new BoundField() { HeaderText = "#", DataField = "CompetencyNumber" };
                competencyNumberField.ItemStyle.Width = Unit.Percentage(8);

                var competencyField = new BoundField() { HeaderText = "Competency Statement", DataField = "CompetencySummary" };
                competencyField.ItemStyle.Width = Unit.Percentage(26);

                var knowledgeField = new BoundField() { HeaderText = "Knowledge", DataField = "CompetencyKnowledge", HtmlEncode = false };
                knowledgeField.ItemStyle.Width = Unit.Percentage(26);

                var skillsField = new BoundField() { HeaderText = "Skills", DataField = "CompetencySkills", HtmlEncode = false };
                skillsField.ItemStyle.Width = Unit.Percentage(26);

                var achievementsField = new BoundField() { HeaderText = "Achievements", DataField = "CompetencyAchievements", HtmlEncode = false };
                achievementsField.ItemStyle.Width = Unit.Percentage(20);

                gv.Columns.Add(competencyNumberField);
                gv.Columns.Add(competencyField);
                gv.Columns.Add(knowledgeField);
                gv.Columns.Add(skillsField);
                gv.Columns.Add(achievementsField);

                gv.DataSource = group.List;
                gv.DataBind();

                gv.HeaderRow.TableSection = TableRowSection.TableHeader;

                ReportOutput.Controls.Add(gv);
            }
        }

        private void InitControls()
        {
            CategorySelector.LoadItems(
                CompetencyRepository.SelectCompetencyCategories(null)
                    .Select(cc => cc.CategoryName).Distinct().OrderBy(c => c));
        }

        private IList<GroupItem> CreateGroups()
        {
            var data = ApplyFilter();

            return data
                .GroupBy(x => x.CategoryName)
                .OrderBy(x => x.Key)
                .Select(x => new GroupItem
                {
                    GroupName = $"{x.Key} ({x.Count()})",
                    List = x.OrderBy(y => y.CompetencySummary).ToList()
                })
                .ToList();
        }

        private IList<CompetencyCategory> ApplyFilter()
        {
            var data = CompetencyRepository.SelectCompetencyCategories(ProfileStandardIdentifier);

            if (!String.IsNullOrEmpty(Keyword))
            {
                data = data.Where(x =>
                    x.CompetencySummary.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    || x.CompetencyKnowledge != null && x.CompetencyKnowledge.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    || x.CompetencySkills != null && x.CompetencySkills.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            if (!String.IsNullOrEmpty(Category))
                data = data.Where(x => String.Equals(x.CategoryName, Category, StringComparison.OrdinalIgnoreCase));

            return data.ToList();
        }

        private string GetFilterText()
        {
            var items = new List<SimpleFilterHelper.FilterItem>();

            if (!String.IsNullOrEmpty(Keyword)) items.Add(new SimpleFilterHelper.FilterItem { Name = "keyword", Value = Keyword });
            if (!String.IsNullOrEmpty(Category)) items.Add(new SimpleFilterHelper.FilterItem { Name = "category", Value = Category });
            if (!String.IsNullOrEmpty(ProfileNumber)) items.Add(new SimpleFilterHelper.FilterItem { Name = "profile", Value = ProfileNumber });

            return SimpleFilterHelper.ConvertToFilter(items);
        }

        private void InitFilter()
        {
            var filterText = SearchText.Text;

            Keyword = null;
            Category = null;

            if (!String.IsNullOrEmpty(filterText))
            {
                var filters = SimpleFilterHelper.Parse(filterText, AllowedFilterNames, DefaultFilterName);

                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        string filterValue = filter.Value;

                        switch (filter.Name.ToLower())
                        {
                            case "keyword":
                                Keyword = filterValue;
                                break;
                            case "category":
                                Category = filterValue;
                                break;
                            case "profile":
                                ProfileNumber = filterValue;
                                ProfileStandardIdentifier = ProfileRepository.Select(filterValue)?.ProfileStandardIdentifier;
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods (export XLSX)

        private void SendXlsx()
        {
            const string name = "Competency Count per Category";

            var dataSource = CreateGroups();
            if (dataSource == null || !dataSource.Any())
                return;

            var helper = new XlsxExportHelper();
            helper.Map("CompetencyNumber", "Competency #", null, 20, HorizontalAlignment.Left);
            helper.Map("CompetencySummary", "Competency Statement", null, 70, HorizontalAlignment.Left);
            helper.Map("CompetencyKnowledge", "Knowledge", null, 70, HorizontalAlignment.Left);
            helper.Map("CompetencySkills", "Skills", null, 70, HorizontalAlignment.Left);
            helper.Map("CompetencyAchievements", "Achievements", null, 70, HorizontalAlignment.Left);

            var bytes = helper.GetXlsxBytes(excel =>
            {
                var sheet = excel.Workbook.Worksheets.Add(Route.Title);
                sheet.Cells.Style.WrapText = true;

                var row = 1;

                foreach (var group in dataSource)
                {
                    var groupCells = sheet.Cells[row, 1, row, 5];
                    groupCells.Merge = true;
                    groupCells.Value = group.GroupName;
                    groupCells.StyleName = XlsxExportHelper.HeaderStyleName;

                    row++;

                    helper.InsertHeader(sheet, row, 1);

                    row++;

                    row += helper.InsertData(sheet, group.List, row, 1);

                    var separatorCells = sheet.Cells[row, 1, row, 5];
                    separatorCells.Merge = true;

                    row++;
                }

                helper.ApplyColumnWidth(sheet, 1, true);
            });

            ReportXlsxHelper.ExportToXlsx(name, bytes);
        }

        #endregion
    }
}