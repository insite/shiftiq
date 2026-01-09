using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using AnswerQuestionOutput = InSite.UI.Portal.Assessments.Attempts.Controls.AnswerQuestionOutput;
using RubricCriteriaList = InSite.UI.Admin.Assessments.Attempts.Controls.RubricCriteriaList;
using ViewForm = InSite.Admin.Assessments.Attempts.Forms.View;

namespace InSite.UI.Admin.Assessments.Attempts.Forms
{
    public partial class Grade : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region Classes

        private class QuestionInfo
        {
            public Guid QuestionIdentifier { get; set; }
            public decimal? QuestionPoints { get; set; }
            public QuestionItemType QuestionType { get; set; }
            public string QuestionText { get; set; }
            public string AnswerText { get; set; }
            public Guid? AnswerFileIdentifier { get; set; }
            public string ExemplarText { get; set; }
            public Dictionary<Guid, decimal> RubricRatingPoints { get; set; }
        }

        [Serializable]
        [JsonObject(MemberSerialization.OptIn)]
        private class ScreenData
        {
            public int RubricIndex
            {
                get => _rubricIndex;
                set
                {
                    _rubricIndex = value;

                    if (_maxRubricIndex < _rubricIndex)
                        _maxRubricIndex = _rubricIndex;
                }
            }

            public int MaxRubricIndex => _maxRubricIndex;

            [JsonProperty(PropertyName = "rubrics")]
            public List<GradeRubricItem> Rubrics { get; set; }

            [JsonProperty(PropertyName = "index")]
            private int _rubricIndex;

            [JsonProperty(PropertyName = "maxIndex")]
            private int _maxRubricIndex = -1;
        }

        #endregion

        #region Constants

        private const string SearchUrl = "/ui/admin/assessments/attempts/reports/search";

        private static readonly string ComposedEssayType = QuestionItemType.ComposedEssay.GetName();
        private static readonly string ComposedVoiceType = QuestionItemType.ComposedVoice.GetName();

        #endregion

        #region Properties

        private Guid AttemptId => Guid.TryParse(Request["attempt"], out var value) ? value : Guid.Empty;

