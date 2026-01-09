using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Graphs;
using Shift.Constant;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class OutlineTreeNodeRepeater : BaseUserControl
    {
        #region Classes

        public class DataItem : GraphNodeModel, IStandardGraphNode<DataItem>
        {
            #region Properties

            public override Guid NodeId { get; set; }
            public int Sequence { get; internal set; }
            public string Code { get; internal set; }
            public string Title { get; internal set; }
            public string Summary { get; internal set; }
            public int Number { get; internal set; }
            public Guid Identifier { get; internal set; }
            public string Icon { get; internal set; }
            public bool IsTheory { get; internal set; }
            public bool IsPractical { get; internal set; }

            public string Type { get; internal set; }
            public string TypeName { get; internal set; }
            public string TypeIcon { get; internal set; }

            public string Label { get; internal set; }
            public string Tags { get; internal set; }
            public string Status { get; internal set; }

            public IReadOnlyCollection<StandardGraphEdge<DataItem>> IncomingEdges => _graph.GetIncomingContainments(NodeId);
            public IReadOnlyCollection<StandardGraphEdge<DataItem>> OutgoingEdges => _graph.GetOutgoingContainments(NodeId);

            public override bool HasParent => _graph.GetIncomingContainments(NodeId).Any();
            public override bool HasChildren => _graph.GetOutgoingContainments(NodeId).Any();

            #endregion

            #region Fields

            private StandardGraph<DataItem> _graph = null;

            #endregion

            #region Construction

            public DataItem()
            {
            }

            public DataItem(Guid id)
            {
                NodeId = id;
            }

            #endregion

            #region Overriden methods

            protected override bool OnGraphAttach(IGraph graph)
            {
                _graph = (StandardGraph<DataItem>)graph;

                return true;
            }

            protected override bool OnGraphDetach()
            {
                _graph = null;

                return true;
            }

            #endregion
        }

        public class TriggerInfo
        {
            public string Title { get; set; }
            public int Asset { get; set; }
        }

        private class EdgeInfo
        {
            public string Label { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
        }

        #endregion

        #region Properties and fields

        private List<TCollectionItem> _items;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;

            BindTags(e.Item, dataItem.Tags);

            if (dataItem.OutgoingEdges.Count > 0)
            {
                var childContainer = (DynamicControl)e.Item.FindControl("Container");
                var childNode = (OutlineTreeNodeRepeater)childContainer.LoadControl("~/UI/Admin/Standards/Standards/Controls/OutlineTreeNodeRepeater.ascx");
                childNode.LoadData(dataItem.OutgoingEdges.Select(x => x.ToNode).OrderBy(x => x.Sequence));
            }
        }

        private void BindTags(RepeaterItem item, string tagsJson)
        {
            if (string.IsNullOrEmpty(tagsJson))
                return;

            var tags = new HashSet<string>();
            var collections = JsonConvert.DeserializeObject<List<Tuple<string, List<string>>>>(tagsJson);

            foreach (var collection in collections)
            {
                foreach (var collectionTag in collection.Item2)
                    tags.Add(collectionTag);
            }

            var tagItems = tags
                .Select(tag => new
                {
                    Tag = tag,
                    ColorClass = GetTagColorClass(tag)
                })
                .OrderBy(x => x.Tag)
                .ToList();

            var tagRepeater = (Repeater)item.FindControl("TagRepeater");
            tagRepeater.DataSource = tagItems;
            tagRepeater.DataBind();
        }

        private string GetTagColorClass(string tag)
        {
            if (_items == null)
                _items = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    CollectionName = CollectionName.Standards_Organizations_Classification_Flag
                });

            var item = _items.Find(x => string.Equals(x.ItemName, tag, StringComparison.OrdinalIgnoreCase));

            var name = item?.ItemColor?.ToEnumNullable<Indicator>()?.GetContextualClass()
                ?? Indicator.Info.GetContextualClass();

            return $"badge bg-{name}";
        }

        #endregion

        #region Methods (data loading)

        public void LoadData(IEnumerable<DataItem> items)
        {
            Repeater.DataSource = items;
            Repeater.DataBind();
        }

        public static void LoadDataItems(Guid rootId, StandardGraph<DataItem> graph)
        {
            var nodeFilter = new HashSet<Guid> { rootId };

            FillNodeFilter(graph.GetNode(rootId));

            {
                var nodes = StandardSearch.Bind(x => new DataItem
                {
                    NodeId = x.StandardIdentifier,
                    Identifier = x.StandardIdentifier,
                    Sequence = x.Sequence,
                    Code = x.Code,
                    Type = x.StandardType,
                    Label = x.StandardLabel,
                    Number = x.AssetNumber,
                    IsTheory = x.IsTheory,
                    IsPractical = x.IsPractical,
                    Tags = x.Tags,
                    Title = x.ContentName,
                    Status = x.StandardStatus
                }, x => nodeFilter.Contains(x.StandardIdentifier), null, null);

                foreach (var node in nodes)
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(
                        node.Identifier,
                        ContentContainer.DefaultLanguage,
                        new[] { ContentLabel.Title, ContentLabel.Summary });

                    node.Title = content.Title.Text.Default ?? node.Title;
                    node.Summary = content.Summary.Text.Default;
                }

                var classifications = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier);

                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];

                    var classification = classifications.FirstOrDefault(x => StringHelper.Equals(x, node.Type));
                    if (classification == null)
                        classification = "Unknown";

                    node.Type = classification;
                    node.TypeName = node.Label.IfNullOrEmpty(classification);
                    node.TypeIcon = StandardSearch.GetStandardTypeIcon(classification);

                    graph.ReplaceNode(node);
                }
            }

            var rootNode = graph.GetNode(rootId);

            if (!string.IsNullOrEmpty(rootNode.Code))
                rootNode.Code = rootNode.Code + ".";

            AssignCode(rootNode, rootNode.Code);

            void FillNodeFilter(DataItem node)
            {
                foreach (var edge in node.IncomingEdges)
                {
                    if (!nodeFilter.Contains(edge.FromNodeId))
                        nodeFilter.Add(edge.FromNodeId);
                }

                foreach (var edge in node.OutgoingEdges)
                {
                    if (!nodeFilter.Contains(edge.ToNodeId))
                        nodeFilter.Add(edge.ToNodeId);

                    FillNodeFilter(edge.ToNode);
                }
            }

            void AssignCode(DataItem node, string codePath)
            {
                foreach (var edge in node.OutgoingEdges)
                {
                    var toNode = edge.ToNode;

                    if (!string.IsNullOrEmpty(toNode.Code))
                    {
                        toNode.Code = codePath + toNode.Code + ".";

                        AssignCode(toNode, toNode.Code);
                    }
                    else
                        AssignCode(toNode, codePath);
                }
            }
        }

        #endregion
    }
}