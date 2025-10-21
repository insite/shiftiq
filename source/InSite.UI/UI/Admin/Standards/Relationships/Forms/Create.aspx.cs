using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Admin.Standards.Relationships.Forms
{
    public partial class Create : AdminBasePage
    {
        #region Helper methods: resources

        private void AddAssetEdge(IEnumerable<Standard> assets)
        {
            if (EdgeType == "connection")
                AddConnections(assets);
            else
                AddContainments(assets);
        }

        private void AddContainments(IEnumerable<Standard> assets)
        {
            if (Standard == null)
                return;

            var exclusions = new HashSet<Guid> { Standard.StandardIdentifier };

            var parentEdges = StandardContainmentSearch.SelectUpstreamRelationships(new[] { Standard.StandardIdentifier });
            foreach (var parentEdge in parentEdges)
                exclusions.Add(parentEdge.ParentStandardIdentifier);

            var childrenEdges = StandardContainmentSearch.SelectTree(new[] { Standard.StandardIdentifier });
            foreach (var childEdge in childrenEdges)
            {
                if (!exclusions.Contains(childEdge.ChildStandardIdentifier))
                    exclusions.Add(childEdge.ChildStandardIdentifier);
            }

            var sequence = StandardContainmentSearch.SelectMaxSequence(Standard.StandardIdentifier);
            var edges = new List<StandardContainment>();

            foreach (var child in assets)
            {
                if (childrenEdges.Any(x => x.ChildStandardIdentifier == child.StandardIdentifier))
                    throw new AddAssetEdgeException($"You cannot add the relationship \"#{Standard.AssetNumber} contains #{child.AssetNumber}\" because it's already added there.");

                if (exclusions.Contains(child.StandardIdentifier))
                    throw new AddAssetEdgeException($"You cannot add the relationship \"#{Standard.AssetNumber} contains #{child.AssetNumber}\" because doing so would create a dependency cycle in your hierarchy.");

                var edge = StandardContainmentSearch.SelectFirst(x => x.ParentStandardIdentifier == Standard.StandardIdentifier && x.ChildStandardIdentifier == child.StandardIdentifier);

                if (edge == null)
                {
                    edges.Add(edge = new StandardContainment
                    {
                        ParentStandardIdentifier = Standard.StandardIdentifier,
                        ChildStandardIdentifier = child.StandardIdentifier,
                        ChildSequence = ++sequence
                    });
                }
            }

            StandardContainmentStore.Insert(edges);
        }

        private void AddConnections(IEnumerable<Standard> assets)
        {
            if (Standard == null)
                return;

            var existUpstreams = StandardConnectionSearch.SelectUpstream(new[] { Standard.StandardIdentifier });
            var existDownstreams = StandardConnectionSearch.SelectDownstream(new[] { Standard.StandardIdentifier });

            var edges = new List<StandardConnection>();

            foreach (var child in assets)
            {
                var existDownstream = existDownstreams.Where(x => x.ToStandardIdentifier == child.StandardIdentifier).FirstOrDefault();
                if (existDownstream != null && existDownstream.FromStandardIdentifier == Standard.StandardIdentifier)
                    throw new AddAssetEdgeException($"You cannot add the connection to #{child.AssetNumber} because #{Standard.AssetNumber} already {existDownstream.ConnectionType.ToLower()} #{child.AssetNumber}.");

                if (existUpstreams.Any(x => x.FromStandardIdentifier == child.StandardIdentifier))
                    throw new AddAssetEdgeException($"You cannot add the connection \"#{Standard.AssetNumber} {DescriptorLabel.SelectedValue.ToLower()} #{child.AssetNumber}\" because doing so would create a dependency cycle in your hierarchy.");

                var edge = StandardConnectionSearch.SelectFirst(x => x.FromStandardIdentifier == Standard.StandardIdentifier && x.ToStandardIdentifier == child.StandardIdentifier);

                if (edge == null)
                {
                    edges.Add(edge = new StandardConnection
                    {
                        FromStandardIdentifier = Standard.StandardIdentifier,
                        ToStandardIdentifier = child.StandardIdentifier,
                        ConnectionType = DescriptorLabel.SelectedValue
                    });
                }
            }

            StandardConnectionStore.Insert(edges);
        }

        #endregion

        #region Classes

        private class AddAssetEdgeException : Exception
        {
            public AddAssetEdgeException(string message) : base(message)
            {
            }
        }

        #endregion

        #region Properties

        private Standard _standard;
        private Standard Standard
        {
            get
            {
                if (_standard == null && AssetID != null)
                    _standard = StandardSearch.Select(AssetID.Value);

                if (_standard == null)
                    HttpResponseHelper.Redirect($"/ui/admin/standards/standards/search", true);

                return _standard;
            }
        }

        private Guid? AssetID
        {
            get
            {
                var value = ViewState[nameof(AssetID)];

                return value != null
                    ? (Guid)value
                    : (Guid.TryParse(Request["assetId"], out Guid assetId)
                        ? assetId
                        : (Guid?)null);
            }
            set
            {
                ViewState[nameof(AssetID)] = value;
            }
        }

        private bool IsNewAssetsSearched
        {
            get
            {
                return ViewState[nameof(IsNewAssetsSearched)] != null
                       && (bool)ViewState[nameof(IsNewAssetsSearched)];
            }
            set { ViewState[nameof(IsNewAssetsSearched)] = value; }
        }

        private Guid? AssessmentQuestionID =>
            Guid.TryParse(Request["questionId"], out Guid questionId) ? questionId : (Guid?)null;

        private ICollection<Guid> SelectedAssets
        {
            get
            {
                return (ICollection<Guid>)(ViewState[nameof(SelectedAssets)]
                    ?? (ViewState[nameof(SelectedAssets)] = new HashSet<Guid>()));
            }
        }

        private string EdgeType => Request["edgetype"];

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreateRelationshipButton.Click += AddAssetButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;

            NewAssets.AutoBinding = false;
            NewAssets.EnablePaging = false;
            NewAssets.RowCreated += NewAssets_ItemCreated;
            NewAssets.DataBinding += NewAssets_DataBinding;
            NewAssets.RowDataBound += NewAssets_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();

                DescriptorLabel.Visible = EdgeType == "connection";

                if (EdgeType == "connection")
                    DescriptorLabel.SelectedIndex = 0;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (CreatorStatus.HasMessage)
                ScrollTop();

            base.OnPreRender(e);
        }

        private void LoadData()
        {
            if (!IsPostBack && AssessmentQuestionID.HasValue)
                AssetID = AssessmentQuestionID.Value;

            AssetType.Text = "standard";

            FromAsset.InnerHtml = string.Format(
                "{0} #{1}: {2}",
                Standard.StandardType,
                Standard.AssetNumber,
                Standard.ContentTitle);

            NewAssets.DataBind();

            DescriptorField.Visible = AssetID.HasValue;

            AssetParentID.Filter.HasChildren = true;
            AssetParentID.Value = null;
        }

        #endregion

        #region Event handlers

        private void NewAssets_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.CheckedChanged += IsSelected_CheckedChanged;
        }

        private void NewAssets_DataBinding(object sender, EventArgs e)
        {
            NewAssets.DataSource = GetNewAssetsData();
        }

        private void NewAssets_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Row.DataItem, "StandardIdentifier");

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.Checked = SelectedAssets.Contains(id);
        }

        private void IsSelected_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var row = (GridViewRow)chk.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var id = grid.GetDataKey<Guid>(row);

            if (!chk.Checked && SelectedAssets.Contains(id))
                SelectedAssets.Remove(id);
            else if (chk.Checked && !SelectedAssets.Contains(id))
                SelectedAssets.Add(id);
        }

        private void AddAssetButton_Click(object sender, EventArgs e)
        {
            if (SelectedAssets.Count == 0)
                return;

            var list = StandardSearch.Select(x => SelectedAssets.Contains(x.StandardIdentifier));

            try
            {
                AddAssets(list);

                Page.ClientScript.RegisterClientScriptBlock(
                    GetType(),
                    "Close",
                    "modalManager.closeModal(true);",
                    true);
            }
            catch (AddAssetEdgeException ex)
            {
                CreatorStatus.AddMessage(AlertType.Error, ex.Message);
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            IsNewAssetsSearched = true;

            SelectedAssets.Clear();

            NewAssets.DataBind();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IsNewAssetsSearched = false;

            AssetParentID.Value = null;
            StandardType.Value = null;
            SearchText.Text = null;

            SelectedAssets.Clear();

            NewAssets.DataBind();
        }

        #endregion

        #region Helper methods

        private SearchResultList GetNewAssetsData()
        {
            SearchResultList result = null;

            if (IsNewAssetsSearched)
            {
                if (AssetID.HasValue || StandardType.Value.IsNotEmpty())
                {
                    var filter = new StandardFilter
                    {
                        OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                        SelectorText = SearchText.Text.Trim()
                    };

                    var sortExpression = "Subtype, ContentTitle";

                    if (AssetParentID.HasValue)
                    {
                        filter.ParentStandardIdentifier = AssetParentID.Value.Value;
                        sortExpression = "Sequence";
                    }

                    if (!string.IsNullOrEmpty(StandardType.Value))
                        filter.StandardTypes = new[] { StandardType.Value };

                    filter.Exclusions.StandardIdentifier.Clear();
                    if (AssetID.HasValue)
                        filter.Exclusions.StandardIdentifier.Add(AssetID.Value);

                    filter.OrderBy = sortExpression;

                    result = StandardSearch.Bind(x => new
                    {
                        x.StandardIdentifier,
                        SubType = x.StandardType,
                        x.AssetNumber,
                        Title = x.ContentTitle,
                        ParentTitle = x.Parent.ContentTitle,
                        x.Sequence,
                        x.ContentTitle
                    }, filter).ToSearchResult();
                }

                var resultCount = result != null ? result.GetList().Count : 0;
                FoundAssetText.Text = resultCount > 0
                    ? $"{resultCount:n0} standards match your criteria"
                    : "No standards match your criteria";
            }

            var hasItems = result.IsNotEmpty();

            NewAssets.Visible = hasItems;
            CreateRelationshipButton.Visible = hasItems;

            return result;
        }

        private void AddAssets(IEnumerable<Standard> assets)
        {
            if (AssetID.HasValue)
                AddAssetEdge(assets);
        }

        private void ScrollTop()
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Create),
                "scroll_to",
                "selectAssetsPanel.scrollTop();",
                true);
        }

        #endregion
    }
}
