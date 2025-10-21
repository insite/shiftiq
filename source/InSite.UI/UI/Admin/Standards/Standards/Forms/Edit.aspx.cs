using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Standards.Standards.Utilities;
using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/standards/standards/search";

        private Guid Identifier
        {
            get
            {
                if (Guid.TryParse(Request.QueryString["id"], out var id))
                    return id;
                else
                {
                    HttpResponseHelper.Redirect(SearchUrl);
                    return Guid.Empty;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            ValidateUrl();

            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;
            SaveButton.Click += SaveButton_Click;

            Detail.Alert += (x, y) => EditorStatus.AddMessage(y);
        }

        public override void ApplyAccessControl()
        {
            if (!CanEdit)
                SaveButton.Visible = false;

            base.ApplyAccessControl();
        }

        private void ValidateUrl()
        {
            var url = HttpRequestHelper.GetCurrentWebUrl();

            var assetNumber = ValueConverter.ToInt32Nullable(url.QueryString["number"]);

            if (assetNumber != null)
            {
                var assetIdentifier = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.AssetNumber == assetNumber && x.OrganizationIdentifier == Organization.OrganizationIdentifier);

                if (assetIdentifier == Guid.Empty)
                {
                    HttpResponseHelper.Redirect(SearchUrl);
                }
                else
                {
                    url.QueryString.Remove("number");
                    url.QueryString.Add("asset", assetIdentifier.ToString());
                    HttpResponseHelper.Redirect(url);
                }
            }

            var assetKey = ValueConverter.ToGuidNullable(url.QueryString["id"]);

            if (assetKey != null)
            {
                var assetIdentifier = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == assetKey);

                if (assetIdentifier == Guid.Empty)
                {
                    HttpResponseHelper.Redirect(SearchUrl);
                }
            }
        }

        private class AssetItem
        {
            public Guid ID { get; set; }
            public int Number { get; set; }
            public Guid Thumbprint { get; set; }
            public string Text { get; set; }
            public string Icon { get; set; }
        }

        private class AssetTreeInfo
        {
            public Guid ChildStandardIdentifier { get; set; }
        }

        private bool GraphHasDependencyCycle
        {
            get => ViewState[nameof(GraphHasDependencyCycle)] != null &&
                   (bool)ViewState[nameof(GraphHasDependencyCycle)];
            set => ViewState[nameof(GraphHasDependencyCycle)] = value;
        }

        private bool GraphContainsMutiplePaths
        {
            get => ViewState[nameof(GraphContainsMutiplePaths)] != null &&
                   (bool)ViewState[nameof(GraphContainsMutiplePaths)];
            set => ViewState[nameof(GraphContainsMutiplePaths)] = value;
        }

        protected Standard Entity
        {
            get
            {
                if (_isEntityLoaded)
                    return _entity;

                _entity = StandardSearch.SelectFirst(x => x.StandardIdentifier == Identifier, x => x.Organization, x => x.Children, x => x.Parent);
                _isEntityLoaded = true;

                return _entity;
            }
        }

        private StandardGraph<StandardGraphNode> Graph =>
            _graph ?? (_graph = StandardGraph<StandardGraphNode>.LoadOrganizationEdges(Organization.OrganizationIdentifier, id => new StandardGraphNode(id)));

        private Standard _entity;
        private bool _isEntityLoaded;
        private StandardGraph<StandardGraphNode> _graph;

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (Entity == null || Entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            try
            {
                Save();
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                {
                    EditorStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                    return;
                }

                throw;
            }

            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void Open()
        {
            _isEntityLoaded = false;

            if (Entity == null || Entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            var rootKeys = new List<Guid>();
            {
                var assetNode = Graph.GetNode(Entity.StandardIdentifier);
                var rootNodes = assetNode?.GetRootNodes();
                if (rootNodes != null)
                {
                    foreach (var rootNode in rootNodes)
                    {
                        rootKeys.Add(rootNode.NodeId);

                        ValidateGraph(rootNode);
                    }
                }

                if (rootKeys.Count == 0)
                    rootKeys.Add(Entity.StandardIdentifier);
            }

            PageHelper.AutoBindHeader(this, null, GetFormTitle(Entity));

            if (Detail.OutgoingContainmentEdgeCount > 0 && Detail.OutgoingContainmentNotEdgeCount > 0)
                EditorStatus.AddMessage(
                    AlertType.Warning,
                    $"Asset #{Entity.AssetNumber} does not use the same type of relationship for all its downstream containment relationships: " +
                    $"{Detail.OutgoingContainmentEdgeCount} are from/to relationships and {Detail.OutgoingContainmentNotEdgeCount} is a parent/child relationship. " +
                    $"Mixing containment relationship types is not recommended.");

            string tab = null, panel = null;

            if (!IsPostBack)
            {
                tab = Request["tab"]?.ToLower();
                panel = Request["panel"]?.ToLower();
            }

            var showOutlineLink = !GraphHasDependencyCycle
                && !GraphContainsMutiplePaths
                && Identity.IsActionAuthorized("ui/admin/standards/manage");
            var rootId = StandardSearch.BindFirst(
                x => (Guid?)x.StandardIdentifier,
                x => rootKeys.Contains(x.StandardIdentifier)
                  && x.StandardType != Shift.Constant.StandardType.Document,
                null, "AssetNumber");

            Detail.SetInputValues(Entity, tab, panel, showOutlineLink ? rootId : null, CanEdit, CanDelete);
        }

        private bool Save()
        {
            var standard = ServiceLocator.StandardSearch.GetStandard(Identifier);

            var beforeParentId = standard.ParentStandardIdentifier;

            Detail.GetInputValues(standard);

            var isParentChanged = standard.ParentStandardIdentifier != beforeParentId;
            if (isParentChanged && !ValidateNewParent(standard))
                return false;

            StandardStore.Update(standard);

            if (isParentChanged && beforeParentId.HasValue)
                StandardStore.Resequence(beforeParentId.Value);

            return true;
        }

        private bool ValidateNewParent(QStandard standard)
        {
            if (!standard.ParentStandardIdentifier.HasValue)
            {
                standard.Sequence = 1;
                return true;
            }

            if (standard.ParentStandardIdentifier.Value == standard.StandardIdentifier)
            {
                EditorStatus.AddMessage(AlertType.Error,
                    "The system doesn't allow to create a self-reference relationship.");
                return false;
            }

            if (StandardContainmentSearch.Exists(x => x.ParentStandardIdentifier == standard.ParentStandardIdentifier.Value
                                                        && x.ChildStandardIdentifier == standard.StandardIdentifier))
            {
                EditorStatus.AddMessage(AlertType.Error,
                    "The selected parent has already contained current asset.");
                return false;
            }

            var edges = StandardContainmentSearch.SelectTree(new[] { standard.StandardIdentifier });
            if (edges.Any(x => x.ChildStandardIdentifier == standard.ParentStandardIdentifier))
            {
                EditorStatus.AddMessage(AlertType.Error,
                    "The system doesn't allow to select a direct/indirect child as asset parent.");
                return false;
            }

            standard.Sequence = StandardSearch.SelectNextSequence(standard.ParentStandardIdentifier.Value);

            return true;
        }

        private void ValidateGraph(StandardGraphNode root)
        {
            GraphHasDependencyCycle = false;
            GraphContainsMutiplePaths = false;

            if (root != null)
            {
                var cyclePaths = root.FindCycles();
                if (cyclePaths.Length > 0)
                {
                    var errorMessage = StandardGraphHelper.BuildDependencyCycleHtmlErrorMessage(root.NodeId, cyclePaths);
                    EditorStatus.AddMessage(AlertType.Error, errorMessage);

                    GraphHasDependencyCycle = true;
                }
            }
        }

        private static string GetFormTitle(Standard entity)
        {
            return StringHelper.FirstValue(entity.ContentTitle, entity.ContentName, "Untitled")
                + $" <span class='form-text'>{entity.StandardType}</span>"
                + OutlineHelper.GetStatusBadgeHtml(entity.StandardStatus, "fs-xs ms-1");
        }
    }
}
