using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Achievements.Write;
using InSite.Application.Banks.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Quizzes.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Courses.Write
{
    public class CourseObjectCommandGenerator
    {
        private Func<Guid, QBankForm> _getForm;
        private Func<Guid, TQuiz> _getQuiz;

        private StringDictionary _usedCodes = new StringDictionary();
        private List<ICommand> _commands = new List<ICommand>();
        private Dictionary<Guid, Guid> _mapActivityToGradeItem = new Dictionary<Guid, Guid>();

        public ICommand[] Commands => _commands.ToArray();
        public Dictionary<Guid, Guid> MapActivityToGradeItem => _mapActivityToGradeItem;

        public CourseObjectCommandGenerator(Func<Guid, QBankForm> getForm, Func<Guid, TQuiz> getQuiz)
        {
            _getForm = getForm;
            _getQuiz = getQuiz;
        }

        public void CreateCourseGradebook(QCourse course, QUnit[] units, Guid gradebook, string gradebookName, string achievementType, string certificateLayout, decimal? passPercent)
        {
            var achievement = UuidFactory.Create();

            _commands.Add(new CreateAchievement(achievement, course.OrganizationIdentifier, achievementType, gradebookName, null, null));

            if (!string.IsNullOrEmpty(certificateLayout))
                _commands.Add(new ChangeCertificateLayout(achievement, certificateLayout));

            _commands.Add(new CreateGradebook(gradebook, course.OrganizationIdentifier, gradebookName, GradebookType.Scores, null, achievement, null));

            var root = CreateGradeCategory(gradebook, passPercent);
            _commands.Add(root);

            _commands.Add(ConfigureAchievement(achievement, gradebook, root.Item));

            int unitSequence = 1;

            foreach (var unit in units.OrderBy(x => x.UnitSequence))
            {
                var categoryUnit = CreateGradeCategory(gradebook, root.Item, unit, unitSequence, passPercent);
                _commands.Add(categoryUnit);

                int moduleSequence = 1;

                foreach (var module in unit.Modules.OrderBy(x => x.ModuleSequence))
                {
                    var category = CreateGradeCategory(gradebook, categoryUnit.Item, unit, module, unitSequence, moduleSequence, passPercent);
                    _commands.Add(category);

                    int activitySequence = 1;

                    foreach (var activity in module.Activities.OrderBy(x => x.ActivitySequence))
                        _commands.AddRange(CreateGradeItem(gradebook, category.Item, activity, unitSequence, moduleSequence, activitySequence++, passPercent));

                    moduleSequence++;
                }

                unitSequence++;
            }
        }

        private AddGradeItem CreateGradeCategory(Guid gradebook, decimal? passPercent)
        {
            var id = UuidFactory.Create();
            var code = "Final";
            var name = "Final Score";

            return new AddGradeItem(gradebook, id, code, name, null, true, GradeItemFormat.None, GradeItemType.Category, GradeItemWeighting.EquallyWithNulls, passPercent, null);
        }

        private ChangeGradeItemAchievement ConfigureAchievement(Guid achievement, Guid gradebook, Guid gradeitem)
        {
            var itemAchievement = new GradeItemAchievement
            {
                Achievement = achievement,
                WhenChange = TriggerCauseChange.Changed,
                WhenGrade = TriggerCauseGrade.Pass,
                ThenCommand = TriggerEffectCommand.Grant,
                ElseCommand = TriggerEffectCommand.Void
            };

            return new ChangeGradeItemAchievement(gradebook, gradeitem, itemAchievement);
        }

        private AddGradeItem CreateGradeCategory(Guid gradebook, Guid category, QUnit unit, int unitSequence, decimal? passPercent)
        {
            var id = UuidFactory.Create();
            var code = $"{unitSequence}";
            var name = unit.UnitName;

            return new AddGradeItem(gradebook, id, code, name, null, true, GradeItemFormat.None, GradeItemType.Category, GradeItemWeighting.EquallyWithNulls, passPercent, category);
        }

        private AddGradeItem CreateGradeCategory(Guid gradebook, Guid category, QUnit unit, QModule module, int unitSequence, int moduleSequence, decimal? passPercent)
        {
            var moduleId = UuidFactory.Create();
            var moduleCode = $"{unitSequence}.{moduleSequence}";
            var moduleName = module.ModuleName;

            return new AddGradeItem(gradebook, moduleId, moduleCode, moduleName, null, true, GradeItemFormat.None, GradeItemType.Category, GradeItemWeighting.EquallyWithNulls, passPercent, category);
        }

        private IEnumerable<ICommand> CreateGradeItem(Guid gradebook, Guid category, QActivity activity, int unitSequence, int moduleSequence, int activitySequence, decimal? passPercent)
        {
            var isAssessment = activity.ActivityType == "Assessment";
            var isQuiz = activity.ActivityType == "Quiz";
            var id = UuidFactory.Create();
            var name = activity.ActivityName;
            var format = isAssessment ? GradeItemFormat.Percent : GradeItemFormat.Boolean;
            var code = GetCode(unitSequence, moduleSequence, activitySequence);

            if (isAssessment && activity.AssessmentFormIdentifier.HasValue)
            {
                var form = _getForm(activity.AssessmentFormIdentifier.Value);
                if (form != null && form.FormPassingScore.HasValue)
                    passPercent = form.FormPassingScore;
            }

            if (string.IsNullOrWhiteSpace(name))
                name = code;

            _mapActivityToGradeItem.Add(activity.ActivityIdentifier, id);

            yield return new AddGradeItem(gradebook, id, code, name, null, true, format, GradeItemType.Score, GradeItemWeighting.None, passPercent, category);

            if (isQuiz && activity.QuizIdentifier.HasValue)
            {
                var quiz = _getQuiz(activity.QuizIdentifier.Value);
                if (quiz != null)
                {
                    if (quiz.QuizType == QuizType.TypingSpeed)
                    {
                        yield return CreateQuizGradeItem(gradebook, id, GetCode(unitSequence, moduleSequence, activitySequence), QuizGradeItem.WordsPerMin);
                        yield return CreateQuizGradeItem(gradebook, id, GetCode(unitSequence, moduleSequence, activitySequence), QuizGradeItem.Accuracy);
                    }
                    else if (quiz.QuizType == QuizType.TypingAccuracy)
                    {
                        yield return CreateQuizGradeItem(gradebook, id, GetCode(unitSequence, moduleSequence, activitySequence), QuizGradeItem.KeystrokesPerHour);
                        yield return CreateQuizGradeItem(gradebook, id, GetCode(unitSequence, moduleSequence, activitySequence), QuizGradeItem.Accuracy);
                    }
                    else
                    {
                        throw ApplicationError.Create("Unexpected quiz type: " + quiz.QuizType);
                    }
                }
            }
        }

        private string GetCode(int unitSequence, int moduleSequence, int activitySequence)
        {
            var code = $"{unitSequence}.{moduleSequence}.{activitySequence}";
            var subcode = 1;

            while (_usedCodes.ContainsKey(code))
                code = $"{unitSequence}.{moduleSequence}.{activitySequence}.{subcode++}";

            _usedCodes.Add(code, code);

            return code;
        }

        private AddGradeItem CreateQuizGradeItem(Guid gradebookId, Guid parentId, string code, QuizGradeItem item)
        {
            return new AddGradeItem(
                gradebookId,
                UuidFactory.Create(),
                code,
                item.FullName,
                item.ShortName,
                true,
                item.Format,
                GradeItemType.Score,
                GradeItemWeighting.None,
                null,
                parentId
            );
        }
    }
}
