using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Banks;
using InSite.Admin.Assessments.Options.Controls;
using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using Literal = System.Web.UI.WebControls.Literal;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionRepeater : BaseUserControl
    {
        #region Constants

        public const string UnpinText = "Unpin";
        public const string PinText = "Pin";
        public const string UnpinClass = "far fa fa-thumbtack";
        public const string PinClass = "far fa fa-thumbtack fa-rotate-90";

        #endregion

        #region Classes

        public class BindSettings
        {
            public Guid? BankIdentifier { get; set; }
            public Guid? OrganizationIdentifier { get; set; }
            public Guid? FormIdentifier { get; set; }
            public Func<PinModel> GetPinModel { get; set; }
            public Func<Question, int> GetBankIndex { get; set; }
            public Func<Question, int> GetFormIndex { get; set; }
            public bool AllowEdit { get; set; }
            public bool AllowAnalyse { get; set; }
            public PropertiesVisibility ShowProperties { get; set; } = PropertiesVisibility.None;
            public ReturnUrl ReturnUrl { get; set; }
            public Func<Question, string> GetChangeUrl { get; set; }
            public Func<Question, string> GetRemoveUrl { get; set; }
            public Func<Question, string> GetAnanlysisUrl { get; set; }

            public BindSettings()
            {
                GetChangeUrl = q => $"/ui/admin/assessments/questions/change?bank={(BankIdentifier ?? q.Set?.Bank?.Identifier)}&question={q.Identifier}";
                GetRemoveUrl = q => $"/admin/assessments/questions/delete?bank={(BankIdentifier ?? q.Set?.Bank?.Identifier)}&question={q.Identifier}";
                GetAnanlysisUrl = q => $"/ui/admin/assessments/questions/analysis?bank={(BankIdentifier ?? q.Set?.Bank?.Identifier)}&question={q.Identifier}";
            }

            public string GetRedirectUrl(string format, params object[] args)
            {
                var url = string.Format(format, args);
                return ReturnUrl == null ? url : ReturnUrl.GetRedirectUrl(url, "question");
            }

            public bool ShowDeleteLink(Question q)
            {
                if (!AllowEdit)
                    return false;

                if (q.PublicationStatus == PublicationStatus.Published && Organization.Toolkits.Assessments.LockPublishedQuestions)
                    return false;

                return true;
            }

            public bool ShowEditLink(Question q)
            {
                if (!AllowEdit)
                    return false;

                if (q.PublicationStatus == PublicationStatus.Published && Organization.Toolkits.Assessments.LockPublishedQuestions)
                    return false;

                return true;
            }
        }

        #endregion

        #region Properties

        private PinModel PinModel { get; set; }

        protected BindSettings CurrentSettings { get; set; }

        #endregion

        #region Fields

        private Dictionary<Guid, QBankQuestion> _sources = null;
        private Dictionary<Guid, Guid> _competencyAreas = null;
        private Dictionary<Guid, QRubric> _rubrics = null;
        private Dictionary<Guid, Guid[]> _gradeItems = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;

            GridStyle.ContentKey = GetType().FullName;
        }

        #endregion

        #region Event handlers

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var question = (Question)e.Item.DataItem;

            var optionRepeater = (OptionReadRepeater)e.Item.FindControl("OptionRepeater");
            e.Item.FindControl("OptionRepeaterSection").Visible = optionRepeater.LoadData(question);

            var allowPin = PinModel != null && CurrentSettings.AllowEdit;

            var pinLink = (LinkButton)e.Item.FindControl("PinLink");
            pinLink.Visible = allowPin;

            if (allowPin)
            {
                var pinned = PinModel.FieldAssetNumbers.Contains(question.Asset);

                pinLink.ToolTip = pinned ? UnpinText : PinText;
                pinLink.OnClientClick = $"bankRead.forms.pinField(this, {question.Asset}); return false;";
                pinLink.Text = $"<i class='{(pinned ? UnpinClass : PinClass)}'></i>";
            }

            var tagsField = e.Item.FindControl("TagsField");
            tagsField.Visible = question.Classification.Tags.IsNotEmpty();

            var snippet = (Web.UI.AssetTitleDisplay)e.Item.FindControl("StandardDisplay");
            snippet.ShowParent = question.Standard != Guid.Empty
                && _competencyAreas.ContainsKey(question.Standard)
                && _competencyAreas[question.Standard] != question.Set.Standard;

            BindGradeItems(e);
            BindRubric(e);
        }

        private void BindGradeItems(RepeaterItemEventArgs e)
        {
            var question = (Question)e.Item.DataItem;
            var gradeItemsRow = e.Item.FindControl("GradeItemsRow");

            if (!_gradeItems.TryGetValue(question.Identifier, out var gradeItemIds))
            {
                gradeItemsRow.Visible = false;
                return;
            }

            var gradeItems = ServiceLocator.RecordSearch
                .GetGradeItems(new QGradeItemFilter { GradeItemIdentifiers = gradeItemIds })
                .Select(x => new
                {
                    Name = StringHelper.BreakHtml(StringHelper.StripHtml(x.GradeItemName))
                })
                .ToList();

            if (gradeItems.Count == 0)
            {
                gradeItemsRow.Visible = false;
                return;
            }

            if (gradeItems.Count == 1)
            {
                ((Literal)e.Item.FindControl("GradeItemName")).Text = gradeItems[0].Name;
                return;
            }

            var repeater = (Repeater)e.Item.FindControl("GradeItemList");
            repeater.DataSource = gradeItems;
            repeater.DataBind();
        }

        private void BindRubric(RepeaterItemEventArgs e)
        {
            var question = (Question)e.Item.DataItem;
            var rubricRow = e.Item.FindControl("RubricRow");

            if (!_rubrics.TryGetValue(question.Identifier, out var rubric))
            {
                rubricRow.Visible = false;
                return;
            }

            var rubricName = (Literal)e.Item.FindControl("RubricTitle");
            rubricName.Text = rubric.RubricTitle;
        }

        #endregion

        #region Methods

        public void LoadData(IEnumerable<Question> questions, BindSettings settings = null)
        {
            CurrentSettings = settings ?? new BindSettings();

            if (CurrentSettings.GetPinModel == null)
                CurrentSettings.GetPinModel = () => null;

            if (CurrentSettings.GetBankIndex == null)
                CurrentSettings.GetBankIndex = q => q.BankIndex;

            if (CurrentSettings.GetFormIndex == null)
                CurrentSettings.GetFormIndex = q => -1;

            PinModel = CurrentSettings.GetPinModel();

            _sources = ServiceLocator.BankSearch
                .GetQuestions(questions.Where(x => x.Source.HasValue).Select(x => x.Source.Value).Distinct())
                .ToDictionary(x => x.QuestionIdentifier);

            var competencies = questions.Select(x => x.Standard).Distinct();
            _competencyAreas = StandardSearch.Bind(x => new { x.StandardIdentifier, x.ParentStandardIdentifier },
                x => x.ParentStandardIdentifier.HasValue
                  && competencies.Any(competency => competency == x.StandardIdentifier))
                .ToDictionary(x => x.StandardIdentifier, x => x.ParentStandardIdentifier.Value);

            var questionIds = questions.Select(x => x.Identifier).ToList();

            _rubrics = ServiceLocator.RubricSearch.GetQuestionRubrics(questionIds);

            LoadGradeItems(questionIds);

            Repeater.DataSource = questions;
            Repeater.DataBind();
        }

        private void LoadGradeItems(IEnumerable<Guid> questionIds)
        {
            var entities = ServiceLocator.BankSearch.GetQuestionGradeItems(questionIds);

            _gradeItems = entities
                .GroupBy(x => x.QuestionIdentifier)
                .ToDictionary(
                    x => x.Key,
                    entry => entry.Select(gradeItem => gradeItem.GradeItemIdentifier).ToArray()
                );
        }

        #endregion

        #region Methods (question repeater helpers)

        protected string GetEnumDescription(Enum value) => value.GetDescription();

        protected string GetFlagHtml(FlagType flag)
        {
            return flag.ToIconHtml();
        }

        protected string DisplayBankSequence(object item)
        {
            return (CurrentSettings.GetBankIndex((Question)item) + 1).ToString();
        }

        protected string DisplayFormSequence(object item)
        {
            var formIndex = CurrentSettings.GetFormIndex((Question)item);

            return formIndex >= 0 ? $"<span class='badge rounded-pill bg-custom-default'>{1 + formIndex}</span>" : null;
        }

        protected static string DisplayRationale(object item)
        {
            Question q = (Question)item;

            if (q.Content == null)
                return null;

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(q.Content.Rationale?.Default))
                sb.Append($"<div class='alert alert-info'><i class='fas fa-info-square'></i> <strong>Feedback to all candidates:</strong> {Markdown.ToHtml(q.Content.Rationale.Default)}</div>");

            if (!string.IsNullOrEmpty(q.Content.RationaleOnCorrectAnswer?.Default))
                sb.Append($"<div class='alert alert-success'><i class='fas fa-check-square'></i> <strong>Feedback on correct answers:</strong> {Markdown.ToHtml(q.Content.RationaleOnCorrectAnswer.Default)}</div>");

            if (!string.IsNullOrEmpty(q.Content.RationaleOnIncorrectAnswer?.Default))
                sb.Append($"<div class='alert alert-danger'><i class='fas fa-times-square'></i> <strong>Feedback on incorrect answers:</strong> {Markdown.ToHtml(q.Content.RationaleOnIncorrectAnswer.Default)}</div>");

            return sb.ToString();
        }

        protected string GetCommentsSummary(object obj)
        {
            if (!(obj is IEnumerable<Comment> comments))
                return string.Empty;

            var html = new StringBuilder();

            html.Append("<table class='w-100'><tbody>");
            foreach (var group in comments.GroupBy(x => x.Flag).OrderBy(x => x.Key))
            {
                html
                    .AppendFormat("<tr><td>{0} <span class='form-text'>{1}</span>", group.Key.ToIconHtml(), group.Key.GetDescription())
                    .Append("</td><td class='text-end'>").Append(group.Count().ToString("n0"))
                    .Append("</td></tr>");
            }
            html.Append("</tbody></table>");

            html.Append("<table class='w-100'><tbody>");
            foreach (var group in comments.Where(x => x.Category != null).GroupBy(x => x.Category).OrderBy(x => x.Key))
            {
                if (string.IsNullOrEmpty(group.Key))
                    continue;
                html
                    .AppendFormat("<tr><td><span class='badge bg-custom-default'>{0}</span>", group.Key)
                    .Append("</td><td class='text-end'>").Append(group.Count().ToString("n0"))
                    .Append("</td></tr>");
            }
            html.Append("</tbody></table>");

            return html.ToString();
        }

        protected string GetSourceLink()
        {
            var source = (Guid?)Eval("Source");
            if (!source.HasValue || !_sources.TryGetValue(source.Value, out var sourceQuestion))
                return "<span class='text-danger'>Not Found</span>";

            var url = $"/ui/admin/assessments/questions/analysis?bank={sourceQuestion.BankIdentifier}&question={sourceQuestion.QuestionIdentifier}";
            if (CurrentSettings.ReturnUrl != null)
                url = CurrentSettings.ReturnUrl.GetRedirectUrl(url, $"bank={Eval("Set.Bank.Identifier")}&question={Eval("Identifier")}");

            return $"<a href='{url}'>{sourceQuestion.QuestionAssetNumber}</a>";
        }

        protected decimal? GetPoints()
        {
            var question = (Question)Page.GetDataItem();

            return _rubrics.TryGetValue(question.Identifier, out var rubric)
                ? rubric.RubricPoints
                : question.Points;
        }

        protected string GetLanguages()
        {
            var question = (Question)Page.GetDataItem();

            var langs = question.Content.Languages.Distinct()
                .Where(x => Language.CodeExists(x))
                .Select(x => Language.GetDisplayName(x))
                .OrderBy(x => x)
                .ToArray();

            if (langs.IsEmpty())
                langs = new[] { Language.GetDisplayName(Language.Default) };

            return string.Join(", ", langs);
        }

        #endregion

        #region Methods (questions filtering)

        internal static readonly ICollection<string> AllowedFilterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "asset",
            "code",
            "difficulty",
            "flag",
            "keyword",
            "lig",
            "reference",
            "standard",
            "status",
            "tag",
            "taxonomy"
        };
        internal const string DefaultFilterName = "keyword";

        internal static IQueryable<Question> ApplyFilter(IQueryable<Question> query, string keyword)
        {
            var filterItems = SimpleFilterHelper.Parse(keyword, AllowedFilterNames, DefaultFilterName);
            if (filterItems.IsEmpty())
                return query;

            foreach (var item in filterItems)
            {
                switch (item.Name.ToLower())
                {
                    case "asset":
                        query = int.TryParse(item.Value, out var assetNum)
                            ? query.Where(x => x.Asset == assetNum)
                            : query.Where(x => false);
                        break;
                    case "code":
                        query = query
                            .Where(x => x.Classification != null && x.Classification.Code != null && x.Classification.Code.Contains(item.Value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "difficulty":
                        query = int.TryParse(item.Value, out var diffNum)
                            ? query.Where(x => x.Classification != null && x.Classification.Difficulty == diffNum)
                            : query.Where(x => false);
                        break;
                    case "flag":
                        query = query.Where(x => x.Flag.ToString().Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "keyword":
                        query = query.Where(x =>
                            x.Content != null && x.Content.Title != null && x.Content.Title.Default != null && x.Content.Title.Default.Contains(item.Value, StringComparison.OrdinalIgnoreCase)
                            || x.Classification != null && x.Classification.LikeItemGroup != null && x.Classification.LikeItemGroup.Contains(item.Value, StringComparison.OrdinalIgnoreCase)
                            || x.Classification != null && x.Classification.Code != null && x.Classification.Code.Contains(item.Value, StringComparison.OrdinalIgnoreCase)
                            || x.Options != null && x.Options.Any(y => y.Content != null && y.Content.Title != null && y.Content.Title.Default != null && y.Content.Title.Default.Contains(item.Value, StringComparison.OrdinalIgnoreCase))
                        );
                        break;
                    case "lig":
                        query = string.Equals(item.Value, "*")
                            ? query.Where(x => x.Classification != null && x.Classification.LikeItemGroup != null)
                            : query.Where(x => x.Classification != null && x.Classification.LikeItemGroup != null && x.Classification.LikeItemGroup.Contains(item.Value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "reference":
                        query = string.Equals(item.Value, "*")
                            ? query.Where(x => x.Classification != null && x.Classification.Reference != null)
                            : query.Where(x => x.Classification != null && x.Classification.Reference != null && x.Classification.Reference.Contains(item.Value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "tag":
                        query = query
                            .Where(x => x.Classification != null && x.Classification.Tag != null && x.Classification.Tag.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "taxonomy":
                        query = int.TryParse(item.Value, out var taxNum)
                            ? query.Where(x => x.Classification != null && x.Classification.Taxonomy == taxNum)
                            : query.Where(x => false);
                        break;
                    case "status":
                        query = query.Where(x => string.Equals(x.Condition, item.Value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "standard":
                        var standards = GetQuestionStandards(item.Value);
                        query = query.Where(x => standards.Contains(x.Identifier));
                        break;
                }
            }

            return query;
        }

        private static List<Guid> GetQuestionStandards(string code)
        {
            return ServiceLocator
                .BankSearch
                .GetQuestions(new QBankQuestionFilter { StandardCode = code, OrganizationIdentifier = Organization.Identifier })
                .Select(x => x.QuestionIdentifier)
                .ToList();
        }

        #endregion
    }
}