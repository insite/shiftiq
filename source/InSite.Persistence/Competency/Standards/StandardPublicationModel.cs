using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardPublicationModel
    {
        public StandardPublicationModel(Guid organizationId, string publisher, string command, StandardModel asset)
        {
            Date = DateTime.UtcNow;

            OrganizationIdentifier = organizationId;
            Publisher = publisher;
            Command = command;
            Asset = asset;

            if (Asset != null)
            {
                AssetType = "Standard";
                AssetSubtype = Asset.StandardType;
                Number = Asset.AssetNumber;
                Title = Asset.Title;
            }
        }

        public DateTime Date { get; set; }
        public string Publisher { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AssetType { get; set; }
        public string AssetSubtype { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int Version { get; set; }

        public string Command { get; set; }
        public int CountInserts { get; set; }
        public int CountUpdates { get; set; }
        public int CountDeletes { get; set; }
        public int ExecutionTime { get; set; }

        public StandardModel Asset { get; set; }

        #region Helper methods

        public string SerializeAsJson()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }

        public string SerializeAsMarkdown()
        {
            var sb = new StringBuilder();

            var tiers = GetMarkdownTiers(Asset);
            if (tiers.Length > 0)
                sb.Append("Tiers: ").AppendLine(string.Join(", ", tiers));

            sb.AppendLine().AppendLine();

            SerializeAsMarkdown(0, tiers, Asset, sb);

            return sb.ToString();
        }

        private static void SerializeAsMarkdown(int depth, string[] tiers, StandardModel asset, StringBuilder sb)
        {
            sb.Append('#', depth + 1);

            //if (!string.IsNullOrEmpty(asset.Code))
            //    sb.Append(' ').Append(asset.Code).Append('.');

            sb.Append(' ').Append(asset.Title);

            if (depth >= tiers.Length ||
                string.Compare(tiers[depth], asset.StandardType, StringComparison.OrdinalIgnoreCase) != 0)
                sb.Append(" [").Append(asset.StandardType).Append(']');

            sb.AppendLine().AppendLine();

            if (!string.IsNullOrEmpty(asset.Summary))
                sb.Append("Summary:").AppendLine().Append(asset.Summary).AppendLine().AppendLine();

            foreach (var child in asset.Children)
                SerializeAsMarkdown(1 + depth, tiers, child, sb);
        }

        private static string[] GetMarkdownTiers(StandardModel model)
        {
            var data = new List<Dictionary<string, int>>();

            GetMarkdownTiers(0, model, data);

            return data.Select(x => x.OrderByDescending(y => y.Value).Select(y => y.Key).FirstOrDefault()).ToArray();
        }

        private static void GetMarkdownTiers(int depth, StandardModel model, List<Dictionary<string, int>> data)
        {
            if (data.Count == depth)
                data.Add(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

            var dict = data[depth];
            if (!dict.ContainsKey(model.StandardType))
                dict.Add(model.StandardType, 0);

            dict[model.StandardType]++;

            foreach (var child in model.Children)
                GetMarkdownTiers(depth + 1, child, data);
        }

        #endregion
    }
}
