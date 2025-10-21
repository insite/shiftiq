using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.UI.Admin.Events.Classes.Reports;

using Shift.Constant;

namespace InSite.Admin.Events.Classes.Reports
{
    public partial class ClassRegistrationsByGroupStatus : BaseReportControl
    {
        private class ClassItem
        {
            public string ClassTitle { get; set; }
            public int MemberCount { get; set; }
            public int NonMemberCount { get; set; }
            public int NoEmployerCount { get; set; }
            public int TotalCount => MemberCount + NonMemberCount + NoEmployerCount;
        }

        private class AchievementItem
        {
            public string AchievementTitle { get; set; }
            public List<ClassItem> Classes { get; set; }
            public int MemberCount => Classes.Sum(x => x.MemberCount);
            public int NonMemberCount => Classes.Sum(x => x.NonMemberCount);
            public int NoEmployerCount => Classes.Sum(x => x.NoEmployerCount);
            public int TotalCount => Classes.Sum(x => x.TotalCount);

            public decimal MemberPercent => TotalCount > 0
                ? (decimal)MemberCount / (decimal)TotalCount
                : 0;
        }

        public override string ReportTitle => "Class Registrations by Employer Status";

        public override string ReportFileName => "ClassRegistrationsByEmployerStatus";

        public override byte[] GetPdf(QEventFilter filter)
        {
            PageTitle.InnerText = ReportTitle;

            var achievements = GetAchievements(filter);

            AchievementRepeater.ItemDataBound += AchievementRepeater_ItemDataBound;
            AchievementRepeater.DataSource = achievements;
            AchievementRepeater.DataBind();

            var memberCount = achievements.Sum(x => x.MemberCount);
            var nonMemberCount = achievements.Sum(x => x.NonMemberCount);
            var totalCount = memberCount + nonMemberCount;
            var memberPercent = Math.Round((double)memberCount / (double)totalCount, 2);
            var nonMemberPercent = 1.0 - memberPercent;

            MemberPercent.Text = $"{memberPercent:p0}";
            MemberCount.Text = memberCount.ToString();
            NonMemberPercent.Text = $"{nonMemberPercent:p0}";
            NonMemberCount.Text = nonMemberCount.ToString();
            TotalCount.Text = totalCount.ToString();

            var criteriaItems = GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;

            return BuildPdf(PageOrientationType.Portrait, 1400, 980, ReportTitle);
        }

        public override byte[] GetXlsx(QEventFilter filter)
        {
            throw new NotImplementedException();
        }

        private List<AchievementItem> GetAchievements(QEventFilter filter)
        {
            var data = ServiceLocator.EventSearch.GetApprenticeSummary(filter);

            data.Sort((a, b) => string.Compare(a.EventTitle, b.EventTitle, true));

            var result = new List<AchievementItem>();

            foreach (var summary in data)
            {
                var achievement = result.Find(x => x.AchievementTitle == summary.AchievementTitle);

                if (achievement == null)
                {
                    achievement = new AchievementItem { AchievementTitle = summary.AchievementTitle, Classes = new List<ClassItem>() };
                    result.Add(achievement);
                }

                achievement.Classes.Add(new ClassItem
                {
                    ClassTitle = summary.EventTitle,
                    MemberCount = summary.MemberCount,
                    NoEmployerCount = summary.NoEmployerCount,
                    NonMemberCount = summary.TotalCount - summary.MemberCount - summary.NoEmployerCount
                });
            }

            result.Sort((a, b) => string.Compare(a.AchievementTitle, b.AchievementTitle, true));

            return result;
        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievement = (AchievementItem)e.Item.DataItem;

            var classRepeater = (Repeater)e.Item.FindControl("ClassRepeater");
            classRepeater.DataSource = achievement.Classes;
            classRepeater.DataBind();
        }
    }
}