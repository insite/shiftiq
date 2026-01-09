using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModuleCommandCreator
    {
        private readonly Guid _courseId;
        private readonly QModule _old;
        private readonly ContentContainer _oldContent;
        private readonly QModule _new;
        private readonly ContentContainer _newContent;

        private readonly List<ICommand> _result = new List<ICommand>();

        private ModuleCommandCreator(Guid courseId, QModule oldModule, ContentContainer oldContent, QModule newModule, ContentContainer newContent, List<ICommand> commands)
        {
            _courseId = courseId;
            _old = oldModule;
            _oldContent = oldContent;
            _new = newModule;
            _newContent = newContent;
            _result = commands;
        }

        public static List<ICommand> Create(Guid courseId, QModule oldModule, ContentContainer oldContent, QModule newModule, ContentContainer newContent, List<ICommand> commands)
        {
            return new ModuleCommandCreator(courseId, oldModule, oldContent, newModule, newContent, commands).Create();
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

            _result.Add(new AddCourseModule(_courseId, _new.UnitIdentifier, _new.ModuleIdentifier, _new.ModuleAsset, _new.ModuleName, _newContent));
        }

        private void AddModifyCommands()
        {
            if (_old != null)
            {
                if (_old.UnitIdentifier != _new.UnitIdentifier)
                    _result.Add(new MoveCourseModule(_courseId, _new.ModuleIdentifier, _new.UnitIdentifier));

                if (_oldContent != null && _newContent != null && !_oldContent.IsEqual(_newContent) || _oldContent == null && _newContent != null)
                    _result.Add(new ModifyCourseModuleContent(_courseId, _new.ModuleIdentifier, _newContent));

                if (!StringHelper.EqualsCaseSensitive(_old.ModuleName, _new.ModuleName, true))
                    _result.Add(new RenameCourseModule(_courseId, _new.ModuleIdentifier, _new.ModuleName));
            }

            if (_old?.ModuleSequence != _new.ModuleSequence && _new.ModuleSequence != -1)
                _result.Add(new ModifyCourseModuleSequence(_courseId, _new.ModuleIdentifier, _new.ModuleSequence));

            if (!StringHelper.EqualsCaseSensitive(_old?.ModuleCode, _new.ModuleCode, true))
                _result.Add(new ModifyCourseModuleCode(_courseId, _new.ModuleIdentifier, _new.ModuleCode));

            if (!StringHelper.EqualsCaseSensitive(_old?.ModuleImage, _new.ModuleImage, true))
                _result.Add(new ModifyCourseModuleImage(_courseId, _new.ModuleIdentifier, _new.ModuleImage));

            if (!StringHelper.EqualsCaseSensitive(_old?.PrerequisiteDeterminer, _new.PrerequisiteDeterminer, true))
                _result.Add(new ModifyCourseModulePrerequisiteDeterminer(_courseId, _new.ModuleIdentifier, _new.PrerequisiteDeterminer));

            if (_old?.SourceIdentifier != _new.SourceIdentifier)
                _result.Add(new ModifyCourseModuleSource(_courseId, _new.ModuleIdentifier, _new.SourceIdentifier));

            if (_old?.ModuleIsAdaptive != _new.ModuleIsAdaptive)
                _result.Add(new ModifyCourseModuleAdaptive(_courseId, _new.ModuleIdentifier, _new.ModuleIsAdaptive));
        }
    }
}
