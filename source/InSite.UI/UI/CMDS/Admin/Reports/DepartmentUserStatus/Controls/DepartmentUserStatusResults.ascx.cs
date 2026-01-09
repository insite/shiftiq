using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Custom.CMDS.Reports.Controls
{
    public partial class DepartmentUserStatusResults : SearchResultsGridViewController<TUserStatusFilter>
    {
        #region Classes

        private class DataItem
        {
            public DateTimeOffset AsAt { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public string OrganizationName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserName { get; set; }
            public string ItemName { get; set; }
            public string DisplayItemName { get; set; }
            public string ListDomain { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public string CountCP { get; set; }
            public string CountEX { get; set; }
            public string CountNC { get; set; }
            public string CountNA { get; set; }
            public string CountNT { get; set; }
            public string CountSA { get; set; }
            public string CountSV { get; set; }
            public string CountVA { get; set; }
            public string CountVN { get; set; }
            public string CountRQ { get; set; }
            public string CountOP { get; set; }
            public decimal? Score { get; set; }
            public decimal? Progress { get; set; }
        }

        #endregion

        #region Events

        public delegate void ZoomEventHandler(Guid department, Guid user, string itemName, Guid organization);

        public event ZoomEventHandler Zoom;

        #endregion

        #region Event handlers

        protected void ZoomIcon_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Zoom")
            {
                var args = e.CommandArgument.ToString().Split(',');

                Zoom(Guid.Parse(args[0]), Guid.Parse(args[1]), args[2].ToString(), Guid.Parse(args[3]));
            }
        }

        #endregion

        #region Data binding

        protected override int SelectCount(TUserStatusFilter filter)
        {
            return new TUserStatusSearch()
                .Count(filter);
        }

        protected override IListSource SelectData(TUserStatusFilter filter)
        {
            var itemDisplayMapping = Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code);

            return new TUserStatusSearch()
                .Select(filter)
                .Select(x => new DataItem
                {
                    AsAt = x.AsAt,
                    DepartmentIdentifier = x.DepartmentIdentifier,
                    DepartmentName = x.DepartmentName,
                    OrganizationName = x.OrganizationName,
                    UserIdentifier = x.UserIdentifier,
                    UserName = x.UserName,
                    ItemName = x.ItemName,
                    DisplayItemName = itemDisplayMapping.GetOrDefault(x.ItemName, x.ItemName),
                    ListDomain = x.ListDomain,
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    CountCP = GetCount(x.CountCP),
                    CountEX = GetCount(x.CountEX),
                    CountNC = GetCount(x.CountNC),
                    CountNA = GetCount(x.CountNA),
                    CountNT = GetCount(x.CountNT),
                    CountSA = GetCount(x.CountSA),
                    CountSV = GetCount(x.CountSV),
                    CountVA = GetCount(x.CountVA),
                    CountVN = GetCount(x.CountVN),
                    CountRQ = GetCount(x.CountRQ),
                    Score = x.Score,
                    Progress = x.CountRQ == 0 ? 0m : (x.CountVA + x.CountVN) / x.CountRQ
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Helper methods

        private string GetCount(int count)
        {
            return count != 0 ? count.ToString() : "-";
        }

        protected string GetComplianceScore(decimal? score)
        {
            if (score == null)
                return "NA <span class='text-body-secondary'><i class='fas fa-circle'></i></span>";

            if (score == 1)
                return $"{score:p0} <span class='text-success'><i class='fas fa-flag-checkered'></i></span>";

            if (0.5m <= score && score < 1.0m)
                return $"{score:p0} <span class='text-danger'><i class='fas fa-flag'></i></span>";

            return $"{score:p0} <span class='text-danger'><i class='far fa-flag'></i></span>";
        }

        #endregion
    }
}