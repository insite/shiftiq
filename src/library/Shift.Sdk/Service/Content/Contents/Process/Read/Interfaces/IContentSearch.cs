using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;

using Shift.Common;
namespace InSite.Application.Contents.Read
{
    public class HelpTopic
    {
        public Guid TopicIdentifier { get; set; }
        public Guid? ParentIdentifier { get; set; }

        public string SiteTitle { get; set; }
        public string SiteDomain { get; set; }

        public string TopicFields { get; set; }
        public string TopicIcon { get; set; }
        public bool TopicIsHidden { get; set; }
        public string TopicLayout { get; set; }
        public DateTimeOffset? TopicModified { get; set; }
        public int TopicSequence { get; set; }
        public string TopicSlug { get; set; }
        public string TopicTitle { get; set; }
        public string TopicType { get; set; }
        public string TopicUrl { get; set; }

        public DateTimeOffset? AuthorDate { get; set; }
        public string AuthorName { get; set; }
    }

    public class HelpTopicContent
    {
        public Guid TopicIdentifier { get; set; }

        public string ContentField { get; set; }
        public string ContentLanguage { get; set; }
        public string ContentText { get; set; }
        public string ContentHtml { get; set; }
    }

    public interface IContentSearch
    {
        string GetTooltipText(string labelName, Guid organizationIdentifier);

        TGroupPermission[] SelectPrivacyGroup(Expression<Func<TGroupPermission, bool>> expr, params Expression<Func<TGroupPermission, object>>[] includes);

        int Count(Expression<Func<TContent, bool>> expr);

        bool Exists(Expression<Func<TContent, bool>> expr);
        TContent Select(Guid content); 
        TContent SelectContainer(Guid container, string label, string language);
        TContent[] SelectContainer(Guid container);
        TContent[] SelectContainer(IEnumerable<Guid> containers, string language = null, IEnumerable<string> labels = null);
        TContent[] SelectContainers(Expression<Func<TContent, bool>> expr);
        TContent[] SelectContainerByLabel(Guid container, string label);
        TContent[] SelectContainerByLanguage(Guid container, string language);

        string[] GetLanguages(Guid container, params string[] labels);
        
        Dictionary<string, string> GetTitles(Guid key);

        ContentContainer GetBlock(Guid container, string language = null, IEnumerable<string> labels = null);
        ContentContainer GetTooltipBlock(Guid container, string language = null, IEnumerable<string> labels = null);
        ContentContainer GetContentContainerCopy(Guid id, string[] contentFields);
        IDictionary<Guid, ContentContainer> GetBlocks(IEnumerable<Guid> containers, string language, IEnumerable<string> labels = null);
        IDictionary<Guid, ContentContainer> GetBlocks(IEnumerable<Guid> containers, IEnumerable<string> languages = null, IEnumerable<string> labels = null);
        ContentContainer GetBlock(IEnumerable<TContent> entities);

        string GetText(Guid container, string label, string language = ContentContainer.DefaultLanguage);
        string GetTitleText(Guid container, string language = ContentContainer.DefaultLanguage);
        string GetHtml(Guid container, string label, string language = ContentContainer.DefaultLanguage);
        string GetSnip(Guid container, string label, string language = ContentContainer.DefaultLanguage);

        IReadOnlyList<StandardContentLabelCount> GetStandardContentLabelCounts(Guid organization);

        HelpTopic[] GetHelpTopics();
        HelpTopicContent[] GetHelpTopicsContents();
    }
}
