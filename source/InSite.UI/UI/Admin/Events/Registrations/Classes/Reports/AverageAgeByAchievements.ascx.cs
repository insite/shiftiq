using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;

namespace InSite.Admin.Events.Registrations.Reports
{
    public partial class AverageAgeByAchievements : UserControl
    {
        private class RegistrationItem
        {
            public int Number { get; set; }
            public string ClassTitle { get; set; }
            public string UserFullName { get; set; }
            public DateTime? BirthDate { get; set; }
            public int? Age { get; set; }
        }

        private class AchievementItem
        {
            public string AchievementTitle { get; set; }
            public List<RegistrationItem> Registrations { get; set; }
            public int RegistrationCount => Registrations.Count;

            public int? AverageAge
            {
                get
                {
                    var ages = Registrations.Where(x => x.Age.HasValue).ToList();

                    return ages.Count > 0
                        ? (int)Math.Round(ages.Sum(x => (double)x.Age.Value) / ages.Count)
                        : (int?)null;
                }
            }
        }

        public void LoadReport(QRegistrationFilter filter)
        {
            PageTitle.InnerText = "Average Age by Achievements";

            var achievements = GetAchievements(filter);

            AchievementRepeater.ItemDataBound += AchievementRepeater_ItemDataBound;
            AchievementRepeater.DataSource = achievements;
            AchievementRepeater.DataBind();

            var criteriaItems = RegistrationCriteriaHelper.GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;
        }

        private List<AchievementItem> GetAchievements(QRegistrationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            return ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Event, x => x.Candidate)
                .GroupBy(x => x.Event.Achievement?.AchievementTitle)
                .Select(x =>
                {
                    var registration = x.First();
                    var number = 1;

                    return new AchievementItem
                    {
                        AchievementTitle = registration.Event.Achievement?.AchievementTitle ?? "N/A",
                        Registrations = x
                            .OrderBy(y => y.Event.EventTitle)
                            .ThenBy(y => y.Candidate.UserFullName)
                            .Select(y => new RegistrationItem
                            {
                                Number = number++,
                                ClassTitle = y.Event.EventTitle,
                                UserFullName = y.Candidate.UserFullName,
                                BirthDate = y.Candidate.Birthdate.HasValue ? y.Candidate.Birthdate.Value.Date : (DateTime?)null,
                                Age = y.Candidate.Birthdate.HasValue
                                    ? GetAge(y.Candidate.Birthdate.Value, registration.Event.EventScheduledStart.Date)
                                    : (int?)null
                            }).ToList()
                    };
                })
                .OrderBy(x => x.AchievementTitle)
                .ToList();
        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var achievement = (AchievementItem)e.Item.DataItem;

            var registrationRepeater = (Repeater)e.Item.FindControl("RegistrationRepeater");
            registrationRepeater.DataSource = achievement.Registrations;
            registrationRepeater.DataBind();
        }

        private static int GetAge(DateTime birthdate, DateTime current)
        {
            var age = current.Year - birthdate.Year;

            if (current.Month < birthdate.Month || current.Month == birthdate.Month && current.Day < birthdate.Day)
                age--;

            return age;
        }
    }
}