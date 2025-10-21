using System;

using InSite.Application.Courses.Read;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Courses;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Courses.Courses
{
    public static class EntityHelper
    {
        internal static QCourse CreateCourse(string name)
        {
            var identity = CurrentSessionState.Identity;

            var course = new QCourse
            {
                CourseAsset = Sequence.Increment(identity.Organization.Identifier, SequenceType.Asset),
                CourseIdentifier = UniqueIdentifier.Create(),
                CourseName = name,
                OrganizationIdentifier = identity.Organization.Identifier,
                CourseSlug = StringHelper.Sanitize(name, '-'),
            };

            return course;
        }

        internal static QUnit CreateUnit(Guid course, string name)
        {
            var identity = CurrentSessionState.Identity;

            var unit = new QUnit
            {
                CourseIdentifier = course,

                UnitAsset = Sequence.Increment(identity.Organization.Identifier, SequenceType.Asset),
                UnitIdentifier = UniqueIdentifier.Create(),
                UnitName = name,
                UnitSequence = 1,
            };

            return unit;
        }

        internal static QModule CreateModule(Guid unit, string name)
        {
            var identity = CurrentSessionState.Identity;

            var module = new QModule
            {
                UnitIdentifier = unit,

                ModuleAsset = Sequence.Increment(identity.Organization.Identifier, SequenceType.Asset),
                ModuleIdentifier = UniqueIdentifier.Create(),
                ModuleName = name,
                ModuleSequence = 1,
            };

            return module;
        }

        internal static QActivity CreateActivity(Guid module, ActivityType type, string name)
        {
            var identity = CurrentSessionState.Identity;

            var activity = new QActivity
            {
                ActivityAsset = Persistence.Sequence.Increment(identity.Organization.Identifier, SequenceType.Asset),
                ActivityIdentifier = UniqueIdentifier.Create(),
                ActivityName = name,
                ActivityType = type.ToString(),

                ModuleIdentifier = module,

                ActivityAuthorDate = DateTime.Today,
                ActivityAuthorName = identity.User.FullName,
            };

            if (string.IsNullOrWhiteSpace(activity.ActivityName))
                activity.ActivityName = "-";

            return activity;
        }

        internal static void RemoveFile(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.StartsWith(Common.Web.HttpRequestHelper.CurrentRootUrlFiles))
                return;

            var path = url.Substring(Common.Web.HttpRequestHelper.CurrentRootUrlFiles.Length);

            FileHelper.Provider.Delete(CurrentSessionState.Identity.Organization.OrganizationIdentifier, path);
        }
    }
}