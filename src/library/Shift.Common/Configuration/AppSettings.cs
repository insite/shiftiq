using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shift.Common
{
    public class AppSettings
    {
        public ReleaseSettings Release { get; set; }
        public Application Application { get; set; }
        public SecuritySettings Security { get; set; }
        public DatabaseSettings Database { get; set; }
        public TimelineSettings Timeline { get; set; }
        public IntegrationSettings Integration { get; set; }
        public Variant Variant { get; set; }
        public Platform Platform { get; set; }
        public EngineSettings Engine { get; set; }
        public ShiftSettings Shift { get; set; }
        public PartitionSettings[] Partitions { get; set; }
        public RouteSettings RouteSettings { get; set; }

        public List<string> ConfigurationProviders { get; set; } = new List<string>();

        public string DataFolderEnterprise
            => Path.Combine(Application.DataPath, Partition.Tenant, Release.Environment);

        public string DataFolderShare
            => Path.Combine(Application.DataPath, "E00", Release.Environment);

        public string CssFileUrl
            => Application.StylePath + Partition.Style + ".css";

        public string HelpUrl => Partition.HelpUrl ?? Application.HelpUrl;

        public string v1ApiBaseUrl
            => UrlHelper.GetAbsoluteUrl(Partition.Domain, Release.GetEnvironment(), "/api", Partition.Tenant);

        public string v2ApiBaseUrl
            => Shift.Api.Hosting.BaseUrl;

        public EnvironmentModel Environment => Release.GetEnvironment();

        private IPartitionModel _partition;

        public IPartitionModel Partition
        {
            get
            {
                if (_partition != null)
                    return _partition;

                var partition = Partitions.SingleOrDefault(p => StringHelper.Equals(p.Tenant, Application.Partition));

                if (partition == null)
                    throw new InvalidOperationException($"Partition not found: {Application.Partition}");

                var current = new PartitionSettings();

                current.Number = partition.Number;

                current.Brand = partition.Brand;
                current.Domain = partition.Domain;
                current.Email = partition.Email;
                current.Name = partition.Name;
                current.Slug = partition.Slug;
                current.Style = partition.Style;

                current.Identifier = partition.Identifier;

                current.WhitelistDomains = partition.WhitelistDomains;
                current.WhitelistEmails = partition.WhitelistEmails;

                current.HelpUrl = partition.HelpUrl;

                _partition = current;

                return _partition;
            }
        }
    }
}