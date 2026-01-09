using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Database.Entities
{
    public partial class Search : SearchPage<TEntityFilter>
    {
        public override string EntityName => "Entity";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);

            SearchResults.Searched += (x, y) =>
            {
                var entities = TEntitySearch.Select(SearchCriteria.Filter);

                SummarizeComponents(entities);
                SummarizeFeatures(entities);
                SummarizeHierarchy(entities);

                SummarizeDuplicates();
                SummarizeProblems();

                var problems = DuplicateCollectionPanel.Visible
                    || DuplicateEntityPanel.Visible
                    || UnexpectedEntityPanel.Visible
                    || UnexpectedCollectionPanel.Visible
                    || OrphanEntityPanel.Visible
                    || OrphanTablePanel.Visible;

                NoProblemPanel.Visible = !problems;
            };
        }

        private void SummarizeComponents(List<TEntity> entities)
        {
            var components = entities
                .GroupBy(x => new { x.ComponentType, x.ComponentName })
                .Select(x => new
                {
                    x.Key.ComponentType,
                    x.Key.ComponentName,
                    Count = x.Count()
                })
                .OrderBy(x => x.ComponentType)
                .ThenBy(x => x.ComponentName)
                ;

            ComponentSummary.DataSource = components;
            ComponentSummary.DataBind();
        }

        private void SummarizeFeatures(List<TEntity> entities)
        {
            var subcomponents = entities
                .GroupBy(x => new { x.ComponentType, x.ComponentName, x.ComponentPart })
                .Select(x => new
                {
                    x.Key.ComponentType,
                    x.Key.ComponentName,
                    x.Key.ComponentPart,
                    Count = x.Count()
                })
                .OrderBy(x => x.ComponentType)
                .ThenBy(x => x.ComponentName)
                .ThenBy(x => x.ComponentPart)
                ;

            SubcomponentSummary.DataSource = subcomponents;
            SubcomponentSummary.DataBind();
        }

        private void SummarizeDuplicates()
        {
            var entities = TEntitySearch.Select(new TEntityFilter());

            var duplicateEntities = entities
                .GroupBy(x => new { x.EntityName })
                .Select(x => new
                {
                    Entity = x.Key.EntityName,
                    Count = x.Count()
                })
                .Where(x => x.Count > 1)
                .OrderBy(x => x.Entity)
                ;

            DuplicateEntityRepeater.DataSource = duplicateEntities;
            DuplicateEntityRepeater.DataBind();
            DuplicateEntityPanel.Visible = duplicateEntities.Any();

            var duplicateCollections = entities
                .GroupBy(x => new { x.CollectionSlug })
                .Select(x => new
                {
                    Collection = x.Key.CollectionSlug,
                    Count = x.Count()
                })
                .Where(x => x.Count > 1)
                .OrderBy(x => x.Collection)
                ;

            DuplicateCollectionRepeater.DataSource = duplicateCollections;
            DuplicateCollectionRepeater.DataBind();
            DuplicateCollectionPanel.Visible = duplicateCollections.Any();
        }

        private void SummarizeProblems()
        {
            var orphanEntities = TEntitySearch.GetOrphanEntities();
            OrphanEntityRepeater.DataSource = orphanEntities;
            OrphanEntityRepeater.DataBind();
            OrphanEntityPanel.Visible = orphanEntities.Any();

            var orphanTables = TEntitySearch.GetOrphanTables();
            OrphanTableRepeater.DataSource = orphanTables;
            OrphanTableRepeater.DataBind();
            OrphanTablePanel.Visible = orphanTables.Any();

            var unexpectedEntities = TEntitySearch.GetUnexpectedEntities();
            UnexpectedEntityRepeater.DataSource = unexpectedEntities;
            UnexpectedEntityRepeater.DataBind();
            UnexpectedEntityPanel.Visible = unexpectedEntities.Any();

            var unexpectedCollections = TEntitySearch.GetUnexpectedCollections();
            UnexpectedCollectionRepeater.DataSource = unexpectedCollections;
            UnexpectedCollectionRepeater.DataBind();
            UnexpectedCollectionPanel.Visible = unexpectedCollections.Any();
        }

        private void SummarizeHierarchy(List<TEntity> items)
        {
            var sb = new StringBuilder();

            var groupedByType = items
                .GroupBy(i => i.ComponentType)
                .OrderBy(g => g.Key);

            foreach (var typeGroup in groupedByType)
            {
                sb.AppendLine($"<h1 class='text-info my-4'>{typeGroup.Key}</h2>");

                sb.AppendLine("      <table class='table table-striped'>");

                var groupedByComponent = typeGroup
                    .GroupBy(i => i.ComponentName)
                    .OrderBy(g => g.Key);

                foreach (var componentGroup in groupedByComponent)
                {
                    sb.AppendLine($"        <tr><td><h3 class='m-0'>{componentGroup.Key}</h3></td>");
                    sb.AppendLine("            <td>");

                    sb.AppendLine("<table class='table table-striped table-bordered subcomponent'>");
                    sb.AppendLine("<tr><th>Subcomponent</th><th>Entity</th><th>Current Table</th><th>Future Table</th></tr>");

                    var groupedByPart = componentGroup
                        .GroupBy(i => i.ComponentPart)
                        .OrderBy(g => g.Key);

                    foreach (var partGroup in groupedByPart)
                    {
                        var n = 0;

                        foreach (var item in partGroup)
                        {
                            if (n == 0)
                                sb.AppendLine($"<tr><td rowspan='{partGroup.Count()}'><strong>{partGroup.Key}</strong></td>");
                            else
                                sb.AppendLine("<tr>");

                            n++;

                            var table = $"{item.StorageSchema}.{item.StorageTable}";

                            var rename = string.Empty;

                            if (item.StorageSchemaRename != null || item.StorageTableRename != null)
                            {
                                var schemaClass = item.StorageSchemaRename != null ? "text-danger" : "text-muted";

                                var tableClass = item.StorageTableRename != null ? "text-danger" : "text-muted";

                                rename = $"<span class='{schemaClass}'>{item.StorageSchemaRename ?? item.StorageSchema}</span>.<span class='{tableClass}'>{item.StorageTableRename ?? item.StorageTable}</span>";
                            }

                            sb.AppendLine($"  <td>{item.EntityName}</td><td>{table}</td><td class='fs-sm'>{rename}</td></tr>");
                        }
                    }

                    sb.AppendLine("</table>");

                    sb.AppendLine("            </td></tr>");
                }

                sb.AppendLine("      </table>");
            }

            EntityHierarchy.Text = sb.ToString();
        }
    }
}