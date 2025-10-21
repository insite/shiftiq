using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Contents.Read;
using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Admin.Assessments.Attempts.Controls;
using InSite.UI.Admin.Assessments.Attempts.Forms;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Assessments.Attempts.Utilities;
using InSite.Web.Helpers;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    public partial class View : AdminBasePage
    {
        #region Classes

        private class RubricDataItem
        {
            public int Key { get; set; }
            public string Objective { get; set; }
            public Guid? CompetencyStandardIdentifier { get; set; }
            public string CompetencyTitle { get; set; }
            public decimal? Points { get; set; }
            public int? Mark { get; set; }
        }

        private class SortedQuestions
        {
            public List<QAttemptQuestion> SingleCorrect { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> TrueOrFalse { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> MultipleCorrect { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> BooleanTable { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> Matching { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> Likert { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> Hotspot { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> ComposedEssay { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> ComposedVoice { get; } = new List<QAttemptQuestion>();
            public List<QAttemptQuestion> Ordering { get; } = new List<QAttemptQuestion>();
            public Dictionary<Guid, List<QAttemptQuestion>> SubQuestions { get; } = new Dictionary<Guid, List<QAttemptQuestion>>();

            private SortedQuestions()
            {

            }

            public static SortedQuestions Create(IEnumerable<QAttemptQuestion> questions)
            {
                var result = new SortedQuestions();

                foreach (var q in questions)
                {
                    if (q.ParentQuestionIdentifier.HasValue)
                    {
                        result.SubQuestions.GetOrAdd(q.ParentQuestionIdentifier.Value, () => new List<QAttemptQuestion>()).Add(q);
                        continue;
                    }

                    var type = q.QuestionType.ToEnum<QuestionItemType>();

                    if (type == QuestionItemType.SingleCorrect)
                        result.SingleCorrect.Add(q);
                    else if (type == QuestionItemType.TrueOrFalse)
                        result.TrueOrFalse.Add(q);
                    else if (type == QuestionItemType.MultipleCorrect)
                        result.MultipleCorrect.Add(q);
                    else if (type == QuestionItemType.BooleanTable)
                        result.BooleanTable.Add(q);
                    else if (type == QuestionItemType.Matching)
                        result.Matching.Add(q);
                    else if (type == QuestionItemType.Likert)
                        result.Likert.Add(q);
                    else if (type.IsHotspot())
                        result.Hotspot.Add(q);
                    else if (type == QuestionItemType.ComposedEssay)
                        result.ComposedEssay.Add(q);
                    else if (type == QuestionItemType.ComposedVoice)
                        result.ComposedVoice.Add(q);
                    else if (type == QuestionItemType.Ordering)
                        result.Ordering.Add(q);
                }

                return result;
            }
        }

        #endregion

        #region Properties

        private Guid? AttemptIdentifier => Guid.TryParse(Request["attempt"], out Guid value) ? value : (Guid?)null;

        private string DefaultPanel => Request.QueryString["panel"];

        private string DefaultStatus => Request.QueryString["status"];

        protected DateTimeOffset? AttemptGraded
        {
            get => (DateTimeOffset?)ViewState[nameof(AttemptGraded)];
            set => ViewState[nameof(AttemptGraded)] = value;
        }

        protected bool IsAttemptImported
        {
            get => (bool)(ViewState[nameof(IsAttemptImported)] ?? false);
            set => ViewState[nameof(IsAttemptImported)] = value;
        }

        protected bool IsFormThirdPartyEnabled
        {
            get => (bool)(ViewState[nameof(IsFormThirdPartyEnabled)] ?? false);
            set => ViewState[nameof(IsFormThirdPartyEnabled)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<Guid, QComment> _comments;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnalyzeCommand.Click += AnalyzeCommand_Click;
            CompleteCommand.Click += CompleteCommand_Click;

            SingleCorrectQuestionRepeater.ItemDataBound += QuestionOptionRepeater_ItemDataBound;
            MultipleCorrectQuestionRepeater.ItemDataBound += QuestionOptionRepeater_ItemDataBound;
            BooleanTableQuestionRepeater.ItemDataBound += QuestionOptionRepeater_ItemDataBound;
            TrueOrFalseQuestionRepeater.ItemDataBound += QuestionOptionRepeater_ItemDataBound;
            MatchingQuestionRepeater.ItemDataBound += MatchingQuestionRepeater_ItemDataBound;
            LikertQuestionRepeater.ItemDataBound += LikertQuestionRepeater_ItemDataBound;
            HotspotQuestionRepeater.ItemDataBound += HotspotQuestionRepeater_ItemDataBound;
            OrderingQuestionRepeater.ItemDataBound += OrderingQuestionRepeater_ItemDataBound;

            CompetencyAreaRepeater.ItemDataBound += CompetencyAreaRepeater_ItemDataBound;
        }

        private void AnalyzeCommand_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new AnalyzeAttempt(AttemptIdentifier.Value, false));
            ViewerStatus.AddMessage(AlertType.Success, "Analysis of this assessment attempt completed successfully.");
        }

        private void CompleteCommand_Click(object sender, EventArgs e)
        {
            if (AttemptIdentifier.HasValue)
                SubmitAttempt(AttemptIdentifier.Value);

            HttpResponseHelper.Redirect(Request.RawUrl, true);
        }

        public static bool SubmitAttempt(Guid attemptId)
        {
            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (attempt == null || attempt.AttemptSubmitted.HasValue)
                return false;

            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(attemptId);
            var hasComposedQuestions = GetComposedQuestions(questions).Any();

            ServiceLocator.SendCommand(new SubmitAttempt(attemptId, HttpContext.Current.Request.UserAgent, !hasComposedQuestions));

            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open(true);

            if (string.Equals(DefaultPanel, "rubrics", StringComparison.OrdinalIgnoreCase))
                RubricPanel.IsSelected = true;

            if (string.Equals(DefaultStatus, "graded", StringComparison.OrdinalIgnoreCase))
                ViewerStatus.AddMessage(AlertType.Success, "The attempt was successfully graded");
        }

        #endregion

        #region Event handlers

        private void CompetencyAreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var folder = (CompetencyArea)e.Item.DataItem;
            var repeater = (Repeater)e.Item.FindControl("CompetencyItemRepeater");
            repeater.ItemDataBound += Repeater_ItemDataBound;
            repeater.DataSource = folder.Items;
            repeater.DataBind();

            var score = (Literal)e.Item.FindControl("CompetencyAreaScore");
            score.Text = folder.ScoreHtml;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (CompetencyItem)e.Item.DataItem;
            var repeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            repeater.DataSource = item.Questions;
            repeater.DataBind();

            var score = (Literal)e.Item.FindControl("CompetencyItemScore");
            score.Text = item.ScoreHtml;

            var points = (Literal)e.Item.FindControl("CompetencyItemPoints");
            points.Text = item.PointsHtml;
        }

        private void QuestionOptionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var question = (QAttemptQuestion)e.Item.DataItem;
            var options = ServiceLocator.AttemptSearch.GetAttemptOptions(AttemptIdentifier.Value, question.QuestionIdentifier);

            var comment = (AssessorComment)e.Item.FindControl("AssessorComment");
            if (comment != null)
                comment.Comment = _comments.GetOrDefault(question.QuestionIdentifier);

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = options;
            optionRepeater.DataBind();
        }

        private void MatchingQuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var question = (QAttemptQuestion)e.Item.DataItem;
            var matches = ServiceLocator.AttemptSearch.GetAttemptMatches(AttemptIdentifier.Value, question.QuestionIdentifier);

            var comment = (AssessorComment)e.Item.FindControl("AssessorComment");
            if (comment != null)
                comment.Comment = _comments.GetOrDefault(question.QuestionIdentifier);

            var matchRepeater = (Repeater)e.Item.FindControl("MatchRepeater");
            matchRepeater.DataSource = matches;
            matchRepeater.DataBind();
        }

        private void LikertQuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var question = (QAttemptQuestion)DataBinder.Eval(e.Item.DataItem, "Question");
            var subQuestions = (IEnumerable<QAttemptQuestion>)DataBinder.Eval(e.Item.DataItem, "SubQuestions");
            var idFilter = subQuestions.Select(x => x.QuestionIdentifier).ToArray();
            var options = ServiceLocator.AttemptSearch.GetAttemptOptions(AttemptIdentifier.Value, idFilter)
                .GroupBy(x => x.QuestionIdentifier).ToDictionary(x => x.Key, x => x.ToArray());

            var comment = (AssessorComment)e.Item.FindControl("AssessorCommentLikert");
            if (comment != null)
                comment.Comment = _comments.GetOrDefault(question.QuestionIdentifier);

            var columnRepeater = (Repeater)e.Item.FindControl("LikertColumnRepeater");
            columnRepeater.DataSource = options.Values.FirstOrDefault();
            columnRepeater.DataBind();

            var rowRepeater = (Repeater)e.Item.FindControl("LikertRowRepeater");
            rowRepeater.ItemDataBound += LikertRowRepeater_ItemDataBound;
            rowRepeater.DataSource = subQuestions.Select((x, i) => new
            {
                Question = x,
                Options = options.GetOrDefault(x.QuestionIdentifier, () => new QAttemptOption[0])
            });
            rowRepeater.DataBind();
        }

        private void LikertRowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var options = (QAttemptOption[])DataBinder.Eval(e.Item.DataItem, "Options");

            var repeater = (Repeater)e.Item.FindControl("LikertOptionRepeater");
            repeater.DataSource = options;
            repeater.DataBind();
        }

        private void HotspotQuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var question = (QAttemptQuestion)e.Item.DataItem;
            var options = ServiceLocator.AttemptSearch.GetAttemptOptions(AttemptIdentifier.Value, question.QuestionIdentifier);
            var pins = ServiceLocator.AttemptSearch.GetAttemptPins(AttemptIdentifier.Value, question.QuestionIdentifier, null);

            var comment = (AssessorComment)e.Item.FindControl("AssessorComment");
            if (comment != null)
                comment.Comment = _comments.GetOrDefault(question.QuestionIdentifier);

            var image = (HtmlControl)e.Item.FindControl("HotspotImage");
            image.Attributes["data-img"] = question.HotspotImage;
            image.Attributes["data-shapes"] = JsonConvert.SerializeObject(options.Select(x => x.OptionShape));
            image.Attributes["data-pins"] = JsonConvert.SerializeObject(pins.Select(x => $"{((x.OptionPoints ?? 0) > 0 ? 1 : 0)},{x.PinX},{x.PinY}").ToArray());

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = options;
            optionRepeater.DataBind();
        }

        private void OrderingQuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var question = (QAttemptQuestion)e.Item.DataItem;
            var options = ServiceLocator.AttemptSearch.GetAttemptOptions(AttemptIdentifier.Value, question.QuestionIdentifier);
            var solution = ServiceLocator.AttemptSearch.GetAttemptMatchedSolution(question.AttemptIdentifier, question.QuestionIdentifier);
            var isAnswerCorrect = solution != null && solution.SolutionPoints > 0;

            var comment = (AssessorComment)e.Item.FindControl("AssessorComment");
            if (comment != null)
                comment.Comment = _comments.GetOrDefault(question.QuestionIdentifier);

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = options.OrderBy(x => x.OptionAnswerSequence).ThenBy(x => x.OptionSequence).Select(x => new
            {
                Text = x.OptionText,
                Sequence = x.OptionSequence,
                IsCorrect = isAnswerCorrect
            });
            optionRepeater.DataBind();
        }

        #endregion

        #region Data binding

        private void Open(bool bindRepeaters)
        {
            if (!AttemptIdentifier.HasValue)
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(AttemptIdentifier.Value, x => x.Form, x => x.LearnerPerson);
            if (attempt == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);

            var bank = ServiceLocator.BankSearch.GetBankState(attempt.Form.BankIdentifier);
            if (bank == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);

            var form = bank.FindForm(attempt.FormIdentifier);
            if (form == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);

            IsFormThirdPartyEnabled = form.ThirdPartyAssessmentIsEnabled;

            var hideLearnerName = ShouldHideLearnerName();

            PageHelper.AutoBindHeader(this, null, hideLearnerName ? "Learner" : attempt.LearnerPerson.UserFullName);

            PersonColumn.Visible = !hideLearnerName;

            BindPersonDetails(attempt, hideLearnerName);

            FormDetails.BindForm(form, attempt.Form.BankIdentifier, bank.IsAdvanced);

            AttemptGraded = attempt.AttemptGraded;
            IsAttemptImported = attempt.AttemptImported.HasValue;

            AttemptStartedOutput.Text = attempt.AttemptStarted.Format(User.TimeZone, nullValue: "N/A");
            AttemptSubmittedOutput.Text = attempt.AttemptSubmitted.Format(User.TimeZone, nullValue: "N/A");
            AttemptGradedOutput.Text = attempt.AttemptGraded.Format(User.TimeZone, nullValue: "N/A");

            CompleteCommand.Visible = !attempt.AttemptSubmitted.HasValue;
            AttemptTimeTaken.Text = attempt.AttemptDuration.HasValue
                ? ((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second)
                : "N/A";

            SebVersion.Text = AttemptHelper.GetSebVersion(attempt.UserAgent);
            SebField.Visible = SebVersion.Text.IsNotEmpty();

            AttemptPoints.Text = attempt.AttemptPoints.HasValue ? $"{attempt.AttemptPoints:n2} / {attempt.FormPoints:n2}" : "N/A";
            AttemptScore.Text = attempt.AttemptScore.HasValue ? $"{attempt.AttemptScore:p0}" : "N/A";
            AttemptId.Text = attempt.AttemptIdentifier.ToString();
            DeleteAttemptLink.NavigateUrl = $"/admin/assessments/attempts/delete?attempt={attempt.AttemptIdentifier}";

            if (attempt.AttemptGraded.HasValue)
            {
                AttemptIsPassing.Text = attempt.AttemptIsPassing
                    ? "<span class='badge bg-success'>Pass</span>"
                    : "<span class='badge bg-danger'>Fail</span>";
            }
            else if (attempt.AttemptSubmitted.HasValue)
                AttemptIsPassing.Text = "<span class='badge bg-warning'>Pending</span>";
            else
                AttemptIsPassing.Text = "N/A";

            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(AttemptIdentifier.Value);

            if (bindRepeaters)
                BindRepeaters(bank, attempt, questions);

            var list = questions.Where(x => x.CompetencyAreaIdentifier.HasValue).ToList();

            var model = new CompetencyReport(list);

            CompetenciesNavItem.Visible = model.Folders.Count > 0;

            CompetencyAreaRepeater.DataSource = model.Folders;
            CompetencyAreaRepeater.DataBind();

            var returnUrl = $"/ui/admin/assessments/attempts/view?attempt={AttemptIdentifier.Value}";
            ViewHistoryLink.NavigateUrl = $"/ui/admin/logs/aggregates/outline?aggregate={AttemptIdentifier.Value}&returnURL=" + HttpUtility.UrlEncode(returnUrl);
        }

        private void BindPersonDetails(QAttempt attempt, bool hideLearnerName)
        {
            if (!hideLearnerName)
            {
                var learner = ServiceLocator.PersonSearch.GetPerson(attempt.LearnerUserIdentifier, attempt.OrganizationIdentifier, x => x.User);
                PersonDetail.BindPerson(learner, User.TimeZone);
            }

            var hasGradingAssessor = attempt.GradingAssessorUserIdentifier.HasValue
                && attempt.GradingAssessorUserIdentifier.Value != attempt.AssessorUserIdentifier;

            if (hasGradingAssessor)
            {
                GradingAssessorDetails.Visible = true;
                GradingAssessorDetails.Bind(attempt.GradingAssessorUserIdentifier.Value);
            }

            if (attempt.AssessorUserIdentifier != attempt.LearnerUserIdentifier)
            {
                if (!hasGradingAssessor)
                    ExamAssessorDetails.Title = "Assessor";

                ExamAssessorDetails.Visible = true;
                ExamAssessorDetails.Bind(attempt.AssessorUserIdentifier);
            }
        }

        private void BindRepeaters(BankState bank, QAttempt attempt, List<QAttemptQuestion> questions)
        {
            SingleCorrectTab.Title = QuestionItemType.SingleCorrect.GetDescription();
            TrueOrFalseTab.Title = QuestionItemType.TrueOrFalse.GetDescription();
            MultipleCorrectTab.Title = QuestionItemType.MultipleCorrect.GetDescription();
            BooleanTableTab.Title = QuestionItemType.BooleanTable.GetDescription();
            MatchingTab.Title = QuestionItemType.Matching.GetDescription();
            LikertTab.Title = QuestionItemType.Likert.GetDescription();
            HotspotTab.Title = "Hotspot";
            OrderingTab.Title = QuestionItemType.Ordering.GetDescription();

            var sorted = SortedQuestions.Create(questions);

            if (IsFormThirdPartyEnabled || User.Identifier == attempt.AssessorUserIdentifier)
                _comments = ServiceLocator.AttemptSearch.GetQAttemptComments(attempt.AttemptIdentifier, attempt.AssessorUserIdentifier)
                    .Where(x => x.AssessmentQuestionIdentifier.HasValue)
                    .GroupBy(x => x.AssessmentQuestionIdentifier.Value)
                    .ToDictionary(x => x.Key, x => x.FirstOrDefault());
            else
                _comments = new Dictionary<Guid, QComment>();

            SingleCorrectTab.Visible = sorted.SingleCorrect.Count > 0;
            SingleCorrectQuestionRepeater.DataSource = sorted.SingleCorrect;
            SingleCorrectQuestionRepeater.DataBind();

            TrueOrFalseTab.Visible = sorted.TrueOrFalse.Count > 0;
            TrueOrFalseQuestionRepeater.DataSource = sorted.TrueOrFalse;
            TrueOrFalseQuestionRepeater.DataBind();

            MultipleCorrectTab.Visible = sorted.MultipleCorrect.Count > 0;
            MultipleCorrectQuestionRepeater.DataSource = sorted.MultipleCorrect;
            MultipleCorrectQuestionRepeater.DataBind();

            BooleanTableTab.Visible = sorted.BooleanTable.Count > 0;
            BooleanTableQuestionRepeater.DataSource = sorted.BooleanTable;
            BooleanTableQuestionRepeater.DataBind();

            MatchingTab.Visible = sorted.Matching.Count > 0;
            MatchingQuestionRepeater.DataSource = sorted.Matching;
            MatchingQuestionRepeater.DataBind();

            LikertTab.Visible = sorted.Likert.Count > 0;
            LikertQuestionRepeater.DataSource = sorted.Likert.Select(x => new
            {
                Question = x,
                SubQuestions = sorted.SubQuestions.GetOrDefault(x.QuestionIdentifier, () => new List<QAttemptQuestion>())
            });
            LikertQuestionRepeater.DataBind();

            HotspotTab.Visible = sorted.Hotspot.Count > 0;
            HotspotQuestionRepeater.DataSource = sorted.Hotspot;
            HotspotQuestionRepeater.DataBind();

            ComposedEssayTab.Visible = sorted.ComposedEssay.Count > 0;
            ComposedEssayViewer.BindQuestions(bank, sorted.ComposedEssay);

            ComposedVoiceTab.Visible = sorted.ComposedVoice.Count > 0;
            ComposedVoiceViewer.BindQuestions(bank, sorted.ComposedVoice);

            OrderingTab.Visible = sorted.Ordering.Count > 0;
            OrderingQuestionRepeater.DataSource = sorted.Ordering;
            OrderingQuestionRepeater.DataBind();

            BindRubrics(attempt, questions);
        }

        private void BindRubrics(QAttempt attempt, List<QAttemptQuestion> questions)
        {
            var hasOldRubrics = Grade.HasOldRubrics(attempt.AttemptStarted);
            var composedQuestions = GetComposedQuestions(questions);

            RubricPanel.Visible = composedQuestions.Length > 0 && !hasOldRubrics;
            Rubrics.LoadData(composedQuestions);

            var gradeUrl = $"/ui/admin/assessments/attempts/grade?attempt={AttemptIdentifier}";

            GradeButton.Visible = attempt.AttemptSubmitted.HasValue && attempt.AttemptGraded == null;
            GradeButton.NavigateUrl = gradeUrl;

            RegradeButton.Visible = attempt.AttemptSubmitted.HasValue
                && attempt.AttemptGraded.HasValue
                && Identity.IsGranted(ActionName.Admin_Assessments_Attempts_Grade_Regrade);

            RegradeButton.NavigateUrl = gradeUrl;

            if (composedQuestions.Length > 0 && hasOldRubrics && attempt.AttemptGraded == null)
                ViewerStatus.AddMessage(AlertType.Warning, "This attempt was created before new rubrics were introduced and therefore it cannot be graded");
        }

        private static QAttemptQuestion[] GetComposedQuestions(List<QAttemptQuestion> questions)
        {
            var composedName1 = QuestionItemType.ComposedEssay.GetName();
            var composedName2 = QuestionItemType.ComposedVoice.GetName();
            return questions.Where(x => x.QuestionType == composedName1 || x.QuestionType == composedName2).ToArray();
        }

        public static bool ShouldHideLearnerName()
        {
            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.Identifier
                  && x.UserIdentifier == User.Identifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == "ui/admin/assessments/attempts/reports/search"
            );

            var settings = ReportRequest.Deserialize(report?.ReportData)?.GetSearch<QAttemptFilter>();

            return settings?.Filter != null && settings.Filter.HideLearnerName;

        }

        #endregion
    }
}