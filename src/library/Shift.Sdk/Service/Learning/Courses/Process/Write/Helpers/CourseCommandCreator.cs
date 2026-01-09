using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;
using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class CourseCommandCreator
    {
        private readonly QCourse _old;
        private readonly ContentContainer _oldContent;
        private readonly QCourse _new;
        private readonly ContentContainer _newContent;
        private readonly List<ICommand> _result;

        private CourseCommandCreator(QCourse oldCourse, ContentContainer oldContent, QCourse newCourse, ContentContainer newContent, List<ICommand> commands)
        {
            _old = oldCourse;
            _oldContent = oldContent;
            _new = newCourse;
            _newContent = newContent;
            _result = commands;
        }

        public static List<ICommand> Create(QCourse oldCourse, ContentContainer oldContent, QCourse newCourse, ContentContainer newContent, List<ICommand> commands)
        {
            return new CourseCommandCreator(oldCourse, oldContent, newCourse, newContent, commands).Create();
        }

        private List<ICommand> Create()
        {
            AddCreateCommand();
            AddAssignCommands();
            AddConnectCommands();
            AddContentChange();
            AddTextChanges();
            AddBoolChanges();
            AddDateTimeOffsetChanges();
            AddIntChanges();
            AddGuidChanges();

            return _result;
        }

        private void AddCreateCommand()
        {
            if (_old != null)
                return;

            if (_newContent == null)
                throw new ArgumentNullException("newContent");

            _result.Add(new CreateCourse(_new.CourseIdentifier, _new.OrganizationIdentifier, _new.CourseAsset, _new.CourseName, _newContent));
        }

        private void AddAssignCommands()
        {
            if (_old?.CompletionActivityIdentifier != _new.CompletionActivityIdentifier)
                _result.Add(new ConfigureCourseCompletionActivity(_new.CourseIdentifier, _new.CompletionActivityIdentifier));

            if (_old?.FrameworkStandardIdentifier != _new.FrameworkStandardIdentifier)
                _result.Add(new ConnectCourseFramework(_new.CourseIdentifier, _new.FrameworkStandardIdentifier));

            if (_old?.StalledToLearnerMessageIdentifier != _new.StalledToLearnerMessageIdentifier
                || _old?.SendMessageStalledAfterDays != _new.SendMessageStalledAfterDays
                || _old?.SendMessageStalledMaxCount != _new.SendMessageStalledMaxCount
                )
            {
                _result.Add(new ConnectCourseMessage(
                    _new.CourseIdentifier,
                    CourseMessageType.StalledToLearner,
                    _new.StalledToLearnerMessageIdentifier,
                    _new.SendMessageStalledAfterDays,
                    _new.SendMessageStalledMaxCount
                ));
            }

            if (_old?.StalledToAdministratorMessageIdentifier != _new.StalledToAdministratorMessageIdentifier)
                _result.Add(new ConnectCourseMessage(_new.CourseIdentifier, CourseMessageType.StalledToAdministrator, _new.StalledToAdministratorMessageIdentifier, null, null));

            if (_old?.CompletedToLearnerMessageIdentifier != _new.CompletedToLearnerMessageIdentifier)
                _result.Add(new ConnectCourseMessage(_new.CourseIdentifier, CourseMessageType.CompletedToLearner, _new.CompletedToLearnerMessageIdentifier, null, null));

            if (_old?.CompletedToAdministratorMessageIdentifier != _new.CompletedToAdministratorMessageIdentifier)
                _result.Add(new ConnectCourseMessage(_new.CourseIdentifier, CourseMessageType.CompletedToAdministrator, _new.CompletedToAdministratorMessageIdentifier, null, null));
        }

        private void AddConnectCommands()
        {
            if (_old?.GradebookIdentifier != _new.GradebookIdentifier)
                _result.Add(new ConnectCourseGradebook(_new.CourseIdentifier, _new.GradebookIdentifier));

            if (_old?.CatalogIdentifier != _new.CatalogIdentifier)
                _result.Add(new ConnectCourseCatalog(_new.CourseIdentifier, _new.CatalogIdentifier));
        }

        private void AddContentChange()
        {
            if (_old != null && (_oldContent != null && _newContent != null && !_oldContent.IsEqual(_newContent) || _oldContent == null && _newContent != null))
                _result.Add(new ModifyCourseContent(_new.CourseIdentifier, _newContent));
        }

        private void AddTextChanges()
        {
            if (!StringHelper.EqualsCaseSensitive(_old?.CourseCode, _new.CourseCode, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseCode, _new.CourseCode));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseDescription, _new.CourseDescription, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseDescription, _new.CourseDescription));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseHook, _new.CourseHook, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseHook, _new.CourseHook));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseIcon, _new.CourseIcon, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseIcon, _new.CourseIcon));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseImage, _new.CourseImage, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseImage, _new.CourseImage));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseLabel, _new.CourseLabel, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseLabel, _new.CourseLabel));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseLevel, _new.CourseLevel, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseLevel, _new.CourseLevel));

            if (_old != null && !StringHelper.EqualsCaseSensitive(_old?.CourseName, _new.CourseName, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseName, _new.CourseName));

            if (!StringHelper.EqualsCaseSensitive(_old?.CoursePlatform, _new.CoursePlatform, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CoursePlatform, _new.CoursePlatform));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseProgram, _new.CourseProgram, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseProgram, _new.CourseProgram));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseSlug, _new.CourseSlug, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseSlug, _new.CourseSlug));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseFlagColor, _new.CourseFlagColor, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseFlagColor, _new.CourseFlagColor));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseFlagText, _new.CourseFlagText, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseFlagText, _new.CourseFlagText));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseStyle, _new.CourseStyle, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseStyle, _new.CourseStyle));

            if (!StringHelper.EqualsCaseSensitive(_old?.CourseUnit, _new.CourseUnit, true))
                _result.Add(new ModifyCourseFieldText(_new.CourseIdentifier, CourseField.CourseUnit, _new.CourseUnit));
        }

        private void AddBoolChanges()
        {
            if (_old?.AllowDiscussion != _new.AllowDiscussion)
                _result.Add(new ModifyCourseFieldBool(_new.CourseIdentifier, CourseField.AllowDiscussion, _new.AllowDiscussion));

            if (_old?.CourseIsHidden != _new.CourseIsHidden)
                _result.Add(new ModifyCourseFieldBool(_new.CourseIdentifier, CourseField.CourseIsHidden, _new.CourseIsHidden));

            if (_old?.IsMultipleUnitsEnabled != _new.IsMultipleUnitsEnabled)
                _result.Add(new ModifyCourseFieldBool(_new.CourseIdentifier, CourseField.IsMultipleUnitsEnabled, _new.IsMultipleUnitsEnabled));

            if (_old?.IsProgressReportEnabled != _new.IsProgressReportEnabled)
                _result.Add(new ModifyCourseFieldBool(_new.CourseIdentifier, CourseField.IsProgressReportEnabled, _new.IsProgressReportEnabled));
        }

        private void AddIntChanges()
        {
            if (_old?.CourseSequence != _new.CourseSequence)
                _result.Add(new ModifyCourseFieldInt(_new.CourseIdentifier, CourseField.CourseSequence, _new.CourseSequence));

            if (_old?.OutlineWidth != _new.OutlineWidth)
                _result.Add(new ModifyCourseFieldInt(_new.CourseIdentifier, CourseField.OutlineWidth, _new.OutlineWidth));
        }

        private void AddGuidChanges()
        {
            if (_old?.SourceIdentifier != _new.SourceIdentifier)
                _result.Add(new ModifyCourseFieldGuid(_new.CourseIdentifier, CourseField.SourceIdentifier, _new.SourceIdentifier));
        }

        private void AddDateTimeOffsetChanges()
        {
            if (_old?.Closed != _new.Closed)
                _result.Add(new ModifyCourseFieldDateTimeOffset(_new.CourseIdentifier, CourseField.Closed, _new.Closed));
        }
    }
}
