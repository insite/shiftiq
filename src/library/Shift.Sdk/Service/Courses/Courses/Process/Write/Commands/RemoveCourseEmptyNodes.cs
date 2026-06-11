using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseEmptyNodes : Command, IHasRun
    {
        public RemoveCourseEmptyNodes(Guid courseId)
        {
            AggregateIdentifier = courseId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var removeUnits = new List<Unit>();
            var removeModules = new List<Module>();

            foreach (var unit in course.Data.Units)
            {
                var empty = true;
                var emptyModules = new List<Module>();

                foreach (var module in unit.Modules)
                {
                    if (module.Activities.Count == 0)
                        emptyModules.Add(module);
                    else
                        empty = false;
                }

                if (empty)
                    removeUnits.Add(unit);
                else
                    removeModules.AddRange(emptyModules);
            }

            if (removeUnits.Count == 0 && removeModules.Count == 0)
                return false;

            foreach (var unit in removeUnits)
                course.Apply(new CourseUnitRemoved(unit.Identifier));

            foreach (var module in removeModules)
                course.Apply(new CourseModuleRemoved(module.Identifier));

            return true;
        }
    }
}
