using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Standards.Standards.Controls;
using InSite.Admin.Standards.Standards.Utilities;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Standards
{
    public partial class Manage : AdminBasePage
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class AjaxArguments
        {
            [JsonProperty(PropertyName = "mode")]
            public string ModeName { get; private set; }

            [JsonProperty(PropertyName = "number")]
            public int StandardAsset { get; private set; }

            [JsonProperty(PropertyName = "path")]
            public string StandardPath { get; private set; }
        }

        [Serializable]
        protected class ControlData
        {
            public Guid StandardID { get; set; }
            public Guid RootKey { get; set; }
            public int RootNumber { get; set; }
        }

        #endregion

        #region Properties

        protected ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            set => ViewState[nameof(CurrentData)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EditorUpdatePanel.Request += EditorUpdatePanel_Request;

            EditorContainer.ControlAdded += EditorContainer_ControlAdded;

            UpdateTreeButton.Click += UpdateTreeButton_Click;

            PrintButton.Click += PrintButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var standard = CurrentSessionState.Identity.IsGranted("Admin/Standards") && Guid.TryParse(Request["standard"], out var standardId)
                ? StandardSearch.Select(standardId)
                : null;

            if (standard == null || standard.OrganizationIdentifier != Organization.OrganizationIdentifier)
                RedirectToSearch();

            CurrentData = new ControlData
            {
                StandardID = standard.StandardIdentifier,
                RootKey = StandardSearch.GetStandardRootKey(standard.StandardIdentifier) ?? standard.StandardIdentifier,
            };

            EditViewButtons.Visible = CurrentSessionState.Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write);

            if (standard.StandardIdentifier != CurrentData.RootKey)
                standard = StandardSearch.Select(CurrentData.RootKey);

            CurrentData.RootNumber = standard.AssetNumber;

            PageHelper.AutoBindHeader(this, null, $"{standard.ContentTitle ?? standard.ContentName ?? "Untitled"} ");

            LoadTreeView();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            CommonScript.Visible = CurrentData != null;
        }

        #endregion

        #region Event handlers

        private void EditorUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (string.IsNullOrEmpty(e.Value))
                return;

            var args = JsonConvert.DeserializeObject<AjaxArguments>(e.Value);
            if (args.ModeName == "edit")
            {
                var editor = EditorContainer.GetControl() as OutlineEdit
                    ?? (OutlineEdit)EditorContainer.LoadControl("~/UI/Admin/Standards/Standards/Controls/OutlineEdit.ascx");

                var canDelete = Identity.IsGranted(PermissionIdentifiers.Admin_Standards, PermissionOperation.Delete);

                if (!editor.LoadData(args.StandardAsset, canDelete))
                {
                    EditorContainer.UnloadControl();
                    ScreenStatus.AddMessage(AlertType.Error, "Unable to load standard editor: " + args.StandardPath);
                }
            }
            else if (args.ModeName == "translate")
            {
                var translate = EditorContainer.GetControl() as OutlineTranslate
                    ?? (OutlineTranslate)EditorContainer.LoadControl("~/UI/Admin/Standards/Standards/Controls/OutlineTranslate.ascx");

                if (!translate.LoadData(args.StandardAsset))
                {
                    EditorContainer.UnloadControl();
                    ScreenStatus.AddMessage(AlertType.Error, "Unable to load standard translator: " + args.StandardPath);
                }
            }
            else if (args.ModeName == "unload")
            {
                EditorContainer.UnloadControl();
            }

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Manage),
                "update_position",
                "outline.helpers.containerPosition.update();",
                true);
        }

        private void EditorContainer_ControlAdded(object sender, EventArgs e)
        {
            var control = EditorContainer.GetControl();
            if (control is OutlineEdit edit)
            {
                edit.Updated += Edit_Updated;
            }
            else if (control is OutlineTranslate translate)
            {
                translate.Updated += Translate_Updated;
            }
        }

        private void Edit_Updated(object sender, EventArgs e)
        {
            EditorContainer.UnloadControl();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Manage),
                "update_tree",
                $"outline.settings.reloadTree();",
                true);
        }

        private void Translate_Updated(object sender, EventArgs e)
        {
            EditorContainer.UnloadControl();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Manage),
                "update_tree",
                $"outline.settings.reloadTree();",
                true);
        }

        private void UpdateTreeButton_Click(object sender, EventArgs e)
        {
            LoadTreeView();
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            var graph = CurrentData?.RootKey != null ? LoadGraph(CurrentData.RootKey) : null;
            if (graph == null)
                return;

            var rootNode = graph.GetNode(CurrentData.RootKey);
            var printNodes = new List<OutlinePrint.NodeItem>();

            AddPrintNode(rootNode, printNodes);

            byte[] data;

            using (var stream = new MemoryStream())
            {
                OutlinePrint.Print(printNodes, stream);

                data = new byte[stream.Length];

                stream.Position = 0;
                stream.Read(data, 0, data.Length);
            }

            Response.SendFile($"standard-outline-{CurrentData.RootNumber}.docx", data, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            void AddPrintNode(OutlineTreeNodeRepeater.DataItem dNode, List<OutlinePrint.NodeItem> pNodes)
            {
                StandardNodeType itemType;

                if (dNode.Type.Equals(StandardType.Profile, StringComparison.OrdinalIgnoreCase))
                    itemType = StandardNodeType.Profile;
                else if (dNode.Type.Equals(StandardType.Framework, StringComparison.OrdinalIgnoreCase))
                    itemType = StandardNodeType.Framework;
                else if (dNode.Type.Equals(StandardType.Area, StringComparison.OrdinalIgnoreCase))
                    itemType = StandardNodeType.Area;
                else
                    itemType = StandardNodeType.Competency;

                var node = new OutlinePrint.NodeItem
                {
                    Label = dNode.Code,
                    Title = dNode.Title,
                    Subtitle = $"{dNode.TypeName} Asset #{dNode.Number}",
                    ItemType = itemType
                };

                pNodes.Add(node);

                var innerEdges = dNode.OutgoingEdges
                    .OrderBy(x => x.ToNode.Code)
                    .ToList();

                if (innerEdges.Count > 0)
                {
                    node.Children = new List<OutlinePrint.NodeItem>();
                    foreach (var edge in innerEdges)
                        AddPrintNode(edge.ToNode, node.Children);
                }
            }
        }

        #endregion

        #region Methods (data binding)

        private StandardGraph<OutlineTreeNodeRepeater.DataItem> LoadGraph(Guid rootKey)
        {
            var graph = StandardGraph<OutlineTreeNodeRepeater.DataItem>.LoadOrganizationEdges(
                Organization.OrganizationIdentifier,
                id => new OutlineTreeNodeRepeater.DataItem(id));

            if (!graph.HasNode(rootKey))
                graph.AddNode(new OutlineTreeNodeRepeater.DataItem(rootKey));

            var cyclePaths = graph.FindCycles(rootKey);
            if (cyclePaths.Length > 0)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    StandardGraphHelper.BuildDependencyCycleHtmlErrorMessage(rootKey, cyclePaths));
                return null;
            }

            OutlineTreeNodeRepeater.LoadDataItems(rootKey, graph);

            return graph;
        }

        private void LoadTreeView()
        {
            var graph = CurrentData != null ? LoadGraph(CurrentData.RootKey) : null;
            if (graph == null)
                return;

            var root = graph.GetNode(CurrentData.RootKey);

            NodeRepeater.LoadData(new[] { root });

            SetupFooter(root);
        }

        private void SetupFooter(OutlineTreeNodeRepeater.DataItem root)
        {
            var nodesCount = 1;
            var edgesCount = 0;

            CalcNodesEdgesCount(root);

            FooterRow.Visible = true;
            FooterText.Text = $"Asset #{root.Number} is a valid hierarchy with no dependency cycles. It contains {nodesCount:n0} nodes and {edgesCount:n0} edges.";

            void CalcNodesEdgesCount(OutlineTreeNodeRepeater.DataItem node)
            {
                var outgoingEdges = node.OutgoingEdges;

                foreach (var edge in outgoingEdges)
                {
                    nodesCount++;

                    CalcNodesEdgesCount(edge.ToNode);
                }

                edgesCount += outgoingEdges.Count;
            }
        }

        #endregion

        #region Methods (helpers)

        private static void RedirectToSearch()
        {
            HttpResponseHelper.Redirect("/ui/admin/standards/standards/search", true);
        }

        #endregion
    }
}
