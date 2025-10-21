using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class TrainingRecordsReport : UserControl
    {
        private class ScoreItem
        {
            public string ScoreName { get; set; }
            public string ScoreValue { get; set; }
            public string ScoreComment { get; set; }
        }

        private class LevelItem
        {
            public bool IsEmpty { get; set; }
            public string AchievementTitle { get; set; }
            public string EventTitle { get; set; }
            public string EventScheduledStart { get; set; }
            public string EventScheduledEnd { get; set; }

            public List<ScoreItem> Scores { get; set; }
        }

        private class TradeItem
        {
            public string AchievementDescription { get; set; }
            public List<LevelItem> Levels { get; set; }
        }

        private class CertificateItem
        {
            public bool IsEmpty { get; set; }
            public string AchievementTitle { get; set; }
            public string Granted { get; set; }
            public string ScoreValue { get; set; }
            public string ExpirationExpected { get; set; }

            public DateTimeOffset? ClassDate { get; set; }
            public string ClassInfo { get; set; }
        }

        private void TradeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var trade = (TradeItem)e.Item.DataItem;

            var levelRepeater = (Repeater)e.Item.FindControl("LevelRepeater");
            levelRepeater.ItemDataBound += LevelRepeater_ItemDataBound;
            levelRepeater.DataSource = trade.Levels;
            levelRepeater.DataBind();
        }

        private void LevelRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var level = (LevelItem)e.Item.DataItem;

            var scoreRepeater = (Repeater)e.Item.FindControl("ScoreRepeater");
            scoreRepeater.DataSource = level.Scores;
            scoreRepeater.DataBind();
        }

        public void LoadReport(QPerson person)
        {
            var serverUrl = Request.Url.Scheme + "://" + Request.Url.Host;

            //School Info
            var organizationLogo = CurrentSessionState.Identity.Organization.PlatformCustomization.PlatformUrl.Logo;
            var hasLogo = CurrentSessionState.Identity.Organization.PlatformCustomization.PlatformUrl.Logo.HasValue();

            OrganizationLogo.Visible = hasLogo;

            if (hasLogo)
            {
                if (organizationLogo.Contains("://"))
                {
                    OrganizationLogo.ImageUrl = organizationLogo;
                }
                else
                {
                    if (!organizationLogo.StartsWith("/"))
                        organizationLogo = "/" + organizationLogo;

                    OrganizationLogo.ImageUrl = $"{serverUrl}{organizationLogo}";
                }
            }

            OrganizationName.Text = CurrentSessionState.Identity.Organization.CompanyName;
            OrganizationName2.Text = OrganizationName.Text;
            OrganizationAddress.Text = LocationHelper.ToHtml(
                CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Street,
                null,
                CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.City,
                CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Province,
                CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.PostalCode,
                null, null, null);
            OrganizationPhone.Text = CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Phone;
            OrganizationEmail.Text = CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Email;

            //Apprentice Info
            ApprenticeFullName.Text = person.User.FullName;
            ApprenticeEmail.Text = person.User.Email.HasValue() ? person.User.Email.ToLower() : null;

            if (person.HomeAddress != null)
                ApprenticeAddress.Text = LocationHelper.ToHtml(
                    person.HomeAddress.Street1,
                    person.HomeAddress.Street2,
                    person.HomeAddress.City,
                    person.HomeAddress.Province,
                    person.HomeAddress.PostalCode,
                    null, null, null);
            else
                ApprenticeAddress.Text = "None";

            ApprenticePhone.Text = person.Phone.HasValue() ? person.Phone : "None";
            ApprenticeITA.Text = person.TradeworkerNumber.HasValue() ? person.TradeworkerNumber : "None";
            ApprenticeBirthDate.Text = person.Birthdate.HasValue ? $"{person.Birthdate:MMM d, yyyy}" : "None";

            var trades = LoadTrades(person.UserIdentifier);
            TradeRepeater.ItemDataBound += TradeRepeater_ItemDataBound;
            TradeRepeater.DataSource = trades;
            TradeRepeater.DataBind();

            var certificates = LoadCertificates(person.UserIdentifier);
            CertificateRepeater.DataSource = certificates;
            CertificateRepeater.DataBind();
        }

        private List<TradeItem> LoadTrades(Guid userIdentifier)
        {
            var data = ServiceLocator.AchievementSearch.GetContactSummaryNoExpiryReport(userIdentifier, CurrentSessionState.Identity.Organization.Identifier);

            var result = data
                .GroupBy(x => x.AchievementDescription)
                .OrderBy(x => x.Key)
                .Select(a => new TradeItem
                {
                    AchievementDescription = a.Key,
                    Levels = a.GroupBy(x => new
                    {
                        AchievementTitle = x.AchievementTitle,
                        EventTitle = x.EventTitle,
                        EventScheduledStart = x.EventScheduledStart,
                        EventScheduledEnd = x.EventScheduledEnd
                    })
                    .OrderBy(x => x.Key.EventScheduledStart)
                    .Select(b => new LevelItem
                    {
                        AchievementTitle = b.Key.AchievementTitle,
                        EventTitle = b.Key.EventTitle,
                        EventScheduledStart = b.Key.EventScheduledStart.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone),
                        EventScheduledEnd = b.Key.EventScheduledEnd.HasValue ? b.Key.EventScheduledEnd.Value.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone) : "N/A",

                        Scores = b.OrderBy(x => x.ScoreSequence).Select(c => new ScoreItem
                        {
                            ScoreName = c.ScoreName,
                            ScoreValue = GetScoreValue(c.ScoreType, c.ScoreNumber, c.ScoreText, c.ScorePercent, c.ScorePoint),
                            ScoreComment = c.ScoreComment
                        })
                        .ToList()
                    })
                    .ToList()
                })
                .ToList();

            foreach (var trade in result)
            {
                if (trade.Levels.Count % 2 == 1)
                    trade.Levels.Add(new LevelItem { IsEmpty = true });
            }

            return result;
        }

        private List<CertificateItem> LoadCertificates(Guid userIdentifier)
        {
            var data = ServiceLocator.AchievementSearch.GetContactSummaryWithExpiryReport(userIdentifier, CurrentSessionState.Identity.Organization.Identifier);

            var result = data
                .Select(x => new CertificateItem
                {
                    AchievementTitle = x.AchievementTitle,
                    Granted = x.Granted.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone),
                    ScoreValue = GetScoreValue(x.ScoreType, x.ScoreNumber, x.ScoreText, x.ScorePercent, x.ScorePoint),
                    ExpirationExpected = x.ExpirationExpected.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone),
                    ClassDate = x.EventScheduledStart
                })
                .OrderBy(x => x.Granted)
                .ToList();

            foreach (var item in result)
                if (item.ClassDate.HasValue)
                    item.ClassInfo = TimeZones.FormatDateOnly(item.ClassDate.Value, CurrentSessionState.Identity.User.TimeZone);

            if (result.Count > 0 && result.Count % 2 == 1)
                result.Add(new CertificateItem { IsEmpty = true });

            return result;
        }

        private static string GetScoreValue(string scoreType, decimal? scoreNumber, string scoreText, decimal? scorePercent, decimal? scorePoint)
        {
            if (scoreNumber.HasValue)
                return $"{scoreNumber:n2}";

            if (!string.IsNullOrEmpty(scoreText))
                return scoreText;

            if (scorePercent.HasValue)
                return $"{scorePercent * 100:n2}%";

            if (scoreType != GradeItemType.Calculation.ToString() && scoreType != GradeItemType.Category.ToString() && scorePoint.HasValue)
                return $"{scorePoint:n2}";

            return null;
        }
    }
}