using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;

using Shift.Common;

namespace InSite.Admin.Records.Reports.Gradebooks.Controls
{
    public partial class StudentMarksReport : UserControl
    {
        private class ScoreRepeaterItem
        {
            public Guid Key { get; set; }
            public string ScoreItemName { get; set; }
            public string Comment { get; set; }
            public string ScoreValue { get; set; }
            public int Level { get; set; }
            public string SpecialCssClass { get; set; }
        }

        private class MasteryItem
        {
            public string StandardTitle { get; set; }
            public decimal? MasteryScore { get; set; }
            public decimal? Score { get; set; }

            public bool IsMastery => MasteryScore <= Score;
        }

        public void LoadReport(User user, Person person, QGradebook gradebook, GradebookState data)
        {
            var serverUrl = Request.Url.Scheme + "://" + Request.Url.Host;

            ClassInfo.Visible = gradebook.Event != null;
            GradebookInfo.Visible = gradebook.Event == null;

            if (gradebook.Event != null)
            {
                ClassName.Text = gradebook.Event.EventTitle;
                ClassStartDate.Text = $"{gradebook.Event.EventScheduledStart:dd-MMM-yyyy}";
                ClassEndDate.Text = $"{gradebook.Event.EventScheduledEnd:dd-MMM-yyyy}";
            }
            else
                GradebookName.Text = gradebook.GradebookTitle;

            StudentFullName.Text = user.FullName;

            if (!string.IsNullOrEmpty(person?.PersonCode))
                StudentITANumber.Text = $@"&nbsp;&nbsp; # {person.PersonCode}";

            if (person?.HomeAddress != null)
                StudentAddress.Text = ClassVenueAddressInfo.GetAddress(person.HomeAddress);

            if (StudentAddress.Text == "" && person?.ShippingAddress != null)
                StudentAddress.Text = ClassVenueAddressInfo.GetAddress(person.ShippingAddress);

            if (person?.EmployerGroup != null)
                Employer.Text = person.EmployerGroup.GroupName.HasValue() ? person.EmployerGroup.GroupName : "None";

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

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(
                new QProgressFilter { GradebookIdentifier = gradebook.GradebookIdentifier },
                null, null, x => x.GradeItem);

            BindFinalRepeater(user.UserIdentifier, data, progresses);
            BindScoreRepeater(user.UserIdentifier, data, progresses);
            BindMasteryRepeater(user.UserIdentifier, data);
        }

        private void BindFinalRepeater(Guid user, GradebookState data, List<QProgress> progresses)
        {
            var list = data.RootItems
                .Where(x => x.IsReported)
                .Select(x =>
                {
                    var score = progresses.Find(y => y.GradeItemIdentifier == x.Identifier && y.UserIdentifier == user);

                    return new
                    {
                        ScoreItemName = x.Name,
                        Comment = score?.ProgressComment,
                        ScoreValue = GradebookHelper.GetScoreValue(score, x),
                        CssClass = x.Name.StartsWith("ITA ") || x.Name.StartsWith("Certificate of Qualification Exam")
                            ? "ita-item"
                            : ""
                    };
                });

            FinalRepeater.DataSource = list;
            FinalRepeater.DataBind();
        }

        private void BindScoreRepeater(Guid user, GradebookState data, List<QProgress> progresses)
        {
            var list = new List<ScoreRepeaterItem>();

            foreach (var rootItem in data.RootItems)
                AddItems(user, data, list, rootItem.Children, 0, progresses);

            ScoreRepeater.Visible = list.Count > 0;
            ScoreRepeater.DataSource = list;
            ScoreRepeater.DataBind();
        }

        private void AddItems(Guid user, GradebookState data, List<ScoreRepeaterItem> output, List<GradeItem> input, int level, List<QProgress> progresses)
        {
            var filtered = input != null ? input.Where(x => x.IsReported).ToList() : null;
            if (filtered.IsEmpty())
                return;

            foreach (var inputItem in filtered)
            {
                var score = progresses.Find(x => x.GradeItemIdentifier == inputItem.Identifier && x.UserIdentifier == user);

                var outputItem = new ScoreRepeaterItem
                {
                    Key = inputItem.Identifier,
                    ScoreItemName = inputItem.Name,
                    Comment = score?.ProgressComment,
                    ScoreValue = GradebookHelper.GetScoreValue(score, inputItem),
                    Level = level,
                    SpecialCssClass = level == 1 ? "special-score" : ""
                };

                output.Add(outputItem);

                AddItems(user, data, output, inputItem.Children, level + 1, progresses);
            }
        }

        private void BindMasteryRepeater(Guid userIdentifier, GradebookState gradebook)
        {
            var masteryItems = GetMasteryItems(userIdentifier, gradebook);

            MasteryRepeater.Visible = masteryItems != null && masteryItems.Count > 0;
            MasteryRepeater.DataSource = masteryItems;
            MasteryRepeater.DataBind();
        }

        private static List<MasteryItem> GetMasteryItems(Guid userIdentifier, GradebookState gradebook)
        {
            if (gradebook.ValidationScores == null || gradebook.ValidationScores.Count == 0)
                return null;

            var list = new List<MasteryItem>();

            var studentScores = gradebook.ValidationScores
                .Where(x => x.User == userIdentifier)
                .ToList();

            foreach (var validationScore in studentScores)
            {
                var standard = StandardSearch.Select(validationScore.Competency);
                if (standard == null)
                    continue;

                var item = new MasteryItem
                {
                    StandardTitle = standard.ContentTitle,
                    MasteryScore = standard.MasteryPoints,
                    Score = validationScore.Points
                };

                list.Add(item);
            }

            list.Sort((a, b) => a.StandardTitle.CompareTo(b.StandardTitle));

            return list;
        }
    }
}