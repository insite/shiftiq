using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class UnitCommandCreator
    {
        private readonly QUnit _old;
        private readonly ContentContainer _oldContent;
        private readonly QUnit _new;
        private readonly ContentContainer _newContent;
        private readonly List<ICommand> _result;

        private UnitCommandCreator(QUnit oldUnit, ContentContainer oldContent, QUnit newUnit, ContentContainer newContent, List<ICommand> commands)
        {
            _old = oldUnit;
            _oldContent = oldContent;
            _new = newUnit;
            _newContent = newContent;
            _result = commands;
        }

        public static List<ICommand> Create(QUnit oldUnit, ContentContainer oldContent, QUnit newUnit, ContentContainer newContent, List<ICommand> commands)
        {
            return new UnitCommandCreator(oldUnit, oldContent, newUnit, newContent, commands).Create();
        }

        private List<ICommand> Create()
        {
            AddCreateCommand();
            AddModifyCommands();

            return _result;
        }

        private void AddCreateCommand()
        {
            if (_old != null)
                return;

            if (_newContent == null)
                throw new ArgumentNullException("newContent");

            _result.Add(new AddCourseUnit(_new.CourseIdentifier, _new.UnitIdentifier, _new.UnitAsset, _new.UnitName, _newContent));
        }

        private void AddModifyCommands()
        {
            if (_old != null)
            {
                if (_oldContent != null && _newContent != null && !_oldContent.IsEqual(_newContent) || _oldContent == null && _newContent != null)
                    _result.Add(new ModifyCourseUnitContent(_new.CourseIdentifier, _new.UnitIdentifier, _newContent));

                if (!StringHelper.EqualsCaseSensitive(_old.UnitName, _new.UnitName, true))
                    _result.Add(new RenameCourseUnit(_new.CourseIdentifier, _new.UnitIdentifier, _new.UnitName));
            }

            if (_old?.UnitSequence != _new.UnitSequence && _new.UnitSequence != -1)
                _result.Add(new ModifyCourseUnitSequence(_new.CourseIdentifier, _new.UnitIdentifier, _new.UnitSequence));

            if (!StringHelper.EqualsCaseSensitive(_old?.UnitCode, _new.UnitCode, true))
                _result.Add(new ModifyCourseUnitCode(_new.CourseIdentifier, _new.UnitIdentifier, _new.UnitCode));

            if (!StringHelper.EqualsCaseSensitive(_old?.PrerequisiteDeterminer, _new.PrerequisiteDeterminer, true))
                _result.Add(new ModifyCourseUnitPrerequisiteDeterminer(_new.CourseIdentifier, _new.UnitIdentifier, _new.PrerequisiteDeterminer));

            if (_old?.SourceIdentifier != _new.SourceIdentifier)
                _result.Add(new ModifyCourseUnitSource(_new.CourseIdentifier, _new.UnitIdentifier, _new.SourceIdentifier));

            if (_old?.UnitIsAdaptive != _new.UnitIsAdaptive)
                _result.Add(new ModifyCourseUnitAdaptive(_new.CourseIdentifier, _new.UnitIdentifier, _new.UnitIsAdaptive));
        }
    }
}
