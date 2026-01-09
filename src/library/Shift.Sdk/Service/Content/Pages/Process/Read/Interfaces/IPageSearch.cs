using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Domain.Foundations;

namespace InSite.Application.Sites.Read
{
    public interface IPageSearch
    {
        QPage GetPage(Guid id);

        QPage[] Select(Expression<Func<QPage, bool>> filter, params Expression<Func<QPage, object>>[] includes);
        VAssessmentPage[] Select(VAssessmentPageFilter filter);
        VAssessmentPage GetAssessmentPage(Guid pageId);
        VAssessmentPage[] GetAssessmentPages(Guid[] formIdentifiers);

        QPage Select(Guid id, params Expression<Func<QPage, object>>[] includes);

        T BindFirst<T>(
            Expression<Func<QPage, T>> binder,
            Expression<Func<QPage, bool>> filter,
            string modelSort = null,
            string entitySort = null);

        T[] Bind<T>(
           Expression<Func<QPage, T>> binder,
           Expression<Func<QPage, bool>> filter,
           string modelSort = null,
           string entitySort = null);

        PageSearchItem[] GetPageSearchItems(QPageFilter filter);

        int Count(QPageFilter filter);
        int Count(VAssessmentPageFilter filter);

        T[] Bind<T>(
            Expression<Func<QPage, T>> binder,
            QPageFilter filter);

        bool Exists(Expression<Func<QPage, bool>> filter);

        List<Tuple<string, string>> GetCourseWebPages(Guid course);

        string GetPagePath(Guid id, bool includeHostName);

        QPage[] GetSitePages(Guid site);

        QPage[] GetTreePages(Guid page);

        QPage[] GetDownstreamPages(Guid page);

        int Count(Expression<Func<QPage, bool>> filter);

        List<Guid> GetPageChildrenIds(Guid page);

        List<QPage> GetReorderByResourceId(Guid resourceId, IEnumerable<Guid> data);

        List<QPage> GetReorderBySiteId(Guid siteId, IEnumerable<Guid> data);

        PageTree CreateTree(Guid site);

        byte[] SerializePage(Guid id);

        byte[] SerializeSite(Guid id);

        void LoadSite(Guid? parentOrganization, Guid organization, Guid user, QSiteExport exportSite, QSite site);

        void LoadPage(Guid? parentOrganization, Guid organization, Guid user, QPageExport exportPage, QPage page, Guid? webSiteIdentifier, Dictionary<string, Guid?> groups = null);

        void SavePageContent(QPageExport exportPage, QPage page);

    }
}
