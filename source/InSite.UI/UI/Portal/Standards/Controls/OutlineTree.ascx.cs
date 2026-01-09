using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class OutlineTree : BaseUserControl
    {
        public class DataNode : StandardGraphNode
        {
            #region Properties

            public IEnumerable<DataNode> IncomingNodes => GetGraph().GetIncomingContainments(NodeId).Select(x => (DataNode)x.FromNode);
            public IEnumerable<DataNode> OutgoingNodes => GetGraph().GetOutgoingContainments(NodeId).Select(x => (DataNode)x.ToNode);

            public StandardGraph<StandardGraphNode> Graph => GetGraph();

            #endregion

            #region Construction

            public DataNode(Guid id) : base(id)
            {

            }

            #endregion
        }

        protected Guid StandardIdentifier
        {
            get => (Guid?)ViewState[nameof(StandardIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(StandardIdentifier)] = value;
        }

        public bool AllowSaveState
        {
            get => (bool)(ViewState[nameof(AllowSaveState)] ?? true);
            set => ViewState[nameof(AllowSaveState)] = value;
        }

        #region Data binding (hierarchy)

        public void LoadData(Guid standardId, GlossaryHelper glossary)
        {
            StandardIdentifier = standardId;

            var items = LoadPrimaryHierarchy(StandardIdentifier, glossary);

            var codePathMap = new Dictionary<Guid, string>();

            BuildCodePath(null, items, codePathMap);
            LoadConnections(items, codePathMap, glossary);

            CalculateFulfillment(items);

            OutlineNode.LoadData(items, 1);
        }

        private static void BuildCodePath(string codePrefix, IEnumerable<OutlineNode.DataItem> items, Dictionary<Guid, string> codePathMap)
        {
            foreach (var item in items)
            {
                item.CodePath = !string.IsNullOrWhiteSpace(codePrefix)
                    ? $"{codePrefix}{item.Code}."
                    : (!string.IsNullOrWhiteSpace(item.Code) ? $"{item.Code}." : null);

                if (!codePathMap.ContainsKey(item.StandardIdentifier))
                    codePathMap.Add(item.StandardIdentifier, item.CodePath);

                if (item.Children.IsNotEmpty())
                    BuildCodePath(item.CodePath, item.Children, codePathMap);
            }
        }

        private IEnumerable<OutlineNode.DataItem> LoadPrimaryHierarchy(Guid standardId, GlossaryHelper glossaryHelper)
        {
            var graph = StandardGraph<StandardGraphNode>.LoadOrganizationEdges(
                Organization.OrganizationIdentifier,
                id => new DataNode(id));

            if (!graph.HasNode(standardId))
                graph.AddNode(new DataNode(standardId));

            var root = (DataNode)graph.GetNode(standardId);

            var nodeFilter = new HashSet<Guid> { standardId };

            FillNodeFilter(root);

            var dataItems = StandardSearch
                .Bind(
                    LinqExtensions1.Expr((Standard x) => OutlineNode.DataItem.Binder.Invoke(x)).Expand(),
                    x => nodeFilter.Contains(x.StandardIdentifier))
                .ToDictionary(x => x.StandardIdentifier);

            PostloadDataItems(standardId, glossaryHelper, dataItems.Keys, dataItems.Values);

            return BuildTree(root).Children;

            void FillNodeFilter(DataNode node)
            {
                foreach (var oNode in node.OutgoingNodes)
                {
                    if (!nodeFilter.Contains(oNode.NodeId))
                        nodeFilter.Add(oNode.NodeId);

                    FillNodeFilter(oNode);
                }
            }

            OutlineNode.DataItem BuildTree(DataNode node)
            {
                var item = dataItems[node.NodeId];

                foreach (var oNode in node.OutgoingNodes)
                    if (dataItems.ContainsKey(oNode.NodeId))
                        item.Children.Add(BuildTree(oNode));

                foreach (var iNode in node.IncomingNodes)
                    if (dataItems.ContainsKey(iNode.NodeId))
                        item.Parents.Add(dataItems[iNode.NodeId]);

                return item;
            }
        }

        private static void LoadCompetencies(Dictionary<Guid, OutlineNode.DataItem> accumulator, IEnumerable<OutlineNode.DataItem> children)
        {
            var keyFilter = children
                .Where(x => x.ParentStandardIdentifier.HasValue && !accumulator.ContainsKey(x.ParentStandardIdentifier.Value))
                .Select(x => x.ParentStandardIdentifier.Value)
                .Distinct()
                .ToArray();

            if (keyFilter.Length == 0)
                return;

            var data = StandardSearch.Bind(
                LinqExtensions1.Expr((Standard x) => OutlineNode.DataItem.Binder.Invoke(x)).Expand(),
                x => keyFilter.Contains(x.StandardIdentifier));

            if (data.Length == 0)
                return;

            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);
        }

        private void PostloadDataItems(Guid rootId, GlossaryHelper glossaryHelper, IEnumerable<Guid> keys, IEnumerable<OutlineNode.DataItem> items)
        {
            var contentBlocks = ServiceLocator.ContentSearch.GetBlocks(keys);

            HashSet<Guid> expandedNodes = null;

            if (AllowSaveState)
                expandedNodes = PersonalizationRepository
                    .GetValue<HashSet<Guid>>(
                        Guid.Empty,
                        CurrentSessionState.Identity.User.UserIdentifier,
                        PersonalizationName.AssetOutlineTreeState + $".{rootId}",
                        false);

            if (expandedNodes == null)
                expandedNodes = new HashSet<Guid>();

            var labels = CurrentSessionState.Identity.Organization.GetStandardContentLabels().NullIfEmpty();

            foreach (var item in items)
            {
                item.IsExpanded = expandedNodes.Contains(item.StandardIdentifier);

                if (!contentBlocks.TryGetValue(item.StandardIdentifier, out var content))
                    continue;

                item.Title = glossaryHelper.Process(
                    item.StandardIdentifier,
                    ContentLabel.Title,
                    content.Title.GetText(Identity.Language));
                item.Contents = new List<OutlineNode.ContentItem>();

                if (labels == null)
                    continue;

                foreach (var label in labels)
                {
                    var trimmedLabel = label.Trim();
                    if (string.Equals(trimmedLabel, "Title", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var contentHtml = content.GetHtml(trimmedLabel, Identity.Language);
                    if (contentHtml.IsEmpty())
                        continue;

                    contentHtml = glossaryHelper.Process(item.StandardIdentifier, ContentLabel.Title, contentHtml);

                    item.Contents.Add(new OutlineNode.ContentItem
                    {
                        Title = LabelHelper.GetTranslation(trimmedLabel),
                        Content = contentHtml
                    });
                }
            }
        }

        #endregion

        #region Data binding (connections)

        private void LoadConnections(IEnumerable<OutlineNode.DataItem> items, Dictionary<Guid, string> codePathMap, GlossaryHelper glossaryHelper)
        {
            foreach (var item in items)
            {
                var connections = StandardConnectionSearch.Bind(
                    x => new OutlineNode.ConnectionInfo
                    {
                        ConnectionType = x.ConnectionType,
                        Identifier = x.ToStandard.StandardIdentifier,
                        Type = x.ToStandard.StandardType,
                        Code = x.ToStandard.Code,
                        Title = x.ToStandard.ContentTitle,
                        Asset = x.ToStandard.AssetNumber
                    },
                    x => x.FromStandardIdentifier == item.StandardIdentifier
                ).ToList();

                var contentBlocks = ServiceLocator.ContentSearch.GetBlocks(
                    connections.Select(x => x.Identifier), labels: new[] { ContentLabel.Title });

                foreach (var connection in connections)
                {
                    if (!codePathMap.TryGetValue(connection.Identifier, out var codePath))
                    {
                        LoadConnectedCodePaths(connection.Identifier, codePathMap, glossaryHelper);
                        codePath = codePathMap[connection.Identifier];
                    }

                    connection.CodePath = codePath;

                    if (contentBlocks.TryGetValue(connection.Identifier, out var content))
                        connection.Title = glossaryHelper.Process(
                            connection.Identifier,
                            ContentLabel.Title,
                            content.Title.GetText(Identity.Language));
                }

                item.Connections = connections;

                if (item.Children.IsNotEmpty())
                    LoadConnections(item.Children, codePathMap, glossaryHelper);
            }
        }

        private void LoadConnectedCodePaths(Guid competencyIdentifier, Dictionary<Guid, string> codePathMap, GlossaryHelper glossaryHelper)
        {
            var frameworkIdentifier = competencyIdentifier;
            var standard = StandardSearch.Select(competencyIdentifier);

            while (standard.ParentStandardIdentifier.HasValue && (standard = StandardSearch.Select(standard.ParentStandardIdentifier.Value)) != null)
                frameworkIdentifier = standard.StandardIdentifier;

            var items = LoadPrimaryHierarchy(frameworkIdentifier, glossaryHelper);

            BuildCodePath(null, items, codePathMap);
        }

        #endregion

        #region Data binding (fulfillment)

        private void CalculateFulfillment(IEnumerable<OutlineNode.DataItem> items)
        {
            var dictionary = new Dictionary<Guid, OutlineNode.DataItem>();
            foreach (var item in items)
            {
                if (dictionary.ContainsKey(item.StandardIdentifier))
                    continue;

                dictionary.Add(item.StandardIdentifier, item);

                var children = item.EnumerateChildrenFlatten();

                foreach (var child in children)
                {
                    if (dictionary.ContainsKey(child.StandardIdentifier))
                        continue;

                    dictionary.Add(child.StandardIdentifier, child);
                }
            }

            SetStatusBasedOnAssessmentAttempts(dictionary);
        }

        private void SetStatusBasedOnAssessmentAttempts(Dictionary<Guid, OutlineNode.DataItem> dictionary)
        {
            SeparateCompetencies(
                ServiceLocator.AttemptSearch.GetAttemptQuestionsByLearner(User.UserIdentifier, new Guid[] { }),
                ServiceLocator.AttemptSearch.GetAttemptOptions(new QAttemptFilter
                {
                    LearnerUserIdentifier = User.UserIdentifier
                }),
                out var correctCompetencies,
                out var incorrectCompetencies
            );

            var competencies = dictionary.Keys.ToList();
            foreach (var competency in competencies)
            {
                var item = dictionary[competency];

                if (correctCompetencies.Contains(competency))
                    item.SatisfactionStatus = SatisfactionStatus.SatisfiedDirectly;

                else if (item.SatisfactionStatus == SatisfactionStatus.NotCompleted && incorrectCompetencies.Contains(competency))
                    item.SatisfactionStatus = SatisfactionStatus.NotSatisfied;
            }
        }

        private void SeparateCompetencies(IEnumerable<AnswerState> questions, IEnumerable<QAttemptOption> options, out HashSet<Guid> correctCompetencies, out HashSet<Guid> incorrectCompetencies)
        {
            var subCompetencies = ServiceLocator.BankSearch
                .GetQuestions(questions.Select(x => x.Question).Distinct().ToArray(), x => x.SubCompetencies)
                .Where(x => x.SubCompetencies.IsNotEmpty())
                .ToDictionary(
                    x => x.QuestionIdentifier,
                    x => x.SubCompetencies.Select(y => y.SubCompetencyIdentifier).ToArray());

            correctCompetencies = new HashSet<Guid>();
            incorrectCompetencies = new HashSet<Guid>();

            foreach (var q in questions)
            {
                var isCorrect = q.AnswerPoints >= q.QuestionPoints;

                if (q.Competency.HasValue)
                {
                    if (isCorrect)
                        correctCompetencies.Add(q.Competency.Value);
                    else
                        incorrectCompetencies.Add(q.Competency.Value);
                }

                if (isCorrect)
                    TryAddSubCompetencies(q.Question, correctCompetencies);
                else
                    TryAddSubCompetencies(q.Question, incorrectCompetencies);
            }

            foreach (var o in options)
            {
                if (!o.CompetencyItemIdentifier.HasValue)
                    continue;

                if (o.OptionIsTrue.HasValue)
                {
                    if (o.AnswerIsSelected.HasValue)
                    {
                        if (o.OptionIsTrue.Value == o.AnswerIsSelected.Value)
                            correctCompetencies.Add(o.CompetencyItemIdentifier.Value);
                        else
                            incorrectCompetencies.Add(o.CompetencyItemIdentifier.Value);
                    }
                }
                else if (o.AnswerIsSelected == true)
                {
                    if (o.OptionPoints > 0)
                        correctCompetencies.Add(o.CompetencyItemIdentifier.Value);
                    else
                        incorrectCompetencies.Add(o.CompetencyItemIdentifier.Value);
                }
            }

            void TryAddSubCompetencies(Guid questionId, HashSet<Guid> collection)
            {
                if (!subCompetencies.TryGetValue(questionId, out var data))
                    return;

                foreach (var id in data)
                    collection.Add(id);
            }
        }

        #endregion
    }
}