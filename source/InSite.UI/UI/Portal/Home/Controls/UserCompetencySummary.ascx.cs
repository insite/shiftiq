using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Portal.Records.Logbooks.Controls;
using InSite.UI.Portal.Standards.Controls;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Portal.Home.Controls
{
    public partial class UserCompetencySummary : BaseUserControl
    {
        #region Classes

        public class FrameworkGroup
        {
            public Guid UserId { get; set; }
            public Guid FrameworkId { get; set; }
            public Guid JournalSetupId { get; set; }
            public string FrameworkTitle { get; set; }
            public string FrameworkSummary { get; set; }

            public bool ShowOutlineTree { get; set; }
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class TreeDataItem
        {
            [JsonProperty(PropertyName = "tree")]
            public string TreeId { get; set; }

            [JsonProperty(PropertyName = "panel")]
            public string TreePanelId { get; set; }

            [JsonProperty(PropertyName = "update")]
            public string UpdatePanelId { get; set; }

            [JsonProperty(PropertyName = "expand")]
            public string ExpandAllId { get; set; }

            [JsonProperty(PropertyName = "collapse")]
            public string CollapseAllId { get; set; }

            [JsonProperty(PropertyName = "displayStatus")]
            public string DisplayStatusSwitchId { get; set; }

            public Guid FrameworkIdentifier { get; set; }
        }

        #endregion

        #region Properties

        protected List<TreeDataItem> TreeData
        {
            get => (List<TreeDataItem>)ViewState[nameof(TreeData)];
            set => ViewState[nameof(TreeData)] = value;
        }

        private GlossaryHelper Glossary
        {
            get => (GlossaryHelper)ViewState[nameof(Glossary)];
            set => ViewState[nameof(Glossary)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.DataBinding += Repeater_DataBinding;
            Repeater.ItemCreated += Repeater_ItemCreated;
            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            TreeData = new List<TreeDataItem>();
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var outlineTreeUpdatePanel = (InSite.Common.Web.UI.UpdatePanel)e.Item.FindControl("OutlineTreeUpdatePanel");
            outlineTreeUpdatePanel.Request += OutlineTreeUpdatePanel_Request;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (FrameworkGroup)e.Item.DataItem;

            var progressGrid = (CompetencyProgressGrid)e.Item.FindControl("ProgressGrid");
            var hasCompetencies = progressGrid.LoadData(dataItem.JournalSetupId, dataItem.FrameworkId, dataItem.UserId);
            progressGrid.Visible = hasCompetencies;

            if (dataItem.ShowOutlineTree)
                TreeData.Add(new TreeDataItem
                {
                    TreeId = e.Item.FindControl("OutlineTree").ClientID,
                    TreePanelId = e.Item.FindControl("OutlineTreePanel").ClientID,
                    UpdatePanelId = e.Item.FindControl("OutlineTreeUpdatePanel").ClientID,
                    ExpandAllId = e.Item.FindControl("ExpandAllButton").ClientID,
                    CollapseAllId = e.Item.FindControl("CollapseAllButton").ClientID,
                    DisplayStatusSwitchId = e.Item.FindControl("DisplayStatusSwitch").ClientID,
                    FrameworkIdentifier = dataItem.FrameworkId
                });
            else
                TreeData.Add(null);
        }

        private void OutlineTreeUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "load")
            {
                var repeaterItem = (RepeaterItem)((System.Web.UI.Control)sender).NamingContainer;

                var treePanel = (Panel)repeaterItem.FindControl("OutlineTreePanel");
                if (treePanel.Visible)
                    return;

                treePanel.Visible = true;

                var dataItem = TreeData[repeaterItem.ItemIndex];
                if (dataItem == null)
                    return;

                var outlineTree = (OutlineTree)repeaterItem.FindControl("OutlineTree");
                outlineTree.Visible = true;
                outlineTree.LoadData(dataItem.FrameworkIdentifier, Glossary);

                OutlineTreeScript.TermsData = Glossary.GetJavaScriptDictionary();
                OutlineTreeScript.UpdateTerms();

                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(UserCompetencySummary),
                    "tree_loaded",
                    $"userCompetencySummary.outlineTreeLoaded({repeaterItem.ItemIndex});",
                    true);
            }
        }

        #endregion

        #region Data binding

        public void BindModelToControls(Guid user)
        {
            var data = new List<FrameworkGroup>();

            AddAssessmentCompetencies(user, data);
            AddLogbookCompetencies(user, data);

            Glossary = new GlossaryHelper(Identity.Language);

            for (var i = 0; i < data.Count; i++)
            {
                var framework = data[i];
                var entity = StandardSearch.BindFirst(
                    x => new
                    {
                        x.StandardIdentifier,
                        x.Icon
                    },
                    x => x.StandardIdentifier == framework.FrameworkId);

                if (entity == null)
                {
                    data[i] = null;
                    continue;
                }

                var content = ServiceLocator.ContentSearch.GetBlock(
                    entity.StandardIdentifier,
                    string.Empty,
                    new[] { ContentLabel.Title, ContentLabel.Summary });

                var title = Glossary.Process(
                    entity.StandardIdentifier,
                    ContentLabel.Title,
                    content.Title.GetText(Identity.Language));

                framework.FrameworkTitle = entity.Icon.IsNotEmpty()
                    ? $"<i class='fa {entity.Icon} me-2'></i> {title}"
                    : title;
                framework.FrameworkSummary = Glossary.Process(
                    entity.StandardIdentifier, ContentLabel.Title, content.Summary.GetHtml(Identity.Language));
            }

            Repeater.DataSource = data.Where(x => x != null).OrderBy(x => x.FrameworkTitle);
            Repeater.DataBind();

            OutlineTreeScript.TermsData = Glossary.GetJavaScriptDictionary();

            Visible = Repeater.Items.Count > 0;
        }

        private void AddAssessmentCompetencies(Guid user, List<FrameworkGroup> data)
        {
            var frameworks = StandardSearch.GetUserStandardFrameworks(Organization.Identifier, user);

            foreach (var frameworkId in frameworks)
            {
                data.Add(new FrameworkGroup
                {
                    FrameworkId = frameworkId,
                    ShowOutlineTree = true
                });
            }
        }

        private void AddLogbookCompetencies(Guid user, List<FrameworkGroup> data)
        {
            var list = ServiceLocator.JournalSearch.GetJournals(
                new QJournalFilter
                {
                    UserIdentifier = user,
                    OrganizationIdentifier = Organization.Identifier
                },
                x => x.JournalSetup.Framework);

            data.AddRange(
                list.Where(x => x.JournalSetup?.FrameworkStandardIdentifier != null)
                    .Select(x => new FrameworkGroup
                    {
                        UserId = user,
                        FrameworkId = x.JournalSetup.FrameworkStandardIdentifier.Value,
                        JournalSetupId = x.JournalSetupIdentifier
                    })
            );
        }

        #endregion
    }
}