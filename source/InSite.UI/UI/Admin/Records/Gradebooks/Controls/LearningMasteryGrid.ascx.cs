using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Records;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class LearningMasteryGrid : UserControl
    {
        private class StandardItem
        {
            public Guid StandardIdentifier { get; set; }
            public string StandardTitle { get; set; }
            public decimal? MasteryScore { get; set; }
        }

        private class StandardScore
        {
            public decimal? Score { get; set; }
            public bool IsMastery { get; set; }
        }

        private class StudentScores
        {
            public Guid StudentIdentifier { get; set; }
            public string StudentName { get; set; }

            public List<StandardScore> Scores { get; set; }
        }

        private class UserEntity
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StandardScoreRepeater.ItemDataBound += StandardScoreRepeater_ItemDataBound;
        }

        private void StandardScoreRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var studentScores = (StudentScores)e.Item.DataItem;

            var scoreRepeater = (Repeater)e.Item.FindControl("ScoreRepeater");
            scoreRepeater.DataSource = studentScores.Scores;
            scoreRepeater.DataBind();
        }

        public void LoadData(Guid? gradebookId)
        {
            var hasGradebook = gradebookId.HasValue;

            StandardScorePanel.Visible = hasGradebook;

            if (!hasGradebook)
            {
                NoScoreAlert.AddMessage(AlertType.Warning, "No Scores");
                return;
            }

            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(gradebookId.Value);
            var students = GetStudents(gradebook);
            var standards = GetStandards(gradebook);
            var scores = new List<StudentScores>();

            foreach (var student in students)
                scores.Add(GetStudentScores(gradebook, student, standards));

            StandardRepeater.DataSource = standards;
            StandardRepeater.DataBind();

            StandardScoreRepeater.DataSource = scores;
            StandardScoreRepeater.DataBind();
        }

        private StudentScores GetStudentScores(GradebookState gradebook, UserEntity user, List<StandardItem> standards)
        {
            var scores = new StudentScores
            {
                StudentIdentifier = user.UserIdentifier,
                StudentName = user.FullName,
                Scores = new List<StandardScore>()
            };

            foreach (var standard in standards)
            {
                var score = gradebook.ValidationScores.Find(x => x.User == user.UserIdentifier && x.Competency == standard.StandardIdentifier);

                scores.Scores.Add(new StandardScore
                {
                    Score = score?.Points,
                    IsMastery = score?.Points != null && standard.MasteryScore.HasValue && score.Points >= standard.MasteryScore.Value
                });
            }

            return scores;
        }

        private static IEnumerable<UserEntity> GetStudents(GradebookState gradebook)
        {
            if (gradebook.Enrollments.IsEmpty())
                return Enumerable.Empty<UserEntity>();

            var learners = UserSearch.Bind(
                x => new UserEntity
                {
                    UserIdentifier = x.UserIdentifier,
                    FullName = x.FullName
                },
                new UserFilter
                {
                    IncludeUserIdentifiers = gradebook.Enrollments.Select(x => x.Learner).Distinct().ToArray()
                });

            return learners.OrderBy(x => x.FullName).AsEnumerable();
        }

        private static List<StandardItem> GetStandards(GradebookState gradebook)
        {
            var list = new List<StandardItem>();

            if (gradebook.ValidationScores != null)
            {
                var standardIds = gradebook.ValidationScores
                    .Select(x => x.Competency)
                    .Distinct()
                    .ToList();

                foreach (var standardIdentifier in standardIds)
                {
                    var standard = StandardSearch.Select(standardIdentifier);

                    if (standard != null)
                    {
                        var item = new StandardItem
                        {
                            StandardIdentifier = standard.StandardIdentifier,
                            StandardTitle = standard.ContentTitle,
                            MasteryScore = standard.MasteryPoints
                        };

                        list.Add(item);
                    }
                }

                list.Sort((a, b) => a.StandardTitle.CompareTo(b.StandardTitle));
            }

            return list;
        }
    }
}