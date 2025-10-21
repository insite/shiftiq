using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using InSite.Admin.Standards.Standards.Utilities;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Info : AdminBasePage
    {
        #region Classes

        private enum ContainerType
        {
            Info, Create, Reorder
        }

        private class StandardInfo
        {
            public Guid? GrandparentIdentifier { get; internal set; }
            public string GrandparentType { get; internal set; }
            public int? GrandparentAsset { get; internal set; }
            public Guid? ParentKey { get; internal set; }
            public Guid? ParentIdentifier { get; internal set; }
            public string ParentType { get; internal set; }
            public int? ParentAsset { get; internal set; }
            public string Type { get; internal set; }
            public int Asset { get; internal set; }
            public string Code { get; internal set; }
            public Guid Key { get; internal set; }
            public Guid Identifier { get; internal set; }

            public int ChildrenCount { get; internal set; }
            public int DownstreamRelationsCount { get; internal set; }

            public static StandardInfo Get(int asset)
            {
                return StandardSearch.BindFirst(x => new StandardInfo
                {
                    GrandparentIdentifier = x.Parent.Parent.StandardIdentifier,
                    GrandparentType = x.Parent.Parent.StandardType,
                    GrandparentAsset = x.Parent.Parent.AssetNumber,
                    ParentKey = x.Parent.StandardIdentifier,
                    ParentIdentifier = x.Parent.StandardIdentifier,
                    ParentType = x.Parent.StandardType,
                    ParentAsset = x.Parent.AssetNumber,
                    Type = x.StandardType,
                    Asset = x.AssetNumber,
                    Code = x.Code,
                    Key = x.StandardIdentifier,
                    Identifier = x.StandardIdentifier,

                    ChildrenCount = x.Children.Count,
                    DownstreamRelationsCount = x.ParentContainments.Count + x.OutgoingConnections.Count,
                }, x => x.OrganizationIdentifier == Organization.Key && x.AssetNumber == asset);
            }
        }

        private class SiblingInfo
        {
            public Guid Key { get; set; }
            public Guid Identifier { get; set; }
            public int Asset { get; set; }
            public string Type { get; set; }
        }

        [Serializable]
        protected class ControlData
        {
            public Guid StandardID { get; set; }
            public Guid StandardIdentifier { get; set; }
            public Guid? ParentKey { get; set; }
            public Guid? PrevSiblingKey { get; set; }
        }

        #endregion

        #region Properties

        private int? StandardAsset => int.TryParse(Request["asset"], out var value) ? value : (int?)null;

        protected ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)] ?? new ControlData();
            set => ViewState[nameof(CurrentData)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreateButton.Click += CreateButton_Click;
            CreateSaveButton.Click += CreateSaveButton_Click;
            CreateCancelButton.Click += CreateCancelButton_Click;

            ReorderButton.Click += ReorderButton_Click;
            ReorderSaveButton.Click += ReorderSaveButton_Click;
            ReorderCancelButton.Click += ReorderCancelButton_Click;

            IndentButton.Click += IndentButton_Click;
            OutdentButton.Click += OutdentButton_Click;

            CopyButton.Click += CopyButton_Click;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var standard = StandardAsset.HasValue ? StandardInfo.Get(StandardAsset.Value) : null;
            if (standard == null)
            {
                ScreenStatus.ShowClose = false;
                ScreenStatus.AddMessage(AlertType.Error, "Standard not found.");
                return;
            }

            var hasGraphError = false;
            var graph = StandardGraph<StandardGraphNode>.LoadOrganizationEdges(Organization.Key, id => new StandardGraphNode(id));
            var assetNode = graph.GetNode(standard.Key);
            var rootNodes = assetNode?.GetRootNodes();

            if (rootNodes != null)
            {
                foreach (var rootNode in rootNodes)
                {
                    var cyclePaths = rootNode.FindCycles();
                    if (cyclePaths.Length != 0)
                    {
                        var error = StandardGraphHelper.BuildDependencyCycleHtmlErrorMessage(rootNode.NodeId, cyclePaths, "/ui/admin/standards/info?asset={0}");

                        ScreenStatus.AddMessage(AlertType.Error, error);

                        hasGraphError = true;

                        break;
                    }
                }
            }

            GetSiblings(standard, graph, out SiblingInfo prevSibling, out SiblingInfo nextSibling);

            CurrentData = new ControlData
            {
                StandardID = standard.Identifier,
                StandardIdentifier = standard.Key,
                ParentKey = standard.ParentKey,
                PrevSiblingKey = prevSibling?.Key
            };

            var canWrite = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards, PermissionOperation.Write);
            var canDelete = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards, PermissionOperation.Delete);

            SetContainerVisibility(ContainerType.Info);

            RowCommands.Visible = true;
            EditButton.Visible = canWrite;
            CreateButton.Visible = canWrite;
            CopyButton.Visible = canWrite;
            DeleteButton.Visible = canDelete;
            ReorderButton.Visible = canWrite;
            IndentButton.Visible = canWrite;
            OutdentButton.Visible = canWrite;
            Separator1.Visible = canWrite;

            ReorderButton.Enabled = !hasGraphError && (standard.ChildrenCount > 1 || standard.DownstreamRelationsCount > 0);
            IndentButton.Enabled = !hasGraphError && prevSibling != null;
            OutdentButton.Enabled = !hasGraphError && standard.ParentIdentifier.HasValue;
            OutlineButton.Enabled = !hasGraphError;
            DownloadButton.Enabled = !hasGraphError;

            CreateInsertBefore.Enabled = CreateInsertAfter.Enabled = standard.ParentKey.HasValue;

            SetOutputValues(standard, prevSibling, nextSibling);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Info),
                "onInit",
                $"modalManager.setModalTitle({HttpUtility.JavaScriptStringEncode(standard.Type + " #" + standard.Asset, true)}); modalManager.setModalWidth(655);",
                true);
        }

        private void SetOutputValues(StandardInfo standard, SiblingInfo prevSibling, SiblingInfo nextSibling)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(
                standard.Identifier,
                Shift.Common.ContentContainer.DefaultLanguage,
                new string[] { ContentLabel.Title, ContentLabel.Summary });

            TitleOutput.InnerHtml = content.Title.GetText().IfNullOrEmpty("<i>(Untitled)</i>");
            SummaryOutput.Visible = !string.IsNullOrEmpty(SummaryOutput.InnerHtml = content.Summary.GetHtml());
            TypeOutput.InnerText = standard.Type;
            CodeField.Visible = !string.IsNullOrEmpty(CodeOutput.InnerText = standard.Code);
            KeyOutput.InnerText = standard.Key.ToString();
            IdentifierOutput.InnerText = standard.Identifier.ToString();
            CreateEntityTitle.InnerHtml = $"Asset #{standard.Asset}: Standard / {standard.Type} / {TitleOutput.InnerText}";

            if (GrandparentField.Visible = standard.GrandparentIdentifier.HasValue)
            {
                var grandparentTitle = ServiceLocator.ContentSearch.GetTitleText(standard.GrandparentIdentifier.Value);

                GrandparentLink.NavigateUrl = "/ui/admin/standards/info?asset=" + standard.GrandparentAsset;
                GrandparentLink.Text = $"{standard.GrandparentType} #{standard.GrandparentAsset}: {grandparentTitle.IfNullOrEmpty("<i>(Untitled)</i>")}";
            }

            if (ParentField.Visible = standard.ParentIdentifier.HasValue)
            {
                var parentTitle = ServiceLocator.ContentSearch.GetTitleText(standard.ParentIdentifier.Value);

                ParentLink.NavigateUrl = "/ui/admin/standards/info?asset=" + standard.ParentAsset;
                ParentLink.Text = $"{standard.ParentType} #{standard.ParentAsset}: {parentTitle.IfNullOrEmpty("<i>(Untitled)</i>")}";
            }

            if (PreviousSiblingField.Visible = prevSibling != null)
            {
                var siblingTitle = ServiceLocator.ContentSearch.GetTitleText(prevSibling.Identifier);

                PreviousSiblingLink.NavigateUrl = "/ui/admin/standards/info?asset=" + prevSibling.Asset;
                PreviousSiblingLink.Text = $"{prevSibling.Type} #{prevSibling.Asset}: {siblingTitle.IfNullOrEmpty("<i>(Untitled)</i>")}";
            }

            if (NextSiblingField.Visible = nextSibling != null)
            {
                var siblingTitle = ServiceLocator.ContentSearch.GetTitleText(nextSibling.Identifier);

                NextSiblingLink.NavigateUrl = "/ui/admin/standards/info?asset=" + nextSibling.Asset;
                NextSiblingLink.Text = $"{nextSibling.Type} #{nextSibling.Asset}: {siblingTitle.IfNullOrEmpty("<i>(Untitled)</i>")}";
            }

            CopyButton.OnClientClick = $"return confirm({HttpUtility.JavaScriptStringEncode($"Are you sure you want to make copy of {standard.Type + " #" + standard.Asset}?", true)})";
        }

        private static void GetSiblings(StandardInfo standard, StandardGraph<StandardGraphNode> graph, out SiblingInfo prevSibling, out SiblingInfo nextSibling)
        {
            prevSibling = null;
            nextSibling = null;

            if (!standard.ParentKey.HasValue)
                return;

            var parent = graph.GetNode(standard.ParentKey.Value);
            var siblings = parent.OutgoingEdges.Select(x => x.ToNode).ToArray();
            var currentIndex = Array.FindIndex(siblings, x => x.NodeId == standard.Key);
            var prevSiblingKey = currentIndex > 0 ? siblings[currentIndex - 1].NodeId : (Guid?)null;
            var nextSiblingKey = currentIndex < siblings.Length - 1 ? siblings[currentIndex + 1].NodeId : (Guid?)null;

            var siblingFilter = new List<Guid>();

            if (prevSiblingKey.HasValue)
                siblingFilter.Add(prevSiblingKey.Value);

            if (nextSiblingKey.HasValue)
                siblingFilter.Add(nextSiblingKey.Value);

            var siblingEntities = StandardSearch.Bind(x => new SiblingInfo
            {
                Key = x.StandardIdentifier,
                Identifier = x.StandardIdentifier,
                Asset = x.AssetNumber,
                Type = x.StandardType,
            }, x => siblingFilter.Contains(x.StandardIdentifier));

            if (prevSiblingKey.HasValue)
                prevSibling = siblingEntities.Single(x => x.Key == prevSiblingKey.Value);

            if (nextSiblingKey.HasValue)
                nextSibling = siblingEntities.Single(x => x.Key == nextSiblingKey.Value);
        }

        #endregion

        #region Event handlers

        private void CreateButton_Click(object sender, EventArgs e)
        {
            SetContainerVisibility(ContainerType.Create);

            CreateType.Value = null;
            CreateTitle.Text = null;
            CreateInsertIn.Checked = true;

            EnableSubCommands(CreateButton.ClientID, true);
        }

        private void CreateSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                EnableSubCommands(CreateButton.ClientID, true);
                return;
            }

            var parentAsset = StandardSearch.BindFirst(x => new
            {
                x.StandardIdentifier,
                x.ParentStandardIdentifier,
                x.Sequence
            }, x => x.StandardIdentifier == CurrentData.StandardIdentifier);

            if (!CreateInsertIn.Checked && !parentAsset.ParentStandardIdentifier.HasValue)
                CreateInsertIn.Checked = true;

            var standard = StandardFactory.Create(CreateType.Value);
            standard.StandardIdentifier = UniqueIdentifier.Create();
            standard.ContentTitle = CreateTitle.Text;

            try
            {
                if (CreateInsertIn.Checked)
                {
                    standard.ParentStandardIdentifier = parentAsset.StandardIdentifier;
                    standard.Sequence = StandardSearch.SelectNextSequence(parentAsset.StandardIdentifier);

                    StandardStore.Insert(standard);
                }
                else
                {
                    standard.ParentStandardIdentifier = parentAsset.ParentStandardIdentifier.Value;

                    var itemsOrder = new List<int>();
                    var siblings = OutlineHelper.LoadReorderItems(standard.ParentStandardIdentifier.Value);

                    foreach (var sibling in siblings)
                    {
                        if (sibling.Key == CurrentData.StandardIdentifier)
                        {
                            if (CreateInsertBefore.Checked)
                            {
                                itemsOrder.Add(standard.AssetNumber);
                                itemsOrder.Add(sibling.Number);
                            }
                            else if (CreateInsertAfter.Checked)
                            {
                                itemsOrder.Add(sibling.Number);
                                itemsOrder.Add(standard.AssetNumber);
                            }
                        }
                        else
                        {
                            itemsOrder.Add(sibling.Number);
                        }
                    }

                    StandardStore.Insert(standard);

                    if (itemsOrder.Count > 0)
                        OutlineHelper.SaveReorder(standard.ParentStandardIdentifier.Value, itemsOrder);
                }
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                {
                    EnableSubCommands(CreateButton.ClientID, true);
                    ScreenStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                    return;
                }

                throw;
            }

            CloseWindowAndRefresh();
        }

        private void CreateCancelButton_Click(object sender, EventArgs e)
        {
            SetContainerVisibility(ContainerType.Info);
        }

        private void ReorderButton_Click(object sender, EventArgs e)
        {
            var data = OutlineHelper.LoadReorderItems(CurrentData.StandardIdentifier);
            if (data.Length == 0)
                return;

            SetContainerVisibility(ContainerType.Reorder);

            ReorderState.Value = string.Empty;

            ReorderRepeater.DataSource = data.Select(x => new
            {
                x.Number,
                x.Type,
                Title = HttpUtility.HtmlEncode(x.Title.IfNullOrEmpty("(Untitled)")),
                Icon = x.Icon ?? StandardSearch.GetStandardTypeIcon(x.Type)
            });
            ReorderRepeater.DataBind();

            EnableSubCommands(ReorderButton.ClientID, true);
        }

        private void ReorderSaveButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReorderState.Value))
            {
                var reorderState = JsonConvert.DeserializeObject<int[]>(ReorderState.Value);

                OutlineHelper.SaveReorder(CurrentData.StandardIdentifier, reorderState);
            }

            ReorderState.Value = string.Empty;

            ReorderRepeater.DataSource = null;
            ReorderRepeater.DataBind();

            CloseWindowAndRefresh();
        }

        private void ReorderCancelButton_Click(object sender, EventArgs e)
        {
            ReorderRepeater.DataSource = null;
            ReorderRepeater.DataBind();

            SetContainerVisibility(ContainerType.Info);
        }

        private void IndentButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentData?.PrevSiblingKey != null)
                    OutlineHelper.Indent(CurrentData.StandardIdentifier, CurrentData.PrevSiblingKey.Value);

                CloseWindowAndRefresh();
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                {
                    ScreenStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                    return;
                }

                throw;
            }
        }

        private void OutdentButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentData?.ParentKey != null)
                    OutlineHelper.Outdent(CurrentData.StandardIdentifier, CurrentData.ParentKey.Value);

                CloseWindowAndRefresh();
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                {
                    ScreenStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                    return;
                }

                throw;
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            var standard = ServiceLocator.StandardSearch.GetStandard(CurrentData.StandardIdentifier);
            standard.AssetNumber = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            standard.StandardIdentifier = default;
            standard.OrganizationIdentifier = Organization.OrganizationIdentifier;
            standard.StandardIdentifier = UniqueIdentifier.Create();
            standard.Sequence = standard.ParentStandardIdentifier.HasValue
                ? StandardSearch.SelectNextSequence(standard.ParentStandardIdentifier.Value)
                : 1;

            standard.ContentTitle = $"{standard.ContentTitle} - Copy";

            StandardStore.Insert(standard);

            CloseWindowAndRefresh();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var asset = StandardSearch.SelectModel(Organization.OrganizationIdentifier, StandardAsset.Value);
            var publication = new StandardPublicationModel(Organization.OrganizationIdentifier, User.FullName, "Download", asset);

            var json = publication.SerializeAsJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            var filename = StringHelper.Sanitize($"{asset.StandardType}-{asset.AssetNumber}", '-', false);

            Response.SendFile(filename, bytes, "application/json");
        }

        #endregion

        #region Methods (helpers)

        private void SetContainerVisibility(ContainerType container)
        {
            InfoContainer.Visible = container == ContainerType.Info;
            CreateContainer.Visible = container == ContainerType.Create;
            ReorderContainer.Visible = container == ContainerType.Reorder;
        }

        private void CloseWindowAndRefresh()
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Info),
                "close_window",
                "modalManager.closeModal({ action: 'refresh' });",
                true
            );
        }

        private void EnableSubCommands(string id, bool enabled)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Info),
                "enable_subcommands",
                $"standardInfo.enableSubCommands({HttpUtility.JavaScriptStringEncode(id, true)},{(enabled ? "true" : "false")});",
                true
            );
        }

        #endregion
    }
}