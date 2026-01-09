using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class RelationshipList : UserControl
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;

            DeleteRelationshipButton.Click += DeleteRelationshipButton_Click;

            GraphRepeater.DataBinding += GraphRepeater_DataBinding;
            GraphRepeater.ItemDataBound += GraphRepeater_ItemDataBound;

            UpdatePanel.Request += UpdatePanel_Request;

            ReorderButton.Click += ReorderButton_Click;

            HeaderContent.ContentKey = typeof(RelationshipList).FullName;
            FooterScript.ContentKey = typeof(RelationshipList).FullName;
        }

        #endregion

        #region Helper methods

        private GraphModel[] GetRelations(Guid standardKey, ConnectionDirection direction)
        {
            return IsContainment ? GetContainments(standardKey, direction) : GetConnections(standardKey, direction);
        }

        private static GraphModel[] GetContainments(Guid standardKey, ConnectionDirection direction)
        {
            var standard = StandardSearch.Select(standardKey);

            StandardContainmentSummary[] edges;

            if (direction == ConnectionDirection.Incoming)
                edges = StandardContainmentSearch.SelectByChildStandardIdentifier(standard.StandardIdentifier);
            else if (direction == ConnectionDirection.Outgoing)
                edges = StandardContainmentSearch.SelectByParentStandardIdentifier(standard.StandardIdentifier);
            else
                throw new NotImplementedException();

            var graphs = new List<GraphModel>();
            foreach (var edge in edges)
            {
                if (graphs.Count == 0)
                    graphs.Add(new GraphModel(standard.ContentTitle, null, direction));

                var graph = graphs[0];

                var model = new EdgeModel
                {
                    EdgeFromID = edge.ParentStandardIdentifier,
                    EdgeToID = edge.ChildStandardIdentifier,
                    EdgeSequence = edge.ChildSequence,
                    IsEdge = !edge.ParentIsPrimaryContainer,
                };

                if (direction == ConnectionDirection.Incoming)
                {
                    model.StandardIdentifier = edge.ParentStandardIdentifier;
                    model.StandardSequence = edge.ParentSequence;
                    model.StandardTitle = edge.ParentTitle;
                    model.AssetNumber = edge.ParentAssetNumber;
                    model.StandardIdentifier = edge.ParentStandardIdentifier;
                    model.StandardType = edge.ParentStandardType;
                    model.StandardOrganizationIdentifier = edge.ParentOrganizationIdentifier;
                }
                else
                {
                    model.StandardIdentifier = edge.ChildStandardIdentifier;
                    model.StandardSequence = edge.ParentSequence;
                    model.StandardTitle = edge.ChildTitle;
                    model.AssetNumber = edge.ChildAssetNumber;
                    model.StandardIdentifier = edge.ChildStandardIdentifier;
                    model.StandardType = edge.ChildStandardType;
                    model.StandardOrganizationIdentifier = edge.ChildOrganizationIdentifier;
                }

                graph.AddEdge(model);
            }

            return graphs.Where(x => x.Edges.Count > 0).OrderBy(x => x.ConnectionType).ToArray();
        }

        private static GraphModel[] GetConnections(Guid standardKey, ConnectionDirection direction)
        {
            var standard = StandardSearch.Select(standardKey);

            IReadOnlyList<StandardConnection> edges;

            if (direction == ConnectionDirection.Incoming)
                edges = StandardConnectionSearch.Select(x => x.ToStandardIdentifier == standardKey, x => x.FromStandard);
            else if (direction == ConnectionDirection.Outgoing)
                edges = StandardConnectionSearch.Select(x => x.FromStandardIdentifier == standardKey, x => x.ToStandard);
            else
                throw new NotImplementedException();

            var graphs = new Dictionary<string, GraphModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var edge in edges)
            {
                if (!graphs.TryGetValue(edge.ConnectionType, out var graph))
                    graphs.Add(edge.ConnectionType, graph = new GraphModel(standard.ContentTitle, edge.ConnectionType, direction));

                var model = new EdgeModel
                {
                    EdgeFromID = edge.FromStandardIdentifier,
                    EdgeToID = edge.ToStandardIdentifier,
                    EdgeSequence = 0,
                    IsEdge = true,
                    ConnectionType = edge.ConnectionType
                };

                if (direction == ConnectionDirection.Incoming)
                {
                    model.StandardIdentifier = edge.FromStandardIdentifier;
                    model.StandardSequence = 0;
                    model.StandardTitle = edge.FromStandard.ContentTitle;
                    model.AssetNumber = edge.FromStandard.AssetNumber;
                    model.StandardIdentifier = edge.FromStandard.StandardIdentifier;
                    model.StandardType = edge.FromStandard.StandardType;
                    model.StandardOrganizationIdentifier = edge.FromStandard.OrganizationIdentifier;
                }
                else
                {
                    model.StandardIdentifier = edge.ToStandardIdentifier;
                    model.StandardSequence = 0;
                    model.StandardTitle = edge.ToStandard.ContentTitle;
                    model.AssetNumber = edge.ToStandard.AssetNumber;
                    model.StandardIdentifier = edge.ToStandard.StandardIdentifier;
                    model.StandardType = edge.ToStandard.StandardType;
                    model.StandardOrganizationIdentifier = edge.ToStandard.OrganizationIdentifier;
                }

                graph.AddEdge(model);
            }

            return graphs.Values.Where(x => x.Edges.Count > 0).OrderBy(x => x.ConnectionType).ToArray();
        }

        private void SaveReorder(EdgeModel[][] orderedArray)
        {
            foreach (var graph in orderedArray)
            {
                var sequence = 0;

                foreach (var graphEdge in graph)
                {
                    if (graphEdge.IsEdge)
                    {
                        StandardContainmentStore.Update(graphEdge.EdgeFromID, graphEdge.EdgeToID, e =>
                        {
                            e.ChildSequence = ++sequence;
                        });
                    }
                    else
                    {
                        StandardStore.Update(graphEdge.EdgeToID, e =>
                        {
                            e.Sequence = ++sequence;
                        });
                    }
                }
            }
        }

        private void DeleteRelations(IEnumerable<Tuple<Guid, Guid>> relations, List<Guid> children)
        {
            if (IsContainment)
            {
                foreach (var relation in relations)
                    StandardContainmentStore.Delete(relation.Item1, relation.Item2);
            }
            else
            {
                foreach (var relation in relations)
                    StandardConnectionStore.Delete(relation.Item1, relation.Item2);
            }

            foreach (var id in children)
                StandardStore.Update(id, x => x.ParentStandardIdentifier = null);
        }

        #endregion

        #region Classes

        protected class GraphModel
        {
            #region Construction

            public GraphModel(string standardTitle, string connectionType, ConnectionDirection direction)
            {
                StandardTitle = standardTitle;
                ConnectionType = connectionType;

                _direction = direction;
                _edges = new List<EdgeModel>();
                _sortedArray = null;
            }

            #endregion

            #region Overriden methods

            public override string ToString()
            {
                return GetDescription();
            }

            #endregion

            #region Properties

            public string GraphName
            {
                get
                {
                    if (string.IsNullOrEmpty(ConnectionType))
                        return _direction == ConnectionDirection.Incoming ? "Parents" : "Children";

                    return _direction == ConnectionDirection.Incoming ? "From" : "To";
                }
            }

            public string GraphDescription => GetDescription();
            public string StandardTitle { get; set; }
            public string ConnectionType { get; }

            public IReadOnlyList<EdgeModel> Edges => _sortedArray
                                                     ?? (_sortedArray = _edges.OrderBy(x => x.StandardSequence)
                                                         .ThenBy(x => x.EdgeSequence).ThenBy(x => x.StandardType)
                                                         .ThenBy(x => x.AssetNumber).ToArray());

            #endregion

            #region Fields

            private readonly ConnectionDirection _direction;
            private readonly List<EdgeModel> _edges;
            private EdgeModel[] _sortedArray;

            #endregion

            #region Public methods

            internal void AddEdge(EdgeModel edgeModel)
            {
                _edges.Add(edgeModel);
            }

            #endregion

            #region Helper methods

            private string GetDescription()
            {
                if (string.IsNullOrEmpty(ConnectionType))
                {
                    return _direction == ConnectionDirection.Incoming
                        ? $"Standards that <strong>contain</strong> {StandardTitle}"
                        : $"Standards <strong>contained by</strong> {StandardTitle}";
                }

                return _direction == ConnectionDirection.Incoming
                    ? $"Standards that <strong>{ConnectionType.Singularize().ToLower()}</strong> {StandardTitle}"
                    : $"Standards <strong>{ReverseDescriptor()}</strong> {StandardTitle}";
            }

            private string ReverseDescriptor()
            {
                if (ConnectionType.Equals("References", StringComparison.OrdinalIgnoreCase))
                    return "is referenced by";

                if (ConnectionType.Equals("Resembles", StringComparison.OrdinalIgnoreCase))
                    return "is similar to";

                if (ConnectionType.Equals("Satisfies", StringComparison.OrdinalIgnoreCase))
                    return "is satisfied by";

                if (ConnectionType.Equals("Uses", StringComparison.OrdinalIgnoreCase))
                    return "is used by";

                throw new NotImplementedException($"Support for descriptor is not implemented: {ConnectionType}");
            }

            #endregion
        }

        protected class EdgeModel
        {
            public Guid EdgeFromID { get; set; }
            public Guid EdgeToID { get; set; }
            public int EdgeSequence { get; set; }
            public string EdgeCondition { get; set; }
            public bool IsEdge { get; set; }

            public Guid StandardIdentifier { get; set; }
            public int StandardSequence { get; set; }
            public string StandardTitle { get; set; }
            public int AssetNumber { get; set; }
            public string StandardType { get; set; }
            public Guid StandardOrganizationIdentifier { get; set; }

            public string ConnectionType { get; set; }
        }

        #endregion

        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        private bool IsContainment
        {
            get => (bool)ViewState[nameof(IsContainment)];
            set => ViewState[nameof(IsContainment)] = value;
        }

        protected Guid AssetID
        {
            get => (Guid)ViewState[nameof(AssetID)];
            set => ViewState[nameof(AssetID)] = value;
        }

        protected int AssetNumber
        {
            get => (int)ViewState[nameof(AssetNumber)];
            set => ViewState[nameof(AssetNumber)] = value;
        }

        protected ConnectionDirection EdgeDirection
        {
            get => (ConnectionDirection)ViewState[nameof(EdgeDirection)];
            set => ViewState[nameof(EdgeDirection)] = value;
        }

        public int ItemCount
        {
            get => (int)(ViewState[nameof(ItemCount)] ?? 0);
            set => ViewState[nameof(ItemCount)] = value;
        }

        public int EdgeCount
        {
            get => (int?)ViewState[nameof(EdgeCount)] ?? 0;
            private set => ViewState[nameof(EdgeCount)] = value;
        }

        public int NotEdgeCount
        {
            get => (int?)ViewState[nameof(NotEdgeCount)] ?? 0;
            private set => ViewState[nameof(NotEdgeCount)] = value;
        }

        public bool EnableAJAX
        {
            get => (bool)(ViewState[nameof(EnableAJAX)] ?? true);
            set => ViewState[nameof(EnableAJAX)] = value;
        }

        #endregion

        #region Loading

        public void Refresh()
        {
            GraphRepeater.DataBind();
        }

        public void LoadData(Standard asset, bool isContainment, ConnectionDirection direction, bool canEdit)
        {
            IsContainment = isContainment;
            AssetID = asset.StandardIdentifier;
            AssetNumber = asset.AssetNumber;
            EdgeDirection = direction;

            GraphRepeater.DataBind();

            // CommandButtons.Visible = direction == RelationshipDirection.Outgoing;
            CommandButtons2.Visible = canEdit && direction == ConnectionDirection.Outgoing;
            ReorderCommandButtons.Visible = canEdit && direction == ConnectionDirection.Outgoing;
            ReorderButton.Visible = IsContainment;

            // AddButton.OnClientClick = $"assetGraphList.showCreator({asset.AssetID}, '{CreatorWindow.ClientID}', '{RefreshButton.UniqueID}'); return false;";
            SelectAllButton.OnClientClick = $"$('#{UnselectAllButton.ClientID}').css('display', ''); $('#{SelectAllButton.ClientID}').css('display', 'none'); return setCheckboxes('{SectionControl.ClientID}', true);";
            UnselectAllButton.OnClientClick = $"$('#{SelectAllButton.ClientID}').css('display', ''); $('#{UnselectAllButton.ClientID}').css('display', 'none'); return setCheckboxes('{SectionControl.ClientID}', false);";
            ReorderButton.OnClientClick = $"inSite.common.gridReorderHelper.startReorder('{UniqueID}'); return false;";
            CreateRelationshipButton.OnClientClick = $"assetGraphList.showCreator('{asset.StandardIdentifier}', '{(IsContainment ? "containment" : "connection")}', '{CreatorWindow.ClientID}', '{RefreshButton.UniqueID}'); return false;";
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdatePanel.ChildrenAsTriggers = EnableAJAX;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(RelationshipList),
                $"register_reorder_{UniqueID}",
                $@"
inSite.common.gridReorderHelper.registerReorder({{
    id:'{UniqueID}',
    selector:'#{SectionControl.ClientID} table.asset-edge-list',
    items:'tbody > tr',
    {(EnableAJAX ? $"updatePanelId:'{UpdatePanel.ClientID}'" : $"callbackControlId:'{ReorderButton.UniqueID}'")},
}});", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void ReorderButton_Click(object sender, EventArgs e)
        {
            OnReorderCommand(Request.Form["__EVENTARGUMENT"]);
        }

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            OnReorderCommand(e.Value);
        }

        private void OnReorderCommand(string argument)
        {
            if (string.IsNullOrEmpty(argument) || EdgeDirection != ConnectionDirection.Outgoing)
                return;

            var args = argument.Split('&');
            var command = args[0];
            var value = args.Length > 1 ? args[1] : null;

            if (command == "cancel-reorder")
            {
            }
            else if (command == "save-reorder")
            {
                var changes = JavaScriptHelper.GridReorder.Parse(value);
                var sourceArray = GetRelations(AssetID, EdgeDirection);
                var orderedArray = new EdgeModel[sourceArray.Length][];
                var isStateChanged = false;

                for (var x = 0; x < sourceArray.Length; x++)
                {
                    var edges = sourceArray[x].Edges.ToArray();

                    for (var y = 0; y < edges.Length; y++)
                    {
                        var edge = edges[y];
                        if (x < GraphRepeater.Items.Count)
                        {
                            var edgeRepeater = (Repeater)GraphRepeater.Items[x].FindControl("EdgeRepeater");
                            if (y < edgeRepeater.Items.Count)
                            {
                                var edgeItem = edgeRepeater.Items[y];

                                var fromIdLiteral = (ITextControl)edgeItem.FindControl("FromID");
                                var toIdLiteral = (ITextControl)edgeItem.FindControl("ToID");

                                var fromId = Guid.Parse(fromIdLiteral.Text);
                                var toId = Guid.Parse(toIdLiteral.Text);

                                if (edge.EdgeFromID != fromId || edge.EdgeToID != toId)
                                    isStateChanged = true;
                            }
                            else
                            {
                                isStateChanged = true;
                            }
                        }
                        else
                        {
                            isStateChanged = true;
                        }

                        if (isStateChanged)
                            break;
                    }

                    if (isStateChanged)
                        break;

                    orderedArray[x] = edges;
                }

                if (isStateChanged)
                    throw new ApplicationException("The state was changed between postbacks");

                foreach (var move in changes)
                {
                    if (move.Source.ContainerIndex != move.Destination.ContainerIndex)
                        throw new ApplicationException("Moving of edges between graphs is not allowed");

                    orderedArray[move.Destination.ContainerIndex][move.Destination.ItemIndex] =
                        sourceArray[move.Source.ContainerIndex].Edges[move.Source.ItemIndex];
                }

                SaveReorder(orderedArray);

                GraphRepeater.DataBind();
            }

            OnRefreshed();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            GraphRepeater.DataBind();
            OnRefreshed();
        }

        private void DeleteRelationshipButton_Click(object sender, EventArgs e)
        {
            if (EdgeDirection != ConnectionDirection.Outgoing)
                return;

            var relations = new List<Tuple<Guid, Guid>>();
            var children = new List<Guid>();

            foreach (RepeaterItem graphItem in GraphRepeater.Items)
            {
                var edgeRepeater = (Repeater)graphItem.FindControl("EdgeRepeater");

                foreach (RepeaterItem edgeItem in edgeRepeater.Items)
                {
                    var isSelected = edgeItem.FindControl("IsSelected");
                    if (!isSelected.Visible || !((ICheckBoxControl)isSelected).Checked)
                        continue;

                    var fromIdLiteral = (ITextControl)edgeItem.FindControl("FromID");
                    var toIdLiteral = (ITextControl)edgeItem.FindControl("ToID");
                    var isEdgeLiteral = (ITextControl)edgeItem.FindControl("IsEdge");

                    var fromId = Guid.Parse(fromIdLiteral.Text);
                    var toId = Guid.Parse(toIdLiteral.Text);
                    var isEdge = bool.Parse(isEdgeLiteral.Text);

                    if (isEdge)
                        relations.Add(new Tuple<Guid, Guid>(fromId, toId));
                    else
                        children.Add(toId);
                }
            }

            DeleteRelations(relations, children);

            GraphRepeater.DataBind();

            OnRefreshed();
        }

        private void GraphRepeater_DataBinding(object sender, EventArgs e)
        {
            var data = GetRelations(AssetID, EdgeDirection);

            GraphRepeater.DataSource = data;
            ItemCount = data.Sum(x => x.Edges.Count);

            EdgeCount = data.Sum(x => x.Edges.Count(y => y.IsEdge));
            NotEdgeCount = data.Sum(x => x.Edges.Count(y => !y.IsEdge));
        }

        private void GraphRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var graph = (GraphModel)e.Item.DataItem;

            var edgeRepeater = (Repeater)e.Item.FindControl("EdgeRepeater");
            edgeRepeater.DataSource = graph.Edges;
            edgeRepeater.DataBind();
        }

        #endregion
    }
}