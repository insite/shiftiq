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
using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Classify : AdminBasePage, IHasParentLinkParameters
    {

        public class DataNode : StandardGraphNode
        {
            public string StandardType { get; set; }
            public string StandardTypeIcon { get; set; }
            public string Code { get; set; }

            public int Depth { get; set; }
            public string CodePath { get; set; }

            public string TitleDefault { get; set; }
            public string TitleSelected { get; set; }

            public string HtmlPrefix { get; set; }
            public string HtmlPostfix { get; set; }

            public IEnumerable<DataNode> IncomingNodes => GetGraph().GetIncomingContainments(NodeId).Select(x => (DataNode)x.FromNode);
            public IEnumerable<DataNode> OutgoingNodes => GetGraph().GetOutgoingContainments(NodeId).Select(x => (DataNode)x.ToNode);

            public DataNode(Guid id) : base(id)
            {

            }
        }

        private class AssetChange
        {
            public string Subtype { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }

            public AssetChange()
            {
                Subtype = null;
                Code = null;
                Title = null;
            }
        }

        public Guid? AssetIdentifier => Guid.TryParse(Page.Request.QueryString["asset"], out var id) ? id : (Guid?)null;

        private Guid? AssetID
        {
            get { return ViewState[nameof(AssetID)] as Guid?; }
            set { ViewState[nameof(AssetID)] = value; }
        }

        private string LanguageCode => ShowTitleLanguages.SelectedValue;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowTitle.AutoPostBack = true;
            ShowTitle.CheckedChanged += ShowTitle_CheckedChanged;

            SearchButton.Click += SearchButton_Click;

            RecodeButton.Click += RecodeButton_Click;

            TreeViewRepeater.ItemDataBound += TreeViewRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();
            SaveButton.Visible = CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/standards/home");

            Page.MaintainScrollPositionOnPostBack = false;

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this, null, "Classify Standards");

                var assetID = AssetIdentifier.HasValue ? StandardSearch.Select(AssetIdentifier.Value)?.StandardIdentifier : null;
                AssetID = assetID;

                if (assetID.HasValue)
                {
                    var asset = StandardSearch.Select(assetID.Value);
                    if (asset.DocumentType != null)
                        RedirectToParent();
                }

                InitAssetSelector(assetID);
                InitShowTitleLanguages();

                OutlineSection.Visible = false;
                SearchSection.IsSelected = true;
                SaveButton.Visible = false;
            }

            if (!SelectedAssetID.HasValue)
                SelectedAssetID.Value = AssetID;
        }

        private void InitAssetSelector(Guid? defaultValue)
        {
            SelectedAssetID.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            SelectedAssetID.Value = defaultValue;
        }

        private void InitShowTitleLanguages()
        {
            var languages = Organization.Languages.OrderBy(x => x.DisplayName).ToList();

            ShowTitleLanguages.DataSource = languages;
            ShowTitleLanguages.DataTextField = "DisplayName";
            ShowTitleLanguages.DataValueField = "TwoLetterISOLanguageName";
            ShowTitleLanguages.DataBind();

            if (languages.Count > 0) ShowTitleLanguages.SelectedIndex = 0;
        }

        private void ShowTitle_CheckedChanged(object sender, EventArgs e)
        {
            ShowTitleLanguages.Visible = ShowTitle.Checked;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void RecodeButton_Click(object sender, EventArgs e)
        {
            LoadData(true);
        }

        private void TreeViewRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var node = (DataNode)e.Item.DataItem;

            var typeSelector = (StandardTypeComboBox)e.Item.FindControl("TypeSelector");
            typeSelector.Visible = ShowType.Checked;
            if (typeSelector.Visible)
            {
                typeSelector.Value = node.StandardType;
            }

            var codeInput = (ITextBox)e.Item.FindControl("CodeInput");
            codeInput.Visible = ShowCode.Checked;
            if (codeInput.Visible)
            {
                codeInput.Text = node.Code;
            }

            var titleInput = (ITextBox)e.Item.FindControl("TitleInput");
            titleInput.Visible = ShowTitle.Checked;
            if (titleInput.Visible)
            {
                titleInput.Text = node.TitleSelected;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var assetChanges = new Dictionary<Guid, AssetChange>();
            var assetIds = new List<Guid>();

            foreach (RepeaterItem repeaterItem in TreeViewRepeater.Items)
            {
                var assetID = Guid.Parse(((ITextControl)repeaterItem.FindControl("AssetID")).Text);
                var typeSelector = (StandardTypeComboBox)repeaterItem.FindControl("TypeSelector");
                var codeInput = (ITextBox)repeaterItem.FindControl("CodeInput");
                var titleInput = (ITextBox)repeaterItem.FindControl("TitleInput");

                var assetChange = new AssetChange();

                if (typeSelector.Visible) assetChange.Subtype = typeSelector.Value;
                if (codeInput.Visible) assetChange.Code = codeInput.Text;
                if (titleInput.Visible) assetChange.Title = titleInput.Text;

                assetChanges.Add(assetID, assetChange);
                assetIds.Add(assetID);
            }

            var (processed, updated) = ModifyStandards(assetIds, assetChanges);

            LoadData();

            if (updated.Count > 0)
                ClassifyStatus.AddMessage(AlertType.Success, $"{updated.Count} standards have been updated.");
            else
                ClassifyStatus.AddMessage(AlertType.Warning, $"There is nothing to update.");
        }

        private (HashSet<Guid> processed, HashSet<Guid> updated) ModifyStandards(List<Guid> assetIds, Dictionary<Guid, AssetChange> assetChanges)
        {
            var processed = new HashSet<Guid>();
            var updated = new HashSet<Guid>();

            StandardStore.Update(x => assetIds.Contains(x.StandardIdentifier), x =>
            {
                var change = assetChanges[x.StandardIdentifier];
                var isUpdate = false;

                if (!string.IsNullOrEmpty(change.Subtype) && (string.IsNullOrEmpty(x.StandardType) || !x.StandardType.Equals(change.Subtype, StringComparison.OrdinalIgnoreCase)))
                {
                    x.StandardType = change.Subtype;
                    isUpdate = true;
                }

                if (!string.IsNullOrEmpty(change.Code) && (string.IsNullOrEmpty(x.Code) || !x.Code.Equals(change.Code, StringComparison.OrdinalIgnoreCase)))
                {
                    x.Code = change.Code;
                    isUpdate = true;
                }

                if (!string.IsNullOrEmpty(change.Title)
                    && string.Equals(LanguageCode, "en", StringComparison.OrdinalIgnoreCase)
                    && (string.IsNullOrEmpty(x.ContentTitle) || !x.ContentTitle.Equals(change.Title, StringComparison.OrdinalIgnoreCase))
                    )
                {
                    x.ContentTitle = change.Title;
                    isUpdate = true;
                }

                processed.Add(x.StandardIdentifier);

                if (isUpdate)
                    updated.Add(x.StandardIdentifier);
            });

            return (processed, updated);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void LoadData(bool isRecode = false)
        {
            var assetID = SelectedAssetID.Value;

            OutlineSection.Visible = OutlineSection.IsSelected = assetID.HasValue;
            SavePanel.Visible = assetID.HasValue;

            if (assetID == null)
                return;

            var asset = StandardSearch.Select(assetID.Value);
            if (asset.DocumentType != null)
                RedirectToParent();

            var graph = StandardGraph<StandardGraphNode>.LoadOrganizationEdges(
                Organization.OrganizationIdentifier,
                id => new DataNode(id));

            if (!graph.HasNode(asset.StandardIdentifier))
                graph.AddNode(new DataNode(asset.StandardIdentifier));

            var root = (DataNode)graph.GetNode(asset.StandardIdentifier);

            {
                var nodeFilter = new HashSet<Guid> { asset.StandardIdentifier };

                FillNodeFilter(root);

                var dataItems = StandardSearch.Bind(
                    x => new
                    {
                        x.StandardIdentifier,
                        x.StandardType,
                        x.Code
                    },
                    x => nodeFilter.Contains(x.StandardIdentifier));
                var contentBlocks = ServiceLocator.ContentSearch.GetBlocks(
                    dataItems.Select(x => x.StandardIdentifier),
                    (new[] { ContentContainer.DefaultLanguage, LanguageCode }).Distinct(),
                    new[] { ContentLabel.Title });
                var types = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier).ToDictionary(x => x, x => x);

                foreach (var item in dataItems)
                {
                    var node = (DataNode)graph.GetNode(item.StandardIdentifier);
                    node.StandardType = item.StandardType;
                    node.StandardTypeIcon = StandardSearch.GetStandardTypeIcon(item.StandardType);
                    node.Code = item.Code;

                    if (contentBlocks.TryGetValue(item.StandardIdentifier, out var content))
                    {
                        node.TitleDefault = content.Title.GetText();
                        node.TitleSelected = content.Title.GetText(LanguageCode);
                    }
                }

                void FillNodeFilter(StandardGraphNode node)
                {
                    foreach (var edge in node.OutgoingEdges)
                    {
                        if (!nodeFilter.Contains(edge.ToNodeId))
                            nodeFilter.Add(edge.ToNodeId);

                        FillNodeFilter(edge.ToNode);
                    }
                }
            }

            SaveButton.Visible = ShowType.Checked || ShowCode.Checked || ShowTitle.Checked;
            RecodeButton.Visible = ShowCode.Checked && root != null && root.HasChildren;

            if (isRecode)
                AutoCode(root);

            TreeViewRepeater.DataSource = GetDataSource(root);
            TreeViewRepeater.DataBind();
        }

        private void AutoCode(DataNode node)
        {
            var code = 1;
            foreach (var oNode in node.OutgoingNodes)
            {
                oNode.Code = code.ToString();
                AutoCode(oNode);
            }
        }

        private List<DataNode> GetDataSource(DataNode root)
        {
            var plainNodes = new List<DataNode>();

            GetDataSource(root, plainNodes, 1, string.Empty);

            if (plainNodes.Count > 0)
            {
                var lastNode = plainNodes[plainNodes.Count - 1];
                if (lastNode.Depth != root.Depth)
                    lastNode.HtmlPostfix += BuildTreeEnd(lastNode.Depth - root.Depth);

                lastNode.HtmlPostfix += "</ul>";
            }

            return plainNodes;
        }

        private void GetDataSource(DataNode curNode, List<DataNode> plainNodes, int depth, string codePath)
        {
            if (curNode.StandardType.Equals("Document", StringComparison.OrdinalIgnoreCase))
                return;

            curNode.Depth = depth;
            curNode.CodePath = codePath.IsNotEmpty()
                ? codePath + "." + curNode.Code
                : curNode.Code;

            if (curNode.TitleDefault.IsEmpty())
                curNode.TitleDefault = "<i style='color:#999;'>No Title</i>";

            var prevNode = plainNodes.Count == 0
                ? null
                : plainNodes[plainNodes.Count - 1];

            if (prevNode == null)
                curNode.HtmlPrefix = $"<ul id='{TreeViewRepeater.ClientID}' class='tree-view' data-init='code'><li data-id='{curNode.NodeId}'>";
            else if (prevNode.Depth == curNode.Depth - 1)
                curNode.HtmlPrefix = $"<ul class='tree-view'><li data-id='{curNode.NodeId}'>";
            else if (prevNode.Depth == curNode.Depth)
                curNode.HtmlPrefix = $"<li data-id='{curNode.NodeId}'>";
            else
                curNode.HtmlPrefix = BuildTreeEnd(prevNode.Depth - curNode.Depth) + $"<li data-id='{curNode.NodeId}'>";

            if (prevNode != null && !curNode.HasChildren)
                curNode.HtmlPostfix = $"</li>";

            plainNodes.Add(curNode);

            foreach (var oNode in curNode.OutgoingNodes)
                GetDataSource(oNode, plainNodes, depth + 1, curNode.CodePath);
        }

        private string BuildTreeEnd(int levels) => string.Concat(Enumerable.Repeat("</ul></li>", levels));

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/standards/edit?id={AssetIdentifier}");

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"id={AssetIdentifier}"
                : null;
        }

    }
}