        private ScreenData Data
        {
            get => (ScreenData)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RubricValidator.ServerValidate += RubricValidator_ServerValidate;

            PrevButton.Click += PrevButton_Click;
            NextButton.Click += NextButton_Click;
            SubmitButton.Click += SubmitButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Data.RubricIndex < Data.Rubrics.Count)
                ShowAnswerSumPoints();
        }

        private void LoadData()
        {
            var questions = LoadComposedQuestions();

            Data = new ScreenData
            {
                RubricIndex = 0,
                Rubrics = ReadRubric(questions)
            };

            TryLoadScreenData();

            if (Data.RubricIndex == Data.Rubrics.Count)
                ShowSummary(false);
            else
                ShowRubric(Data.RubricIndex, false);
        }

        private QuestionInfo[] LoadComposedQuestions()
        {
            var attempt = ServiceLocator.AttemptSearch.GetAttempt(AttemptId, x => x.LearnerPerson);
            if (attempt == null || attempt.AttemptSubmitted == null || HasOldRubrics(attempt.AttemptStarted))
                HttpResponseHelper.Redirect(SearchUrl);

            var groupIds = TGroupPermissionSearch.SelectGroupFromActionPermission(PermissionNames.Design_Grading_Assessors);
            var exists = MembershipSearch.Exists(groupIds, CurrentSessionState.Identity.User.UserIdentifier);
            var isOnlyAssessor = !Identity.IsGranted(Route.ToolkitName, PermissionOperation.Read);

            if (exists && isOnlyAssessor && (attempt.GradingAssessorUserIdentifier != Identity.User.UserIdentifier))
                HttpResponseHelper.Redirect(GetParentUrl($"attempt={AttemptId}&panel=rubrics"));

            if (attempt.AttemptGraded.HasValue && !Identity.IsGranted(ActionName.Admin_Assessments_Attempts_Grade_Regrade))
                HttpResponseHelper.Redirect(SearchUrl);

            var allQuestions = ServiceLocator.AttemptSearch.GetAttemptQuestions(AttemptId);
            var composedQuestions = allQuestions
                .Where(x => x.QuestionType == ComposedEssayType || x.QuestionType == ComposedVoiceType)
                .ToList();

            if (composedQuestions.Count == 0)
                HttpResponseHelper.Redirect(SearchUrl);

            SetPageTitle(attempt, exists, isOnlyAssessor);

            var form = ServiceLocator.BankSearch.GetFormData(attempt.FormIdentifier);
            var bank = form.Specification.Bank;
            var keepInitRatingPoints = Organization.Toolkits.Assessments.RubricReGradeKeepInitialScores;

            return composedQuestions
                .Select(x => new QuestionInfo
                {
                    QuestionIdentifier = x.QuestionIdentifier,
                    QuestionPoints = x.QuestionPoints,
                    QuestionType = x.QuestionType.ToEnum<QuestionItemType>(),
                    QuestionText = x.QuestionText,
                    AnswerText = x.AnswerText,
                    AnswerFileIdentifier = x.AnswerFileIdentifier,
                    ExemplarText = bank.FindQuestion(x.QuestionIdentifier)?.Content.Exemplar.Get(Identity.Language),
                    RubricRatingPoints = keepInitRatingPoints && x.RubricRatingPoints.IsNotEmpty()
                        ? JsonConvert.DeserializeObject<Dictionary<Guid, decimal>>(x.RubricRatingPoints)
                        : null
                })
                .ToArray();
        }

        private void SetPageTitle(QAttempt attempt, bool exists, bool isOnlyAssessor)
        {
            var learnerName = !Organization.Toolkits.Assessments.ShowPersonNameToGradingAssessor && exists && isOnlyAssessor
                ? attempt.LearnerPerson.PersonCode
                : ViewForm.ShouldHideLearnerName()
                    ? "Learner"
                    : $"{attempt.LearnerPerson.UserFullName} <span class='form-text'>{attempt.LearnerPerson.PersonCode}</span>";

            PageHelper.AutoBindHeader(this, null, learnerName);
        }

        private static List<GradeRubricItem> ReadRubric(IEnumerable<QuestionInfo> questions)
        {
            // When QuestionPoints == -1 this means that the attempt was started without rubric assigned to the question

            var lang = Identity.Language;
            var result = new List<GradeRubricItem>();
            var rubrics = ServiceLocator.RubricSearch.GetQuestionRubrics(questions.Where(x => x.QuestionPoints != -1).Select(x => x.QuestionIdentifier));
            var contents = lang != Language.Default && rubrics.Count > 0
                ? ServiceLocator.ContentSearch.GetBlocks(rubrics.Values.Select(x => x.RubricIdentifier).Distinct(), lang, new[] { ContentLabel.Title })
                : null;

            foreach (var q in questions)
            {
                var item = new GradeRubricItem
                {
                    QuestionIdentifier = q.QuestionIdentifier,
                    QuestionType = q.QuestionType,
                    QuestionText = q.QuestionText,
                    AnswerText = q.AnswerText,
                    ExemplarText = q.ExemplarText,
                    AnswerFileIdentifier = q.AnswerFileIdentifier,
                    InitRatingPoints = q.RubricRatingPoints
                };

                var rubric = rubrics.GetOrDefault(q.QuestionIdentifier);
                if (rubric != null)
                {
                    var rubricContent = contents?.GetOrDefault(rubric.RubricIdentifier);
                    item.RubricIdentifier = rubric.RubricIdentifier;
                    item.RubricTitle = (rubricContent?.Title.GetText(lang)).IfNullOrEmpty(rubric.RubricTitle);
                    item.RubricPoints = rubric.RubricPoints;
                }

                result.Add(item);
            }

            return result;
        }

        #endregion

        #region Event handlers

        private void RubricValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var criteria = RubricCriteriaList.SaveCriteria();

            args.IsValid = criteria.Count > 0 && criteria.All(x => x.Ratings.Any(y => y.SelectedPoints.HasValue));
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            var url = GetParentUrl($"attempt={AttemptId}&panel=rubrics");

            DeleteScreenData();

            HttpResponseHelper.Redirect(url);
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (Data.RubricIndex < Data.Rubrics.Count)
                Data.Rubrics[Data.RubricIndex].Criteria = RubricCriteriaList.SaveCriteria();

            ShowRubric(Data.RubricIndex - 1, true);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Data.Rubrics[Data.RubricIndex].Criteria = RubricCriteriaList.SaveCriteria();

            if (Data.RubricIndex == Data.Rubrics.Count - 1)
                ShowSummary(true);
            else
                ShowRubric(Data.RubricIndex + 1, true);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            var commands = new List<Command>();

            foreach (var r in Data.Rubrics)
            {
                var ratings = r.Criteria.Select(x =>
                    x.Ratings
                        .Where(y => y.SelectedPoints.HasValue)
                        .Select(y => new
                        {
                            y.RatingId,
                            Points = y.SelectedPoints.Value
                        })
                        .First()
                    )
                    .ToDictionary(x => x.RatingId, x => x.Points);

                var command = new ScoreComposedQuestion(AttemptId, r.QuestionIdentifier, ratings);

                commands.Add(command);
            }

            commands.Add(new GradeAttempt(AttemptId));

            ServiceLocator.SendCommands(commands);

            var url = GetParentUrl($"attempt={AttemptId}&panel=rubrics&status=graded");

            DeleteScreenData();

            HttpResponseHelper.Redirect(url);
        }

        #endregion

        #region Methods (UI)

        private void ShowRubric(int index, bool saveData)
        {
            Data.RubricIndex = index;

            RubricPanel.Visible = true;
            Summary.Visible = false;

            var r = Data.Rubrics[index];
            var isEssay = r.QuestionType == QuestionItemType.ComposedEssay;
            var isVoice = r.QuestionType == QuestionItemType.ComposedVoice;

            QuestionText.InnerText = r.QuestionText;

            ExemplarCard.Visible = r.ExemplarText.IsNotEmpty();
            ExemplarText.InnerHtml = Markdown.ToHtml(r.ExemplarText);

            AnswerText.Visible = isEssay;
            if (isEssay)
                AnswerText.Text = Markdown.ToHtml(r.AnswerText);

            AnswerAudio.Visible = isVoice;
            if (isVoice)
            {
                var audioUrl = AnswerQuestionOutput.GetFileUrl(r.AnswerFileIdentifier);
                AnswerAudio.AudioURL = audioUrl;
                AnswerAudio.Visible = audioUrl != null;
            }

            RubricTitle.Text = r.RubricTitle;
            CriteriaRubricPoints.Text = $"{r.RubricPoints:n2}";

            RubricCriteriaList.LoadData(r.RubricIdentifier, r.Criteria, "updateAnswerSumPoints", r.InitRatingPoints);

            PrevButton.Visible = index > 0;
            NextButton.Visible = index <= Data.Rubrics.Count;
            SubmitButton.Visible = false;
            RubricPosition.Visible = true;
            RubricPosition.InnerText = Translate("Rubric {0:n0} out of {1:n0}").Format(index + 1, Data.Rubrics.Count);

            if (saveData)
                SaveScreenData();
        }

        private void SetSum(decimal answerPoints)
        {
            var rubricPoints = Data.RubricIndex == Data.Rubrics.Count
                ? Data.Rubrics.Sum(x => x.RubricPoints)
                : Data.Rubrics[Data.RubricIndex].RubricPoints;

            SumPanel.InnerHtml = Translate("Total Points: {0} out of {1:n2}")
                .Format($"<span id='{ClientID}_AnswerSumPoints'>{answerPoints:n2}</span>", rubricPoints);
        }

        private void ShowAnswerSumPoints()
        {
            var criteria = RubricCriteriaList.SaveCriteria();

            var sum = criteria.Count > 0
                ? criteria.Sum(x => x.Ratings.Sum(y => y.SelectedPoints ?? 0))
                : 0;

            SetSum(sum);
        }

        private void ShowSummary(bool saveData)
        {
            Data.RubricIndex = Data.Rubrics.Count;

            RubricPanel.Visible = false;

            Summary.Visible = true;
            Summary.LoadData(Data.Rubrics);

            var answerSum = Data.Rubrics.Sum(x => x.SelectedPoints);
            SetSum(answerSum);

            PrevButton.Visible = true;
            NextButton.Visible = false;
            SubmitButton.Visible = true;
            RubricPosition.Visible = false;

            if (saveData)
                SaveScreenData();
        }

        #endregion

        #region Methods (screen data backup)

        private const string ReportType = "AttemptGrade";

        private void SaveScreenData()
        {
            var entity = LoadScreenDataReport();
            var isNew = entity == null;

            if (isNew)
            {
                entity = new TReport
                {
                    ReportIdentifier = UniqueIdentifier.Create(),
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    UserIdentifier = User.UserIdentifier,
                    ReportType = ReportType,
                    ReportTitle = "Grade Attempt",
                    ReportDescription = AttemptId.ToString(),
                    Created = DateTimeOffset.Now,
                    CreatedBy = User.UserIdentifier,
                };
            }

            entity.ReportData = JsonConvert.SerializeObject(Data);
            entity.Modified = DateTimeOffset.Now;
            entity.ModifiedBy = User.UserIdentifier;

            if (isNew)
                TReportStore.Insert(entity);
            else
                TReportStore.Update(entity);
        }

        private void TryLoadScreenData()
        {
            var entity = LoadScreenDataReport();
            var data = entity == null ? null : JsonConvert.DeserializeObject<ScreenData>(entity.ReportData);
            if (data == null || Data.Rubrics.Count != data.Rubrics.Count)
                return;

            List<(GradeRubricItem Current, GradeRubricItem Loaded)> zipped =
                Data.Rubrics.Zip(data.Rubrics, (a, b) => (a, b)).ToList();
            var isValid = zipped.All(x => x.Current.QuestionIdentifier == x.Loaded.QuestionIdentifier
                                       && x.Current.RubricIdentifier == x.Loaded.RubricIdentifier);
            if (!isValid)
                return;

            foreach (var r in zipped)
            {
                if (r.Loaded.Criteria.IsEmpty())
                    break;

                var criteria = RubricCriteriaList.GetCriteria(r.Current.RubricIdentifier);

                isValid = criteria.Count == r.Loaded.Criteria.Count
                    && criteria.Zip(r.Loaded.Criteria, (a, b) => a.Load(b)).All(x => x);

                if (isValid)
                    r.Current.Criteria = criteria;
                else
                    break;
            }

            if (!isValid)
            {
                foreach (var r in Data.Rubrics)
                    r.Criteria = null;
            }
            else
                Data.RubricIndex = data.MaxRubricIndex;
        }

        private void DeleteScreenData()
        {
            var entity = LoadScreenDataReport();
            if (entity != null)
                TReportStore.Delete(entity.ReportIdentifier);
        }

        private TReport LoadScreenDataReport()
        {
            return TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType
                  && x.ReportDescription == AttemptId.ToString()
            );
        }

        #endregion

        #region Methods (navigation back)

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/view")
                ? $"attempt={AttemptId}&panel=rubrics"
                : null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        #endregion

        #region Methods (helpers)

        private static readonly DateTimeOffset NewRubricsSince = new DateTimeOffset(2023, 8, 12, 0, 0, 0, TimeSpan.Zero);

        public static bool HasOldRubrics(DateTimeOffset? attemptStarted)
        {
            return attemptStarted.HasValue && attemptStarted < NewRubricsSince;
        }

        #endregion
    }
}