using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Forms.Models;
using InSite.Application.Banks.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

using Section = InSite.Domain.Banks.Section;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class WorkshopQuestionRepeater : BaseUserControl
    {
        #region Classes

        public class DataItem
        {
            public int? FormSequence { get; }
            public int CandidateCommentsCount { get; }
            public string CandidateCommentUrl { get; set; }
            public string CandidateCommentsVisibility { get; set; }
            public Guid? FieldId { get; }
            public Guid QuestionId { get; }

            public Question Entity { get; }

            public DataItem(Domain.Banks.Field field)
            {
                FormSequence = field.FormSequence;
                FieldId = field.Identifier;
                QuestionId = field.Question.Identifier;
                CandidateCommentsCount = field.Question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Candidate).Count();
                var bankID = field.Question.Set.Bank.Identifier;
                CandidateCommentUrl = $"/ui/admin/assessments/bankscomments/search?bank={bankID}&question={QuestionId}&role=Candidate&showAuthor=0&panel=results";
                if (CandidateCommentsCount == 0)
                    CandidateCommentsVisibility = "display:none;";

                Entity = field.Question;
            }

            public DataItem(Question question)
            {
                QuestionId = question.Identifier;
                CandidateCommentsCount = question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Candidate).Count();
                if (CandidateCommentsCount == 0)
                    CandidateCommentsVisibility = "display:none;";
                var bankID = question.Set.Bank.Identifier;
                CandidateCommentUrl = $"/ui/admin/assessments/bankscomments/search?bank={bankID}&question={QuestionId}&role=Candidate&showAuthor=0&panel=results";
                Entity = question;
            }
        }

        #endregion

        #region Events

        public event EventHandler NeedRefresh;

        private void OnNeedRefresh() => NeedRefresh?.Invoke(this, EventArgs.Empty);

        public event CommandEventHandler QuestionReplace;

        private void OnQuestionReplace(Guid questionId, string command)
        {
            QuestionReplace?.Invoke(this, new CommandEventArgs(command, questionId));
        }

        #endregion

        #region Fields

        private QuestionTable _currentQuestionTable;
        private DataItem _currentDataItem;
        protected ReturnUrl _returnUrl;

        #endregion

        #region Properties

        protected BankState Bank { get; set; }

        protected string StatusList => "'" + string.Join("','", QuestionConditionComboBox.Conditions) + "'";
        protected string FlagList => "'" + string.Join("','", FlagComboBox.Flags) + "'";

        private List<TCollectionItem> _taxonomies;
        private List<TCollectionItem> Taxonomies => _taxonomies ?? (_taxonomies = GetCollectionItems(CollectionName.Assessments_Questions_Classification_Taxonomy));

        protected string TaxonomyList
        {
            get
            {
                var result = new StringBuilder();
                result.Append("{value:'',text:''}");

                foreach (var taxonomy in Taxonomies)
                {
                    result.Append(',');
                    result.Append($"{{value:{taxonomy.ItemSequence},text:'{taxonomy.ItemSequence}. {taxonomy.ItemName}'}}");
                }

                return result.ToString();
            }
        }

        protected string CompetencyLists
        {
            get
            {
                var result = new StringBuilder();

                if (Bank != null)
                {
                    foreach (var set in Bank.Sets)
                    {
                        var key = set.Identifier.ToString().Replace("-", "");
                        result.Append($"$(\"a.editable-input.competency_{key}\").editable({{source: [{GetCompetencyList(set.Identifier)}]}}); ");
                        result.AppendLine();
                    }
                }

                return result.ToString();
            }
        }

        protected Guid EntityID { get; set; }

        protected BankEntityType EntityType { get; set; }

        protected Guid BankID
        {
            get => (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        protected Guid SetID
        {
            get => (Guid)ViewState[nameof(SetID)];
            set => ViewState[nameof(SetID)] = value;
        }

        protected Guid SectionID
        {
            get => (Guid)ViewState[nameof(SectionID)];
            set => ViewState[nameof(SectionID)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCreated += Repeater_ItemCreated;
            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.ItemCommand += Repeater_ItemCommand;

            GridStyle.ContentKey = GetType().FullName;
        }

        #endregion

        #region Event handlers

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var replaceButton = (DropDownButton)e.Item.FindControl("ReplaceButton");
            replaceButton.Click += ReplaceButton_Click;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            _currentDataItem = (DataItem)e.Item.DataItem;

            var question = _currentDataItem.Entity;
            var isTable = question.Layout.Type == OptionLayoutType.Table;

            _currentQuestionTable = isTable
                ? QuestionTable.Build(question.Layout.Columns, question.Options.Select(x => x.Content.Title.Default))
                : null;

            var multiView = (MultiView)e.Item.FindControl("QuestionItemsMultiView");

            if (question.Type == QuestionItemType.SingleCorrect)
            {
                multiView.SetActiveView((View)e.Item.FindControl("SingleCorrectItemsView"));

                var optionRepeater = (Repeater)e.Item.FindControl("SingleCorrectOptionRepeater");
                optionRepeater.DataSource = question.Options.Select(x => new
                {
                    x.Points,
                    x.Letter,
                    Title = x.Content.Title?.Default,
                    Option = x
                });
                optionRepeater.DataBind();
            }
            else if (question.Type == QuestionItemType.TrueOrFalse)
            {
                multiView.SetActiveView((View)e.Item.FindControl("TrueOrFalseItemsView"));

                var optionRepeater = (Repeater)e.Item.FindControl("TrueOrFalseOptionRepeater");
                optionRepeater.DataSource = question.Options.Select(x => new
                {
                    x.Points,
                    x.Letter,
                    Title = x.Content.Title?.Default,
                    Option = x
                });
                optionRepeater.DataBind();
            }
            else if (question.Type == QuestionItemType.MultipleCorrect)
            {
                multiView.SetActiveView((View)e.Item.FindControl("MultipleCorrectItemsView"));

                var optionRepeater = (Repeater)e.Item.FindControl("MultipleCorrectOptionRepeater");
                optionRepeater.DataSource = question.Options.Select(x => new
                {
                    x.Letter,
                    Title = x.Content.Title?.Default,
                    x.Points,
                    x.IsTrue,
                    Option = x
                });
                optionRepeater.DataBind();
            }
            else if (question.Type.IsComposed())
            {
                multiView.SetActiveView((View)e.Item.FindControl("ComposedRubricItemsView"));

                var optionRepeater = (Repeater)e.Item.FindControl("ComposedRubricOptionRepeater");
                optionRepeater.DataSource = question.Options.Select(x => new
                {
                    x.Letter,
                    Title = x.Content.Title?.Default,
                    x.Points,
                    Option = x
                });
                optionRepeater.DataBind();
            }
            else if (question.Type == QuestionItemType.BooleanTable)
            {
                multiView.SetActiveView((View)e.Item.FindControl("BooleanTableItemsView"));

                var optionRepeater = (Repeater)e.Item.FindControl("BooleanTableOptionRepeater");
                optionRepeater.DataSource = question.Options.Select(x => new
                {
                    x.Letter,
                    Title = x.Content.Title?.Default,
                    x.Points,
                    x.IsTrue,
                    Option = x
                });
                optionRepeater.DataBind();
            }
            else if (question.Type == QuestionItemType.Matching)
            {
                multiView.SetActiveView((View)e.Item.FindControl("MatchesItemsView"));

                var pairsRepeater = (Repeater)e.Item.FindControl("MatchingPairsRepeater");
                pairsRepeater.Visible = (question.Matches?.Pairs).IsNotEmpty();
                pairsRepeater.DataSource = question.Matches.Pairs.Select(x => new { Left = x.Left.Title.Default, Right = x.Right.Title.Default, x.Points });
                pairsRepeater.DataBind();

                var distractors = question.Matches.Distractors
                    .Where(x => !string.IsNullOrEmpty(x.Title.Default))
                    .Select(x => new { Value = x.Title.Default })
                    .ToList();

                var distractorsRepeater = (Repeater)e.Item.FindControl("MatchingDistractorsRepeater");
                distractorsRepeater.Visible = distractors.Count > 0;
                distractorsRepeater.DataSource = distractors;
                distractorsRepeater.DataBind();
            }

            var isForm = EntityType == BankEntityType.Form;
            var isFormPublished = false;
            if (isForm)
            {
                var form = question.Set.Bank.FindForm(EntityID);
                isFormPublished = form.Publication.Status == PublicationStatus.Published;
            }

            var commentRepeater = (WorkshopQuestionCommentRepeater)e.Item.FindControl("CommentRepeater");
            commentRepeater.BankID = Bank.Identifier;
            if (commentRepeater.Visible = question.Comments.Count > 0)
            {
                if (isForm)
                    commentRepeater.LoadFormData(EntityID, question, _returnUrl);
                else
                    commentRepeater.LoadSpecificationData(question, _returnUrl);
            }

            var replaceButton = (DropDownButton)e.Item.FindControl("ReplaceButton");
            replaceButton.Visible = isForm && !isFormPublished;
            replaceButton.Items["NewVersion"].Visible = question.PublicationStatus == PublicationStatus.Published && question.IsLastVersion();
            replaceButton.Items["RollbackQuestion"].Visible = question.AssetVersion > 0 && !question.IsFirstVersion();

            var copyField = e.Item.FindControl("CopyField");
            copyField.Visible = !isForm;
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Copy")
            {
                var sourceId = Guid.Parse(((ITextControl)e.Item.FindControl("QuestionIdentifier")).Text);
                var destinationId = UniqueIdentifier.Create();

                ServiceLocator.SendCommand(new DuplicateQuestion(BankID, sourceId, destinationId, Sequence.Increment(Organization.Identifier, SequenceType.Asset)));

                OnNeedRefresh();

                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(WorkshopQuestionRepeater),
                    "scroll_to_question",
                    $"workshopQuestionRepeater.scrollToQuestion('{destinationId}');",
                    true);
            }
        }

        private void ReplaceButton_Click(object sender, CommandEventArgs e)
        {
            var item = (RepeaterItem)((System.Web.UI.Control)sender).NamingContainer;
            var questionId = Guid.Parse(((ITextControl)item.FindControl("QuestionIdentifier")).Text);

            OnQuestionReplace(questionId, e.CommandName);
        }

        #endregion

        #region Methods

        public void Hide()
        {
            MainPanel.Visible = false;
        }

        public void LoadData(Specification specification, IEnumerable<DataItem> data, ReturnUrl returnUrl)
        {
            MainPanel.Visible = true;

            Bank = specification.Bank;
            BankID = Bank.Identifier;

            EntityID = specification.Identifier;
            EntityType = BankEntityType.Specification;

            _returnUrl = returnUrl;

            Repeater.DataSource = data;
            Repeater.DataBind();
        }

        public void LoadData(BankState bank, Section section, IEnumerable<DataItem> data, ReturnUrl returnUrl)
        {
            MainPanel.Visible = true;

            Bank = bank;
            BankID = Bank.Identifier;

            EntityID = section.Form.Identifier;
            EntityType = BankEntityType.Form;

            SectionID = section.Identifier;
            SetID = section.Criterion.Sets[0].Identifier;

            _returnUrl = returnUrl;

            Repeater.DataSource = data;
            Repeater.DataBind();
        }

        #endregion

        #region Methods (question repeater helpers)

        protected string GetFlagHtml(FlagType flag)
        {
            return flag.ToIconHtml();
        }

        protected string GetOptionRepeaterTableHead(string prefix, string postfix)
        {
            if (_currentQuestionTable == null)
                return string.Empty;

            var html = new StringBuilder();

            html.Append("<thead><tr>").Append(prefix);

            var questionID = _currentDataItem.QuestionId;

            var colNumber = 0;
            foreach (var col in _currentQuestionTable.GetHeader())
                RenderOptionRepeaterCell(BankID, questionID, 0, colNumber++, html, "th", col);

            html.Append(postfix).Append("</tr></thead>");

            return html.ToString();
        }

        protected string GetOptionRepeaterTableHeadTitleCols()
        {
            if (_currentQuestionTable == null)
                return "<th></th>";

            var html = new StringBuilder();

            var questionID = _currentDataItem.QuestionId;

            var colNumber = 0;
            foreach (var col in _currentQuestionTable.GetHeader())
                RenderOptionRepeaterCell(BankID, questionID, 0, colNumber++, html, "th", col);

            return html.ToString();
        }

        protected string GetOptionRepeaterTitle(RepeaterItem item)
        {
            var option = (Option)Eval("Option");

            if (_currentQuestionTable == null)
            {
                var text = GetOptionText(option);
                return $"<td class='option-title'>{text}</td>";
            }

            var html = new StringBuilder();

            for (var i = 0; i < _currentQuestionTable.ColumnsCount; i++)
            {
                var cell = _currentQuestionTable.GetBody(item.ItemIndex, i);

                RenderOptionRepeaterCell(option.Question.Set.Bank.Identifier, option.Question.Identifier, option.Number, i, html, "td", cell);
            }

            return html.ToString();
        }

        private static void RenderOptionRepeaterCell(Guid bankID, Guid questionID, int optionNumber, int colNumber, StringBuilder html, string tagName, QuestionTable.CellData cell)
        {
            html.Append("<")
                .Append(tagName)
                .Append(" style='text-align:")
                .Append(cell.Alignment.ToString().ToLower())
                .Append(";'");

            if (!string.IsNullOrEmpty(cell.CssClass))
                html.Append(" class='").Append(cell.CssClass).Append("'");

            html.Append(">");

            var dataName = tagName == "td" ? ElementUpdater.ElementTypes.OptionTitleColumn : ElementUpdater.ElementTypes.OptionHeaderColumn;
            var dataPk = $"{bankID}:{questionID}:{optionNumber}:{colNumber}";

            html.Append("<a href='#' class='editable-input'")
                .Append(" data-type='text'")
                .AppendFormat(" data-name='{0}'", dataName)
                .AppendFormat(" data-pk='{0}'", dataPk)
                .AppendFormat(" data-value='{0}'", HttpUtility.HtmlEncode(cell.Text ?? string.Empty))
                .AppendFormat(">{0}</a>", Markdown.ToHtml(cell.Text));

            html.Append("</")
                .Append(tagName)
                .Append(">");
        }

        private static string GetOptionText(Option option)
        {
            var optionTitle = option.Content.Title != null ? option.Content.Title.Default : null;

            return $@"
<a href='#' class='editable-input option'
    data-name='{ElementUpdater.ElementTypes.OptionTitle}'
    data-type='textarea'
    data-pk='{option.Question.Set.Bank.Identifier}:{option.Question.Identifier}:{option.Number}'
    data-value='{HttpUtility.HtmlEncode(optionTitle ?? string.Empty)}'
>{Markdown.ToHtml(optionTitle)}</a>
";
        }

        protected string GetReviewLabel(object status)
        {
            var label = string.Empty;

            if (status != null)
            {
                switch ((string)status)
                {
                    case "Requires Change":
                        label = "<div class='badge bg-danger'><i class='fas fa-flag'></i> Requires Change</div>";
                        break;
                    case "Modified":
                        label = "<div class='badge bg-info'>Modified</div>";
                        break;
                    case "Removed":
                        label = "<div class='badge bg-custom-default'>Removed</div>";
                        break;
                    default:
                        break;
                }
            }

            return label;
        }

        protected static string DisplayRationale(object item)
        {
            var q = ((DataItem)item).Entity;

            if (q.Content == null)
                return null;

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(q.Content.Rationale?.Default))
                sb.Append($"<div class='alert alert-info' role='alert'><i class='fas fa-info-square'></i> <strong>Feedback to all candidates:</strong> {Markdown.ToHtml(q.Content.Rationale.Default)}</div>");

            if (!string.IsNullOrEmpty(q.Content.RationaleOnCorrectAnswer?.Default))
                sb.Append($"<div class='alert alert-success' role='alert'><i class='fas fa-check-square'></i> <strong>Feedback on correct answers:</strong> {Markdown.ToHtml(q.Content.RationaleOnCorrectAnswer.Default)}</div>");

            if (!string.IsNullOrEmpty(q.Content.RationaleOnIncorrectAnswer?.Default))
                sb.Append($"<div class='alert alert-danger' role='alert'><i class='fas fa-times-square'></i> <strong>Feedback on incorrect answers:</strong> {Markdown.ToHtml(q.Content.RationaleOnIncorrectAnswer.Default)}</div>");

            return sb.ToString();
        }

        private string GetRedirectUrl(string url, params object[] args) =>
            _returnUrl.GetRedirectUrl(string.Format(url, args), "question");

        protected string GetChangeUrl(DataItem item) =>
            GetRedirectUrl(
                EntityType == BankEntityType.Form
                    ? "/ui/admin/assessments/questions/change?bank={0}&form={1}&question={2}"
                    : "/ui/admin/assessments/questions/change?bank={0}&question={2}",
                BankID, EntityID, item.QuestionId
            );

        protected string GetAnalysisUrl(DataItem item) =>
            GetRedirectUrl("/ui/admin/assessments/questions/analysis?bank={0}&question={1}", BankID, item.QuestionId);

        #endregion

        #region Methods (option repeater helpers)

        private Dictionary<Guid, SnippetBuilder.StandardModel[]> _competencies;

        private SnippetBuilder.StandardModel[] GetCompetencies(Guid setID)
        {
            if (_competencies == null)
                _competencies = new Dictionary<Guid, SnippetBuilder.StandardModel[]>();

            SnippetBuilder.StandardModel[] assets;

            if (!_competencies.TryGetValue(setID, out assets))
            {
                var set = Bank.FindSet(setID);

                assets = StandardSearch.Bind(
                    x => new SnippetBuilder.StandardModel
                    {
                        Identifier = x.StandardIdentifier,
                        Label = x.StandardLabel,
                        Type = x.StandardType,
                        Name = x.ContentName,
                        Number = x.AssetNumber,
                        Title = x.ContentTitle,
                        Code = x.Code,
                        ParentCode = x.Parent.Code
                    },
                    x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Parent.StandardIdentifier == set.Standard,
                    null);

                _competencies.Add(setID, assets);
            }

            return assets;
        }

        protected string GetCompetencyTitle(Guid setID, Guid standard)
        {
            if (standard == Guid.Empty)
                return null;

            var assets = GetCompetencies(setID);
            var asset = assets.FirstOrDefault(x => x.Identifier == standard);

            return asset != null ? GetCompetencyTitle(asset) : "";
        }

        protected string GetCompetencyTitle(Guid setID, Guid standard, Guid questionId, PublicationStatus status)
        {
            if (standard == Guid.Empty)
                return null;

            var assets = GetCompetencies(setID);
            var asset = assets.FirstOrDefault(x => x.Identifier == standard);

            var setClassId = (setID.ToString().Replace("-", "")).ToString();

            var competencyAsset = (asset != null ? GetCompetencyTitle(asset) : "");

            if (status == PublicationStatus.Published)
                return competencyAsset;

            return string.Format(@"<a href='#' class='editable-input competency competency_{0}' 
                                data-name='Standard'
                                data-type='select'
                                data-pk='{1}:{2}' 
                                data-value='{3}'>
                                {4}
                            </a> ", setClassId, Bank.Identifier, questionId, standard, competencyAsset);

        }

        private string GetCompetencyTitle(SnippetBuilder.StandardModel asset)
        {
            return SnippetBuilder.GetHtml(asset, true, false);
        }

        private string GetCompetencyList(Guid setID)
        {
            var assets = GetCompetencies(setID);

            var items = assets.Select(x => new
            {
                ID = x.Identifier,
                Text = StringHelper.StripHtml(SnippetBuilder.GetHtml(x, true, false, false, false))
            }).OrderBy(x => x.Text);

            var result = new StringBuilder();
            result.Append("{value:'',text:''}");

            foreach (var item in items)
            {
                result.Append(',');
                result.Append($"{{value:'{item.ID}',text:{HttpUtility.JavaScriptStringEncode(item.Text, true)}}}");
            }

            return result.ToString();
        }

        protected string GetSingleCorrectOptionIcon(decimal value) => value > 0
            ? $"<i class='text-{FlagType.Green.GetContextualClass()} far fa-check-circle'></i>"
            : $"<i class='text-{FlagType.Red.GetContextualClass()} far fa-times-circle text-danger'></i>";

        protected string GetMultipleCorrectOptionIcon(bool? isTrue) =>
            !isTrue.HasValue
                ? "<i class='far fa-exclamation-triangle text-warning' title='Not Configured'></i>"
                : isTrue.Value
                    ? "<i class='far fa-check-square'></i>"
                    : "<i class='far fa-square'></i>";

        protected string GetBooleanTableOptionIcon(bool? isTrue, bool answer) =>
            !isTrue.HasValue
                ? "<i class='far fa-exclamation-triangle text-warning' title='Not Configured'></i>"
                : isTrue.Value == answer
                    ? "<i class='far fa-dot-circle'></i>"
                    : "<i class='far fa-circle'></i>";

        protected string GetOptionPoints(decimal points) => $"{points:n2} points";

        protected string GetOptionPointsEditable()
        {
            var option = (Option)Eval("Option");

            return $@"
<a href='#' class='editable-input points'
    data-name='{ElementUpdater.ElementTypes.OptionPoints}'
    data-type='text'
    data-pk='{option.Question.Set.Bank.Identifier}:{option.Question.Identifier}:{option.Number}'
>{option.Points:n2}</a> points
";
        }

        #endregion

        public static List<TCollectionItem> GetCollectionItems(string collection)
        {
            var organization = CurrentSessionState.Identity.Organization;

            var items = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = organization.Identifier,
                CollectionName = collection
            });

            if (items.Count == 0 && organization.ParentOrganizationIdentifier.HasValue)
                items = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = organization.ParentOrganizationIdentifier.Value,
                    CollectionName = collection
                });

            return items.ToList();
        }

        public static Dictionary<int, TCollectionItem> GetCollectionItemsAsDictionary(string collection)
        {
            return GetCollectionItems(collection).ToDictionary(x => x.ItemSequence, x => x);
        }
    }
}