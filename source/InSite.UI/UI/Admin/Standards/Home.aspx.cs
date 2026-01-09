using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Standards
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var insite = CurrentSessionState.Identity.IsOperator;

            var counts = ServiceLocator.SiteSearch.SelectCount(Organization.OrganizationIdentifier);
            var hasCounts = counts.Length > 0;
            var sum = counts.Sum(x => x.Count);

            CountStandards();
            CountCollections();
            CountDocuments();

            var canWrite = Identity.IsGranted(PermissionIdentifiers.Admin_Standards, PermissionOperation.Write);

            UploadStandardsRow.Visible = canWrite;
            TroubleshootStanradrs.Visible = canWrite;
        }

        private StandardFilter CreateStandardFilter()
            => new StandardFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };

        private static readonly Dictionary<string, int> StandardOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { StandardType.Blueprint, 1 },
            { StandardType.Scenario, 1 },
            { StandardType.Document, 2 },
        };

        private void CountStandards()
        {
            var filter = CreateStandardFilter();
            var types = StandardSearch.GetAllTypeItems(Organization.Identifier);

            StandardRepeater.DataSource = types
                .Where(x => x.ItemName != StandardType.Collection)
                .OrderBy(x => StandardOrder.GetOrDefault(x.ItemName, 0)).ThenBy(x => x.ItemName)
                .Select(x =>
                {
                    filter.StandardTypes = new[] { x.ItemName };

                    return new
                    {
                        Url = x.ItemName == StandardType.Document
                            ? "/ui/admin/standards/documents/search"
                            : $"/ui/admin/standards/standards/search?type={x.ItemName}",
                        Count = StandardSearch.Count(filter),
                        Title = x.ItemName.Pluralize(),
                        Icon = x.ItemIcon.IfNullOrEmpty("far fa-square-question")
                    };
                });
            StandardRepeater.DataBind();
        }

        private void CountCollections()
        {
            var filter = CreateStandardFilter();

            filter.StandardTypes = new[] { Shift.Constant.StandardType.Collection };
            var collectionStandardCount = StandardSearch.Count(filter);
            CollectionCount.Text = $"{collectionStandardCount:n0}";
        }

        private void CountDocuments()
        {
            var filter = CreateStandardFilter();
            var data = StandardSearch.CountDocumentTypes(filter);
            var totalCount = data.Sum(x => x.Count);
            NoDocuments.Visible = totalCount == 0;

            DocumentCounterRepeater.DataSource = data;
            DocumentCounterRepeater.DataBind();
        }

        protected static string GetDocumentTypeName(string name)
        {
            if (string.Equals(name, DocumentType.NationalOccupationStandard, StringComparison.OrdinalIgnoreCase))
                return "NOS";
            else
                return name;
        }
    }
}