using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Attempts;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class ProgressModel
    {
        #region Classes

        public class ProgressInfo : ProgressState
        {
            public string Grade { get; set; }
        }

        #endregion

        #region Properties

        public Guid User { get; private set; }
        public Course Course { get; private set; }

        public string CourseUrl { get; private set; }

        public Dictionary<Guid, LearningResult> Results { get; private set; }
        public Dictionary<Guid, AnswerResult> Answers { get; private set; }
        public Dictionary<Guid, AttemptResult> Attempts { get; private set; }
        public Dictionary<Guid, GradeItemResult> GradeItems { get; private set; }

        public Guid _currentPageActivity;

        public int? _currentPageNumber;
        public int CurrentPage
        {
            get
            {
                if (!_currentPageNumber.HasValue)
                {
                    var activities = GetVisibleActivities();
                    var index = activities.FindIndex(x => x.Identifier == _currentPageActivity);

                    if (index >= 0)
                    {
                        _currentActivity = activities[index];
                        _currentPageNumber = index + 1;
                    }
                    else
                    {
                        _currentPageNumber = 0;
                    }
                }

                return _currentPageNumber.Value;
            }
        }

        public bool CurrentPageIsValid
        {
            get
            {
                var activities = GetVisibleActivities();
                return CurrentPage <= activities.Count;
            }
        }

        public bool IsCourseHidden { get; private set; }

        public int? _nextPageNumber = int.MaxValue;
        public int? NextPage
        {
            get
            {
                if (_nextPageNumber == int.MaxValue)
                {
                    var activities = GetVisibleActivities();
                    for (var i = CurrentPage + 1; i <= activities.Count && _nextPageNumber == int.MaxValue; i++)
                    {
                        var next = activities[i - 1];
                        var activityResult = Results[next.Identifier];
                        var moduleResult = Results[next.Module.Identifier];

                        if (!activityResult.IsLocked && !activityResult.IsHidden && !moduleResult.IsLocked && !moduleResult.IsHidden)
                            _nextPageNumber = i;
                    }

                    if (_nextPageNumber == int.MaxValue)
                        _nextPageNumber = null;
                }

                return _nextPageNumber;
            }
        }

        public int? _previousPageNumber = int.MaxValue;
        public int? PreviousPage
        {
            get
            {
                if (_previousPageNumber == int.MaxValue)
                {
                    var activities = GetVisibleActivities();
                    for (var i = CurrentPage - 1; i > 0 && _previousPageNumber == int.MaxValue; i--)
                    {
                        var prev = activities[i - 1];
                        var activityResult = Results[prev.Identifier];
                        var moduleResult = Results[prev.Module.Identifier];

                        if (!activityResult.IsLocked && !activityResult.IsHidden && !moduleResult.IsLocked && !moduleResult.IsHidden)
                            _previousPageNumber = i;
                    }

                    if (_previousPageNumber == int.MaxValue)
                        _previousPageNumber = null;
                }

                return _previousPageNumber;
            }
        }

        private Activity _currentActivity;
        public Activity CurrentActivity
        {
            get
            {
                if (_currentActivity == null)
                {
                    var activities = GetVisibleActivities();
                    _currentActivity = CurrentPage > 0 && activities.Count >= CurrentPage
                        ? activities[CurrentPage - 1]
                        : new Activity();
                }

                return _currentActivity;
            }
        }

        private LearningResult _currentActivityResult;
        public LearningResult CurrentActivityResult
        {
            get
            {
                if (_currentActivityResult == null && !Results.TryGetValue(CurrentActivity.Identifier, out _currentActivityResult))
                    _currentActivityResult = new LearningResult();

                return _currentActivityResult;
            }
        }

        private LearningResult _currentModuleResult;
        public LearningResult CurrentModuleResult
        {
            get
            {
                if (_currentModuleResult == null && (CurrentActivity.Module == null || !Results.TryGetValue(CurrentActivity.Module.Identifier, out _currentModuleResult)))
                    _currentModuleResult = new LearningResult();

                return _currentModuleResult;
            }
        }

        public string NextPageUrl => BuildUrl(CourseUrl, $"activity={NextPage}");

        public string PreviousPageUrl => BuildUrl(CourseUrl, $"activity={PreviousPage}");

        private string BuildUrl(string path, string parameters)
        {
            return parameters.IsNotEmpty()
                ? path.Contains("?") ? path + "&" + parameters : path + "?" + parameters
                : path;
        }

        #endregion

        #region Construction

        private ProgressModel(Course course, Guid user, string courseUrl)
        {
            Course = course;
            User = user;
            CourseUrl = courseUrl;
            Results = new Dictionary<Guid, LearningResult>();
            Answers = new Dictionary<Guid, AnswerResult>();
            Attempts = new Dictionary<Guid, AttemptResult>();
            GradeItems = new Dictionary<Guid, GradeItemResult>();
        }

        public ProgressModel(Course course, Guid user, string courseUrl, int page)
            : this(course, user, courseUrl)
        {
            _currentPageNumber = page;
        }

        public ProgressModel(Course course, Guid user, string courseUrl, Guid page)
            : this(course, user, courseUrl)
        {
            _currentPageActivity = page;
        }

        #endregion

        #region Initialization

        public void ApplyProgression(Guid activity, ProgressInfo progress)
        {
            SetProgress(Course.GetActivity(activity), Results[activity], new[] { progress });
        }

        public void ApplyProgression(ProgressInfo[] progresses, IEnumerable<AnswerState> answers, Guid[] userGroups)
        {
            foreach (var progress in progresses)
            {
                if (!GradeItems.TryGetValue(progress.Item, out var score))
                {
                    var item = Course.Gradebook.Items.Single(i => i.Identifier == progress.Item);
                    if (item != null)
                        GradeItems.Add(progress.Item, new GradeItemResult
                        {
                            GradeItem = item.Identifier,
                            Score = progress.Percent,
                            IsPass = progress.Percent >= item.PassPercent
                        });
                }
            }

            foreach (var answer in answers)
            {
                if (!Answers.TryGetValue(answer.Question, out var result))
                    Answers.Add(answer.Question, result = new AnswerResult
                    {
                        AnswerIsCorrect = false,
                        Form = answer.Form,
                        AttemptIsPass = answer.AttemptIsPass,
                        AttemptScore = answer.AttemptScore
                    });

                result.AnswerIsCorrect = result.AnswerIsCorrect || answer.QuestionPoints <= answer.AnswerPoints;
            }

            foreach (var answer in Answers.Values)
            {
                if (!Attempts.ContainsKey(answer.Form))
                    Attempts.Add(answer.Form, new AttemptResult
                    {
                        Form = answer.Form,
                        IsPass = answer.AttemptIsPass,
                        Score = answer.AttemptScore
                    });
            }

            IsCourseHidden = Course.PrivacyGroups.Count > 0
                && Course.PrivacyGroups.Select(x => x.Identifier).Intersect(userGroups).Count() == 0;

            var page = 0;
            var activityResults = new List<LearningResult>();

            for (var i = 0; i < Course.Units.Count; i++)
            {
                var unit = Course.Units[i];

                var unitResult = new LearningResult();

                if (unit.PrerequisiteDeterminer == DeterminerType.All)
                    unitResult.Status = AreAllPrerequisitesSatisfied(unit.Prerequisites) ? "Unlocked" : "Locked";
                else
                    unitResult.Status = IsAnyPrerequisiteSatisfied(unit.Prerequisites) ? "Unlocked" : "Locked";

                unitResult.IsHidden = unit.PrivacyGroups.Count > 0
                    && unit.PrivacyGroups.Select(x => x.Identifier).Intersect(userGroups).Count() == 0;

                Results.Add(unit.Identifier, unitResult);

                for (var j = 0; j < unit.Modules.Count; j++)
                {
                    var module = unit.Modules[j];
                    var moduleResult = new LearningResult();
                    bool isLocked;

                    if (unitResult.IsLocked || module.IsAdaptive && !module.HasPrerequisites)
                    {
                        isLocked = true;
                    }
                    else
                    {
                        isLocked = module.PrerequisiteDeterminer == DeterminerType.All
                            ? !AreAllPrerequisitesSatisfied(module.Prerequisites)
                            : !IsAnyPrerequisiteSatisfied(module.Prerequisites);
                    }

                    moduleResult.Status = isLocked ? "Locked" : "Unlocked";

                    moduleResult.IsHidden = unitResult.IsHidden || (module.PrivacyGroups.Count > 0
                        && module.PrivacyGroups.Select(x => x.Identifier).Intersect(userGroups).Count() == 0);

                    Results.Add(module.Identifier, moduleResult);

                    for (var k = 0; k < module.Activities.Count; k++)
                    {
                        var activity = module.Activities[k];

                        var activityResult = new LearningResult();

                        activityResult.IsHidden = moduleResult.IsHidden || (activity.PrivacyGroups.Count > 0
                            && activity.PrivacyGroups.Select(x => x.Identifier).Intersect(userGroups).Count() == 0);

                        activityResult.Navigation.ActivityIdentifier = activity.Identifier;
                        activityResult.Navigation.PageNumber = ++page;

                        if (CourseUrl.IsEmpty())
                            throw new ArgumentNullException("CourseUrl");

                        activityResult.Navigation.Url = BuildUrl(CourseUrl, $"activity={activityResult.Navigation.PageNumber}");

                        Results.Add(activity.Identifier, activityResult);
                        activityResults.Add(activityResult);

                        SetProgress(activity, activityResult, progresses);
                    }
                }
            }

            GetVisibleActivities();
            GetVisibleModules();
            GetVisibleUnits();

            foreach (var unit in Course.Units)
            {
                foreach (var module in unit.Modules)
                {
                    var countVisibleActivitiesInModule = module.Activities.Count(a => _visibleActivities.Any(v => v.Identifier == a.Identifier));
                    if (countVisibleActivitiesInModule == 0)
                    {
                        Results[module.Identifier].IsHidden = true;
                        _visibleModules.Remove(module);
                    }
                }
            }

            var currentPage = CurrentPage;
            var n = 1;
            foreach (var result in activityResults)
            {
                if (_visibleActivities.Any(a => a.Identifier == result.Navigation.ActivityIdentifier))
                {
                    result.Navigation.PageNumber = n++;

                    if (CourseUrl.IsEmpty())
                        throw new ArgumentNullException("CourseUrl");

                    result.Navigation.Url = BuildUrl(CourseUrl, $"activity={result.Navigation.PageNumber}");
                }
                else
                {

                }
                result.Navigation.IsSelected = result.Navigation.PageNumber == currentPage;
            }
        }

        private void SetProgress(Activity activity, LearningResult activityResult, ProgressInfo[] progresses)
        {
            bool isLocked;

            if (activity.IsAdaptive && !activity.HasPrerequisites)
            {
                isLocked = true;
            }
            else
            {
                isLocked = activity.PrerequisiteDeterminer == DeterminerType.All
                    ? !AreAllPrerequisitesSatisfied(activity.Prerequisites)
                    : !IsAnyPrerequisiteSatisfied(activity.Prerequisites);
            }

            activityResult.Status = isLocked ? "Locked" : "Unlocked";

            if (activity.GradeItem == null)
                return;

            var progress = progresses.FirstOrDefault(x => x.User == User && x.Item == activity.GradeItem.Identifier);
            if (progress == null)
                return;

            if (activity.GradeItem.Format == GradeItemFormat.Boolean)
            {
                activityResult.Status = progress.Status;
                activityResult.Grade = progress.Grade;
            }
            else if (progress.Text != null)
            {
                activityResult.Status = progress.Text;
            }
            else if (activity.GradeItem.PassPercent.HasValue && progress.Percent.HasValue)
            {
                activityResult.Status = "Completed";
                activityResult.Grade = progress.Percent >= activity.GradeItem.PassPercent ? "Pass" : "Fail";
                activityResult.Score = progress.Percent;
            }

            activityResult.Completed = progress.Graded;
        }

        private bool AreAllPrerequisitesSatisfied(IEnumerable<Prerequisite> prerequisites)
        {
            foreach (var pre in prerequisites)
            {
                if (pre.Type == PrerequisiteType.ActivityCompleted)
                {
                    if (!TestResult(r => r.IsCompleted))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.AssessmentFailed)
                {
                    if (!TestAttempt(a => a.IsFail))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.AssessmentPassed)
                {
                    if (!TestAttempt(a => a.IsPass))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.AssessmentScored)
                {
                    var from = pre.Condition.ScoreFrom;
                    var thru = pre.Condition.ScoreThru;

                    if (!TestAttempt(a => Number.IsInRange(Math.Round(a.Score ?? 0, 2, MidpointRounding.AwayFromZero) * 100, from, thru)))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.QuestionAnsweredCorrectly)
                {
                    if (!TestAnswer(a => a.AnswerIsCorrect))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.QuestionAnsweredIncorrectly)
                {
                    if (!TestAnswer(a => !a.AnswerIsCorrect))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.GradeItemPassed)
                {
                    if (TestGradeItem(r => r.IsFail))
                        return false;
                }
                else if (pre.Type == PrerequisiteType.GradeItemFailed)
                {
                    if (TestGradeItem(r => !r.IsFail))
                        return false;
                }
                else
                    throw ApplicationError.Create("Unexpected prerequisite type: " + pre.Type.GetName());

                bool TestResult(Func<LearningResult, bool> test)
                {
                    return Results.TryGetValue(pre.Condition.Identifier, out var result) && test(result);
                }

                bool TestAttempt(Func<AttemptResult, bool> test)
                {
                    return Attempts.TryGetValue(pre.Condition.Identifier, out var attempt) && test(attempt);
                }

                bool TestAnswer(Func<AnswerResult, bool> test)
                {
                    return Answers.TryGetValue(pre.Condition.Identifier, out var answer) && test(answer);
                }

                bool TestGradeItem(Func<GradeItemResult, bool> test)
                    => GradeItems.TryGetValue(pre.Condition.Identifier, out var item) && test(item);
            }

            return true;
        }

        private bool IsAnyPrerequisiteSatisfied(IEnumerable<Prerequisite> prerequisites)
        {
            if (prerequisites.Count() == 0)
                return true;

            foreach (var pre in prerequisites)
            {
                if (pre.Type == PrerequisiteType.ActivityCompleted)
                {
                    if (TestResult(r => r.IsCompleted))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.AssessmentFailed)
                {
                    if (TestAttempt(a => a.Score.HasValue && a.IsFail))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.AssessmentPassed)
                {
                    if (TestAttempt(a => a.Score.HasValue && a.IsPass))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.AssessmentScored)
                {
                    if (TestAttempt(a => Number.IsInRange(Math.Round(a.Score ?? 0, 2, MidpointRounding.AwayFromZero) * 100, pre.Condition.ScoreFrom, pre.Condition.ScoreThru)))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.QuestionAnsweredCorrectly)
                {
                    if (TestAnswer(a => a.AnswerIsCorrect))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.QuestionAnsweredIncorrectly)
                {
                    if (TestAnswer(a => !a.AnswerIsCorrect))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.GradeItemPassed)
                {
                    if (TestGradeItem(r => r.IsPass))
                        return true;
                }
                else if (pre.Type == PrerequisiteType.GradeItemFailed)
                {
                    if (TestGradeItem(r => r.IsFail))
                        return true;
                }
                else
                    throw ApplicationError.Create("Unexpected prerequisite type: " + pre.Type.GetName());

                bool TestResult(Func<LearningResult, bool> test)
                    => Results.TryGetValue(pre.Condition.Identifier, out var result) && test(result);

                bool TestAttempt(Func<AttemptResult, bool> test)
                    => Attempts.TryGetValue(pre.Condition.Identifier, out var attempt) && test(attempt);

                bool TestAnswer(Func<AnswerResult, bool> test)
                    => Answers.TryGetValue(pre.Condition.Identifier, out var answer) && test(answer);

                bool TestGradeItem(Func<GradeItemResult, bool> test)
                    => GradeItems.TryGetValue(pre.Condition.Identifier, out var item) && test(item);
            }

            return false;
        }

        #endregion

        #region Methods

        public Guid[] GetAssessmentForms()
        {
            var forms = new List<Guid>();

            foreach (var activity in Course.GetAllSupportedActivities())
            {
                if (activity.Assessment != null && !forms.Contains(activity.Assessment.Identifier))
                    forms.Add(activity.Assessment.Identifier);
            }

            return forms.ToArray();
        }

        private List<Activity> _visibleActivities;
        public List<Activity> GetVisibleActivities()
        {
            if (_visibleActivities == null)
            {
                _visibleActivities = new List<Activity>();

                foreach (var activity in Course.GetAllSupportedActivities())
                {
                    if (!Results.TryGetValue(activity.Identifier, out var result) || result.IsHidden)
                        continue;

                    if (!activity.IsAdaptive)
                        _visibleActivities.Add(activity);
                    else if (result.IsUnlocked)
                        _visibleActivities.Add(activity);
                }
            }

            return _visibleActivities;
        }

        private List<Module> _visibleModules;
        public List<Module> GetVisibleModules()
        {
            if (_visibleModules == null)
            {
                _visibleModules = new List<Module>();

                foreach (var unit in Course.Units)
                {
                    foreach (var module in unit.Modules)
                    {
                        if (!Results.TryGetValue(module.Identifier, out var result)
                            || result.IsHidden
                            || result.IsLocked
                            || GetVisibleActivities().Count(x => !Results[x.Identifier].IsHidden) == 0
                            || module.IsAdaptive && result.IsLocked
                            )
                        {
                            continue;
                        }

                        _visibleModules.Add(module);
                    }
                }
            }

            return _visibleModules;
        }

        private List<Unit> _visibleUnits;
        public List<Unit> GetVisibleUnits()
        {
            if (_visibleUnits == null)
            {
                _visibleUnits = new List<Unit>();

                foreach (var unit in Course.Units)
                {
                    if (!Results.TryGetValue(unit.Identifier, out var result) || result.IsHidden)
                        continue;

                    if (GetVisibleModules().Count(x => !Results[x.Identifier].IsHidden) == 0)
                        continue;

                    _visibleUnits.Add(unit);

                    if (_visibleUnits.Count == 1 && !Course.AllowMultipleUnits)
                        break;
                }
            }

            return _visibleUnits;
        }

        private List<Unit> _allUnits;
        public List<Unit> GetAllUnits()
        {
            if (_allUnits == null)
            {
                _allUnits = new List<Unit>();

                foreach (var unit in Course.Units)
                {
                    if (!Results.TryGetValue(unit.Identifier, out var result))
                        continue;

                    if (GetAllModules().Count(x => !Results[x.Identifier].IsHidden) == 0)
                        continue;

                    _allUnits.Add(unit);

                    if (_allUnits.Count == 1 && !Course.AllowMultipleUnits)
                        break;
                }
            }

            return _allUnits;
        }

        private List<Module> _allModules;
        public List<Module> GetAllModules()
        {
            if (_allModules == null)
            {
                _allModules = new List<Module>();

                foreach (var unit in Course.Units)
                {
                    foreach (var module in unit.Modules)
                    {
                        if (!Results.TryGetValue(module.Identifier, out var result))
                            continue;

                        _allModules.Add(module);
                    }
                }
            }

            return _allModules;
        }

        private List<Activity> _allActivities;
        public List<Activity> GetAllActivities()
        {
            if (_allActivities == null)
            {
                _allActivities = new List<Activity>();

                foreach (var activity in Course.GetAllSupportedActivities())
                {
                    if (!Results.TryGetValue(activity.Identifier, out var result))
                        continue;
                    
                    _allActivities.Add(activity);
                }
            }

            return _allActivities;
        }

        public string CreateCssClassForActivityRow(Guid activityId)
        {
            var result = Results[activityId];

            if (result.Navigation.IsSelected)
                return "table-warning";

            return string.Empty;
        }

        #endregion
    }
}