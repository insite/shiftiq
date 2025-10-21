using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Graphs;

namespace InSite.Persistence
{
    public static class StandardGraphHelper
    {
        #region Constants

        private const string DefaulEditorUrl = "/ui/admin/standards/edit?id={0}";

        #endregion

        #region Classes

        public interface IAssetInfo
        {
            Guid AssetId { get; }
            string Subtype { get; }
            int Number { get; }
            Guid Thumbprint { get; }
        }

        private class AssetInfo : IAssetInfo
        {
            public Guid AssetId { get; set; }
            public string Subtype { get; set; }
            public int Number { get; set; }
            public Guid Thumbprint { get; set; }
        }

        #endregion

        public static IEnumerable<Guid> GetDependencyCycleIdentifiers<TNode>(Guid rootId, TNode[][] cyclePaths) where TNode : GraphNodeModel
        {
            var ids = new HashSet<Guid> { rootId };

            foreach (var cyclePath in cyclePaths)
            {
                foreach (var node in cyclePath)
                {
                    if (!ids.Contains(node.NodeId))
                        ids.Add(node.NodeId);
                }
            }

            return ids;
        }

        public static string BuildDependencyCycleHtmlErrorMessage<TNode>(Guid rootId, TNode[][] cyclePaths, string editorUrl = DefaulEditorUrl) where TNode : GraphNodeModel
        {
            var assetFilter = GetDependencyCycleIdentifiers(rootId, cyclePaths);
            var assets = SelectAssets(assetFilter);

            var infoCyclePaths = new IAssetInfo[cyclePaths.Length][];

            for (var x = 0; x < cyclePaths.Length; x++)
            {
                var nodePath = cyclePaths[x];
                var infoPath = infoCyclePaths[x] = new IAssetInfo[nodePath.Length];

                for (var y = 0; y < nodePath.Length; y++)
                {
                    var nodeId = nodePath[y].NodeId;
                    infoPath[y] = assets[nodeId];
                }
            }

            return BuildDependencyCycleHtmlErrorMessage(assets[rootId], infoCyclePaths, editorUrl);
        }

        public static string BuildDependencyCycleHtmlErrorMessage(IAssetInfo root, IAssetInfo[][] cyclePaths, string editorUrl = DefaulEditorUrl)
        {
            if (cyclePaths.Length == 0)
                return string.Empty;

            var hasOneCycle = cyclePaths.Length == 1;

            var errorBuilder = new StringBuilder();

            errorBuilder.Append("The ");

            WriteAssetLink(errorBuilder, root, editorUrl);

            errorBuilder.Append(" contains ");
            errorBuilder.Append(Shift.Common.Humanizer.ToQuantity(cyclePaths.Length, "dependency cycle"));
            errorBuilder.Append(", which must be removed before you can outline this standard.");

            if (hasOneCycle)
                errorBuilder.Append(" The cycle is: ");
            else
                errorBuilder.Append(" The cycles are:");

            errorBuilder.Append("<ul>");

            foreach (var path in cyclePaths)
            {
                errorBuilder.Append("<li>");

                WritePathString(errorBuilder, path, editorUrl);

                errorBuilder.Append("</li>");
            }

            errorBuilder.Append("</ul>");

            return errorBuilder.ToString();
        }

        #region Helper methods

        private static IDictionary<Guid, AssetInfo> SelectAssets(IEnumerable<Guid> ids)
        {
            return StandardSearch
                .Bind(
                    x => new AssetInfo
                    {
                        AssetId = x.StandardIdentifier,
                        Subtype = x.StandardType,
                        Number = x.AssetNumber,
                        Thumbprint = x.StandardIdentifier
                    },
                    x => ids.Contains(x.StandardIdentifier))
                .ToDictionary(x => x.AssetId, x => x);
        }

        private static void WritePathString(
            StringBuilder sb,
            IEnumerable<IAssetInfo> path,
            string editorUrl)
        {
            if (!path.Any())
                return;

            foreach (var node in path)
            {
                WriteAssetLink(sb, node, editorUrl);

                sb.Append(" / ");
            }

            sb.Remove(sb.Length - 3, 3);
        }

        private static void WriteAssetLink(StringBuilder sb, IAssetInfo asset, string editorUrl)
        {
            if (string.IsNullOrEmpty(editorUrl) || asset == null)
                return;

            sb.Append("<a href='")
                .AppendFormat(editorUrl, asset.Thumbprint)
                .AppendFormat("'>{0} #{1}</a>", asset.Subtype, asset.Number);
        }

        #endregion
    }
}
