using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.CourseObjects;

using Shift.Common;

namespace InSite.Domain.Foundations
{
    public sealed class DomainCache
    {
        #region Singleton

        public static DomainCache Instance { get; } = new DomainCache();

        static DomainCache() { }

        private DomainCache()
        {
            _courses = new Dictionary<Guid, Course>();

            _sites = new Dictionary<Guid, ISiteOutline>();
            _domains = new Dictionary<string, Guid>();
        }

        #endregion

        #region Caching

        private readonly object _courseSyncRoot = new object();
        private Dictionary<Guid, Course> _courses;

        private readonly object _siteSyncRoot = new object();
        private Dictionary<Guid, ISiteOutline> _sites;
        private Dictionary<string, Guid> _domains;

        #endregion

        public Course GetCourse(Guid courseId, Func<Course> create)
        {
            lock (_courseSyncRoot)
            {
                if (!_courses.TryGetValue(courseId, out var course))
                {
                    course = create();

                    if (course != null)
                        _courses.Add(courseId, course);
                }

                return course;
            }
        }

        public void RemoveCourse(Guid courseId)
        {
            lock (_courseSyncRoot)
                _courses.Remove(courseId);
        }

        public void RemoveCourses(Func<Course, bool> filter)
        {
            lock (_courseSyncRoot)
            {
                var courses = _courses.Values.Where(x => filter(x)).ToArray();
                foreach (var c in courses)
                    RemoveCourse(c.Identifier);
            }
        }

        public ISiteOutline GetSite(Guid site)
        {
            lock (_siteSyncRoot)
                return _sites.GetOrDefault(site);
        }

        public ISiteOutline GetSite(string domain)
        {
            lock (_siteSyncRoot)
                return _domains.TryGetValue(domain, out var id) ? GetSite(id) : null;
        }

        public void AddSite(ISiteOutline site)
        {
            lock (_siteSyncRoot)
            {
                _sites.Add(site.Identifier, site);
                _domains.Add(site.Domain, site.Identifier);
            }
        }

        public void RemoveSite(Guid siteId)
        {
            lock (_siteSyncRoot)
            {
                if (!_sites.TryGetValue(siteId, out var site))
                    return;

                _sites.Remove(site.Identifier);
                _domains.Remove(site.Domain);
            }
        }
    }
}
