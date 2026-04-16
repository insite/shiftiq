using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.UI.Portal.Learning.Controls
{
    public partial class PlanAchievementTypeRepeater : BaseUserControl
    {
        public class DataSource
        {
            public int ValidItemsCount { get; }
            public int InvalidItemsCount { get; }
            public IList<PolicyItem> Items { get; }
            public int TotalItemsCount => ValidItemsCount + InvalidItemsCount;

            public DataSource(int validCount, int invalidCount, IList<PolicyItem> items)
            {
                ValidItemsCount = validCount;
                InvalidItemsCount = invalidCount;
                Items = items;
            }
        }

        public class PolicyItem
        {
            public string Type { get; }
            public IList<VCmdsCredentialAndExperience> Credentials { get; } = new List<VCmdsCredentialAndExperience>();

            public PolicyItem(string type)
            {
                Type = type;
            }
        }

        #region Properties

        public Guid? SelectedCredentialId { get; set; }

        #endregion

        #region Fields

        private Func<Guid?, string> _getUrl;

        #endregion

        #region Intialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TypeRepeater.ItemDataBound += TypeRepeater_ItemDataBound;
            TypeRepeater.ItemCreated += TypeRepeater_ItemCreated;
        }

        public void LoadData(IEnumerable<PolicyItem> data, Func<Guid?, string> getUrl)
        {
            _getUrl = getUrl;

            TypeRepeater.DataSource = data;
            TypeRepeater.DataBind();
        }

        public static DataSource GetDataSource(IEnumerable<VCmdsCredentialAndExperience> credentials, bool showInvalidOnly)
        {
            if (LearningHelper.ShowSafetyAchievementsOnly())
                credentials = credentials
                    .Where(x => x.AchievementLabel == "Time-Sensitive Safety Certificate");

            var validCredentials = new List<VCmdsCredentialAndExperience>();
            var invalidCredentials = new List<VCmdsCredentialAndExperience>();

            foreach (var c in credentials)
            {
                if (c.CredentialStatus == "Valid")
                    validCredentials.Add(c);
                else
                    invalidCredentials.Add(c);
            }

            var items = showInvalidOnly
                ? CreateList(invalidCredentials)
                : CreateList(credentials);

            return new DataSource(validCredentials.Count, invalidCredentials.Count, items);
        }

        private static IList<PolicyItem> CreateList(IEnumerable<VCmdsCredentialAndExperience> data)
        {
            var result = new List<PolicyItem>();
            var index = new Dictionary<string, PolicyItem>();

            foreach (var row in data)
            {
                var type = row.AchievementLabel.IfNullOrEmpty("N/A");

                if (!index.TryGetValue(type, out var item))
                {
                    result.Add(item = new PolicyItem(type));
                    index.Add(type, item);
                }

                item.Credentials.Add(row);
            }

            return result;
        }

        #endregion

        #region Event handlers

        private static void TypeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var policyItem = (PolicyItem)e.Item.DataItem;

            var repeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            repeater.DataSource = policyItem.Credentials;
            repeater.DataBind();
        }

        private void TypeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("AchievementRepeater");

            repeater.ItemDataBound += AchievementRepeater_ItemDataBound;
        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = (VCmdsCredentialAndExperience)e.Item.DataItem;

            var tableRow = (HtmlTableRow)e.Item.FindControl("Row");
            var achievementLink = (HyperLink)e.Item.FindControl("AchievementLink");
            var warningPanel = e.Item.FindControl("WarningPanel");

            UI.CMDS.Portal.Achievements.Credentials.SearchResults.BindDataItem(e.Item, row, false);

            achievementLink.NavigateUrl = _getUrl(row.CredentialIdentifier);

            if (SelectedCredentialId == row.CredentialIdentifier)
                tableRow.Attributes["class"] = "selected";

            warningPanel.Visible = row.CredentialStatus == "Valid" && row.CredentialGranted == null;
        }

        #endregion

        #region Helpers

        protected string GetAchievementTypeDisplay(string type)
        {
            var organization = Organization.Code;
            return Shift.Common.AchievementTypes.Pluralize(type, organization);
        }

        #endregion;
    }
}