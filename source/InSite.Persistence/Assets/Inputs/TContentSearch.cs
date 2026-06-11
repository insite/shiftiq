using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class TContentSearch : IContentSearch
    {
        public static TContentSearch Instance { get; } = new TContentSearch();

        #region Select by TContentFilter

        public static int Count(TContentFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static SearchResultList Select(TContentFilter filter)
        {
            var sortExpression = "ContentSnip";

            if (!string.IsNullOrEmpty(filter.OrderBy))
                sortExpression = filter.OrderBy;

            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<TContent> CreateQuery(TContentFilter filter, InternalDbContext db) =>
            CreateQuery(db.TContents.AsQueryable(), filter);

        private static IQueryable<TContent> CreateQuery(IQueryable<TContent> query, TContentFilter filter)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.ContainerIdentifier))
            {
                Guid.TryParse(filter.ContainerIdentifier, out var container);
                if (container != null)
                {
                    query = query.Where(x => x.ContainerIdentifier == container);
                }
            }

            if (filter.ContentLabel.HasValue())
                query = query.Where(x => x.ContentLabel.Contains(filter.ContentLabel));

            if (filter.ContentLanguage.HasValue())
                query = query.Where(x => x.ContentLanguage == filter.ContentLanguage);

            if (filter.TextHTML.HasValue())
                query = query.Where(x => x.ContentText.Contains(filter.TextHTML) || x.ContentHtml.Contains(filter.TextHTML));

            if (filter.ContainerType.HasValue())
                query = query.Where(x => x.ContainerType.Contains(filter.ContainerType));

            return query;
        }

        #endregion

        public string GetTooltipText(string labelName, Guid organizationIdentifier)
        {
            string LabelName = $"{labelName}.[Tooltip]";

            var contents = SelectContainers(
                x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier && x.ContentLabel == LabelName && x.ContainerType == ContentContainerType.Application);

            if (contents == null || contents.Length == 0)
                return null;

            var org = contents.FirstOrDefault(x => x.OrganizationIdentifier == organizationIdentifier);
            if (org != null)
                return org.ContentText;

            var global = contents.FirstOrDefault(x => x.OrganizationIdentifier == OrganizationIdentifiers.Global);
            if (global != null)
                return global.ContentText;

            return null;
        }

        public TGroupPermission[] SelectPrivacyGroup(Expression<Func<TGroupPermission, bool>> expr, params Expression<Func<TGroupPermission, object>>[] includes)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.TGroupPermissions.Where(expr).ApplyIncludes(includes);

                return query.ToArray();
            }
        }

        public int Count(Expression<Func<TContent, bool>> expr)
        {
            using (var db = new InternalDbContext())
            {
                return db
                    .TContents
                    .Count(expr);
            }
        }

        public bool Exists(Expression<Func<TContent, bool>> expr)
        {
            using (var db = new InternalDbContext())
                return db.TContents.Where(expr).Any();
        }

        public TContent Select(Guid content)
        {
            using (var db = new InternalDbContext())
            {
                return db.TContents
                    .Where(x => x.ContentIdentifier == content)
                    .FirstOrDefault();
            }
        }

        public TContent SelectContainer(Guid id, string label, string language)
        {
            using (var db = new InternalDbContext())
            {
                var content = db.TContents
                    .Where(x => x.ContainerIdentifier == id && x.ContentLabel == label && x.ContentLanguage == language)
                    .FirstOrDefault();
                return content;
            }
        }

        public TContent[] SelectContainerByLabel(Guid id, string label)
        {
            using (var db = new InternalDbContext())
            {
                return db.TContents
                    .Where(x => x.ContainerIdentifier == id && x.ContentLabel == label)
                    .ToArray();
            }
        }

        public TContent[] SelectContainerByLanguage(Guid id, string language)
        {
            using (var db = new InternalDbContext())
            {
                return db.TContents
                    .Where(x => x.ContainerIdentifier == id && x.ContentLanguage == language)
                    .ToArray();
            }
        }

        public TContent[] SelectContainer(Guid id)
        {
            using (var db = new InternalDbContext())
                return db.TContents.Where(x => x.ContainerIdentifier == id).ToArray();
        }

        public TContent[] SelectContainer(IEnumerable<Guid> ids, string language = null, IEnumerable<string> labels = null)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.TContents.AsQueryable().Where(x => ids.Contains(x.ContainerIdentifier));

                if (!string.IsNullOrEmpty(language))
                    query = query.Where(x => x.ContentLanguage == language);

                if (labels != null && labels.Any())
                    query = query.Where(x => labels.Contains(x.ContentLabel));

                return query.ToArray();
            }
        }

        public TContent[] SelectContainers(Expression<Func<TContent, bool>> expr)
        {
            using (var db = new InternalDbContext())
                return db.TContents.Where(expr).ToArray();
        }

        public Shift.Common.ContentContainer GetBlock(Guid id, string language = null, IEnumerable<string> labels = null)
        {
            using (var db = new InternalDbContext())
            {
                var hasFilter = false;
                var query = db.TContents.AsQueryable().Where(x => x.ContainerIdentifier == id);

                if (!string.IsNullOrEmpty(language))
                {
                    query = query.Where(x => x.ContentLanguage == language);
                    hasFilter = true;
                }

                if (labels != null && labels.Any())
                {
                    query = query.Where(x => labels.Contains(x.ContentLabel));
                    hasFilter = true;
                }

                var content = GetBlock(query);
                content.IsLoaded = !hasFilter;
                return content;
            }
        }

        public Shift.Common.ContentContainer GetContentContainerCopy(Guid id, string[] contentFields)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.TContents.AsQueryable().Where(x => x.ContainerIdentifier == id);

                var currentContent = GetBlock(query);
                var content = new Shift.Common.ContentContainer();

                content = ContentContainerValues(currentContent, content, contentFields);

                return content;
            }
        }

        public Shift.Common.ContentContainer GetTooltipBlock(Guid id, string language = null, IEnumerable<string> labels = null)
        {
            using (var db = new InternalDbContext())
            {
                var hasFilter = false;
                var query = db.TContents.AsQueryable().Where(x => x.OrganizationIdentifier == id && x.ContentLabel.Contains("Tooltip"));

                if (!string.IsNullOrEmpty(language))
                {
                    query = query.Where(x => x.ContentLanguage == language);
                    hasFilter = true;
                }

                if (labels != null && labels.Any())
                {
                    query = query.Where(x => labels.Contains(x.ContentLabel));
                    hasFilter = true;
                }

                var content = GetBlock(query);
                content.IsLoaded = !hasFilter;
                return content;
            }
        }

        public IDictionary<Guid, Shift.Common.ContentContainer> GetBlocks(IEnumerable<Guid> ids, string language, IEnumerable<string> labels = null) =>
            GetBlocks(ids, language.IsNotEmpty() ? new[] { language } : null, labels);

        public IDictionary<Guid, Shift.Common.ContentContainer> GetBlocks(IEnumerable<Guid> ids, IEnumerable<string> languages = null, IEnumerable<string> labels = null)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.TContents.AsQueryable().Where(x => ids.Contains(x.ContainerIdentifier));

                if (languages != null)
                    query = query.Where(x => languages.Contains(x.ContentLanguage));

                if (labels != null && labels.Any())
                    query = query.Where(x => labels.Contains(x.ContentLabel));

                return query.GroupBy(x => x.ContainerIdentifier).ToDictionary(g => g.Key, g => GetBlock(g));
            }
        }

        public ContentContainer GetBlock(IEnumerable<TContent> entities)
        {
            var result = new ContentContainer();

            foreach (var e in entities)
            {
                if (string.IsNullOrEmpty(e.ContentLabel))
                    continue;

                var item = result[e.ContentLabel];

                item.Html[e.ContentLanguage] = e.ContentHtml;
                item.Text[e.ContentLanguage] = e.ContentText;
                item.Snip[e.ContentLanguage] = e.ContentSnip;
            }

            return result;
        }

        private Shift.Common.ContentContainer ContentContainerValues(Shift.Common.ContentContainer copyFrom, Shift.Common.ContentContainer copyTo, string[] contentFields)
        {
            foreach (var name in contentFields)
            {
                var item = copyTo[name];
                item.Html = copyFrom[name].Html;
                item.Text = copyFrom[name].Text;
                item.Snip = copyFrom[name].Snip;
            }
            return copyTo;
        }

        public string[] GetLanguages(Guid id, params string[] labels)
        {
            var result = new Shift.Common.ContentContainer();

            using (var db = new InternalDbContext())
            {
                var query = db.TContents.AsQueryable().Where(x => x.ContainerIdentifier == id);

                if (labels.IsNotEmpty())
                    query = query.Where(x => labels.Contains(x.ContentLabel));

                return query.Select(x => x.ContentLanguage).OrderBy(x => x).Distinct().ToArray();
            }
        }

        public string GetTitleText(Guid container, string language = Shift.Common.ContentContainer.DefaultLanguage) =>
            GetText(container, ContentLabel.Title, language);

        public string GetText(Guid container, string label, string language = Shift.Common.ContentContainer.DefaultLanguage)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentNullException(nameof(label));

            if (string.IsNullOrEmpty(language))
                language = Shift.Common.ContentContainer.DefaultLanguage;

            var content = GetBlock(container, language, new[] { label });

            return content.GetText(label, language);
        }

        public string GetHtml(Guid container, string label, string language = Shift.Common.ContentContainer.DefaultLanguage)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentNullException(nameof(label));

            if (string.IsNullOrEmpty(language))
                language = Shift.Common.ContentContainer.DefaultLanguage;

            var content = GetBlock(container, language, new[] { label });

            return content.GetHtml(label, language);
        }

        public string GetSnip(Guid container, string label, string language = Shift.Common.ContentContainer.DefaultLanguage)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentNullException(nameof(label));

            if (string.IsNullOrEmpty(language))
                language = Shift.Common.ContentContainer.DefaultLanguage;

            var content = GetBlock(container, language, new[] { label });

            return content.GetSnip(label, language);
        }

        public IReadOnlyList<StandardContentLabelCount> GetStandardContentLabelCounts(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.TContents
                    .Where(x => x.OrganizationIdentifier == organization)
                    .GroupBy(x => new { x.ContainerType, x.ContentLabel })
                    .Select(x => new StandardContentLabelCount
                    {
                        ContainerType = x.Key.ContainerType,
                        ContentLabel = x.Key.ContentLabel,
                        Contents = x.Count()
                    })
                    .OrderBy(x => x.ContainerType)
                    .ThenBy(x => x.ContentLabel)
                    .ToList();
            }
        }

        public HelpTopic[] GetHelpTopics()
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<HelpTopic>("SELECT * FROM content.VHelpTopic").ToArray();
            }
        }

        public HelpTopicContent[] GetHelpTopicsContents()
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<HelpTopicContent>("SELECT * FROM content.VHelpTopicContent").ToArray();
            }
        }

        #region Helpers

        public Dictionary<string, string> GetTitles(Guid key)
        {
            var dictionary = new Dictionary<string, string>();

            var id = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == key);

            if (id != Guid.Empty)
            {
                var contents = SelectContainerByLabel(id, ContentLabel.Title);
                foreach (var content in contents)
                    dictionary.Add(content.ContentLanguage, content.ContentText);
            }

            return dictionary;
        }

        #endregion
    }
}
