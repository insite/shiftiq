using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;
using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ActivityCommandCreator
    {
        private readonly Guid _courseId;
        private readonly QActivity _old;
        private readonly ContentContainer _oldContent;
        private readonly QActivity _new;
        private readonly ContentContainer _newContent;
        private readonly List<ICommand> _result;

        private ActivityCommandCreator(Guid courseId, QActivity oldActivity, ContentContainer oldContent, QActivity newActivity, ContentContainer newContent, List<ICommand> commands)
        {
            _courseId = courseId;
            _old = oldActivity;
            _oldContent = oldContent;
            _new = newActivity;
            _newContent = newContent;
            _result = commands;
        }

        public static List<ICommand> Create(Guid courseId, QActivity oldActivity, ContentContainer oldContent, QActivity newActivity, ContentContainer newContent, List<ICommand> commands)
        {
            return new ActivityCommandCreator(courseId, oldActivity, oldContent, newActivity, newContent, commands).Create();
        }

        private List<ICommand> Create()
        {
            AddCreateCommand();
            AddModifyCommands();
            AddConnectCommands();
            AddUrlChanges();
            AddTextChanges();
            AddBoolChanges();
            AddIntChanges();
            AddGuidChanges();
            AddDateChanges();

            return _result;
        }

        private void AddCreateCommand()
        {
            if (_old != null)
                return;

            if (_newContent == null)
                throw new ArgumentNullException("newContent");

            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), _new.ActivityType, true);

            _result.Add(new AddCourseActivity(
                _courseId,
                _new.ModuleIdentifier,
                _new.ActivityIdentifier,
                _new.ActivityAsset,
                _new.ActivityName,
                activityType,
                _newContent
            ));
        }

        private void AddModifyCommands()
        {
            if (_old != null)
            {
                if (_old.ModuleIdentifier != _new.ModuleIdentifier)
                    _result.Add(new MoveCourseActivity(_courseId, _new.ActivityIdentifier, _new.ModuleIdentifier));

                if (!string.Equals(_old.ActivityType, _new.ActivityType, StringComparison.OrdinalIgnoreCase))
                {
                    var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), _new.ActivityType, true);
                    _result.Add(new ModifyCourseActivityType(_courseId, _new.ActivityIdentifier, activityType));
                }

                if (_oldContent != null && _newContent != null && !_oldContent.IsEqual(_newContent) || _oldContent == null && _newContent != null)
                    _result.Add(new ModifyCourseActivityContent(_courseId, _new.ActivityIdentifier, _newContent));
            }

            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityUrl, _new.ActivityUrl, true)
                || !StringHelper.EqualsCaseSensitive(_old?.ActivityUrlTarget, _new.ActivityUrlTarget, true)
                || !StringHelper.EqualsCaseSensitive(_old?.ActivityUrlType, _new.ActivityUrlType, true)
                )
            {
                _result.Add(new ModifyCourseActivityUrl(_courseId, _new.ActivityIdentifier, _new.ActivityUrl, _new.ActivityUrlTarget, _new.ActivityUrlType));
            }
        }

        private void AddConnectCommands()
        {
            if (_old?.AssessmentFormIdentifier != _new.AssessmentFormIdentifier)
                _result.Add(new ConnectCourseActivityAssessmentForm(_courseId, _new.ActivityIdentifier, _new.AssessmentFormIdentifier));

            if (_old?.SurveyFormIdentifier != _new.SurveyFormIdentifier)
                _result.Add(new ConnectCourseActivitySurveyForm(_courseId, _new.ActivityIdentifier, _new.SurveyFormIdentifier));

            if (_old?.GradeItemIdentifier != _new.GradeItemIdentifier)
                _result.Add(new ConnectCourseActivityGradeItem(_courseId, _new.ActivityIdentifier, _new.GradeItemIdentifier));

            if (_old?.PrerequisiteActivityIdentifier != _new.PrerequisiteActivityIdentifier)
                _result.Add(new ConnectCourseActivityLegacyPrerequisite(_courseId, _new.ActivityIdentifier, _new.PrerequisiteActivityIdentifier));

            if (_old?.QuizIdentifier != _new.QuizIdentifier)
                _result.Add(new ConnectCourseActivityQuiz(_courseId, _new.ActivityIdentifier, _new.QuizIdentifier));
        }

        private void AddUrlChanges()
        {
            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityUrl, _new.ActivityUrl, true)
                || !StringHelper.EqualsCaseSensitive(_old?.ActivityUrlTarget, _new.ActivityUrlTarget, true)
                || !StringHelper.EqualsCaseSensitive(_old?.ActivityUrlType, _new.ActivityUrlType, true)
                )
            {
                _result.Add(new ModifyCourseActivityUrl(_courseId, _new.ActivityIdentifier, _new.ActivityUrl, _new.ActivityUrlTarget, _new.ActivityUrlType));
            }
        }

        private void AddTextChanges()
        {
            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityAuthorName, _new.ActivityAuthorName, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityAuthorName, _new.ActivityAuthorName));

            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityCode, _new.ActivityCode, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityCode, _new.ActivityCode));

            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityHook, _new.ActivityHook, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityHook, _new.ActivityHook));

            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityImage, _new.ActivityImage, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityImage, _new.ActivityImage));

            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityMode, _new.ActivityMode, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityMode, _new.ActivityMode));

            if (_old != null && !StringHelper.EqualsCaseSensitive(_old.ActivityName, _new.ActivityName, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityName, _new.ActivityName));

            if (!StringHelper.EqualsCaseSensitive(_old?.ActivityPlatform, _new.ActivityPlatform, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.ActivityPlatform, _new.ActivityPlatform));

            if (!StringHelper.EqualsCaseSensitive(_old?.PrerequisiteDeterminer, _new.PrerequisiteDeterminer, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.PrerequisiteDeterminer, _new.PrerequisiteDeterminer));

            if (!StringHelper.EqualsCaseSensitive(_old?.RequirementCondition, _new.RequirementCondition, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.RequirementCondition, _new.RequirementCondition));

            if (!StringHelper.EqualsCaseSensitive(_old?.DoneButtonText, _new.DoneButtonText, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.DoneButtonText, _new.DoneButtonText));

            if (!StringHelper.EqualsCaseSensitive(_old?.DoneButtonInstructions, _new.DoneButtonInstructions, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.DoneButtonInstructions, _new.DoneButtonInstructions));

            if (!StringHelper.EqualsCaseSensitive(_old?.DoneMarkedInstructions, _new.DoneMarkedInstructions, true))
                _result.Add(new ModifyCourseActivityFieldText(_courseId, _new.ActivityIdentifier, ActivityField.DoneMarkedInstructions, _new.DoneMarkedInstructions));
        }

        private void AddBoolChanges()
        {
            if (_old?.ActivityIsMultilingual != _new.ActivityIsMultilingual)
                _result.Add(new ModifyCourseActivityFieldBool(_courseId, _new.ActivityIdentifier, ActivityField.ActivityIsMultilingual, _new.ActivityIsMultilingual));

            if (_old?.ActivityIsAdaptive != _new.ActivityIsAdaptive)
                _result.Add(new ModifyCourseActivityFieldBool(_courseId, _new.ActivityIdentifier, ActivityField.ActivityIsAdaptive, _new.ActivityIsAdaptive));

            if (_old?.ActivityIsDispatch != _new.ActivityIsDispatch)
                _result.Add(new ModifyCourseActivityFieldBool(_courseId, _new.ActivityIdentifier, ActivityField.ActivityIsDispatch, _new.ActivityIsDispatch));
        }

        private void AddIntChanges()
        {
            if (_old?.ActivityMinutes != _new.ActivityMinutes)
                _result.Add(new ModifyCourseActivityFieldInt(_courseId, _new.ActivityIdentifier, ActivityField.ActivityMinutes, _new.ActivityMinutes));

            if (_old?.ActivitySequence != _new.ActivitySequence && _new.ActivitySequence != -1)
                _result.Add(new ModifyCourseActivityFieldInt(_courseId, _new.ActivityIdentifier, ActivityField.ActivitySequence, _new.ActivitySequence));
        }

        private void AddGuidChanges()
        {
            if (_old?.SourceIdentifier != _new.SourceIdentifier)
                _result.Add(new ModifyCourseActivityFieldGuid(_courseId, _new.ActivityIdentifier, ActivityField.SourceIdentifier, _new.SourceIdentifier));
        }

        private void AddDateChanges()
        {
            if (_old?.ActivityAuthorDate != _new.ActivityAuthorDate)
                _result.Add(new ModifyCourseActivityFieldDate(_courseId, _new.ActivityIdentifier, ActivityField.ActivityAuthorDate, _new.ActivityAuthorDate));
        }
    }
}
