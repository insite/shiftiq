using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using UserEntity = InSite.Persistence.User;

namespace InSite.Admin.Records.Reports.Forms
{
    public partial class LearningMastery : AdminBasePage
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SelectedGradebook.AutoPostBack = true;
            SelectedGradebook.ValueChanged += SelectedGradebook_ValueChanged;

            StandardScoreRepeater.ItemDataBound += StandardScoreRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                InitGradebookList();

                PageHelper.AutoBindHeader(this);
            }
        }

        private void SelectedGradebook_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
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

        private void InitGradebookList()
        {
            var gradebooks = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                GradebookTypes = new[] { GradebookType.ScoresAndStandards.ToString() }
            });

            SelectedGradebook.LoadItems(gradebooks, "GradebookIdentifier", "GradebookTitle");

            var totalGradebooks = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter { OrganizationIdentifier = Organization.Identifier });

            var alertType = gradebooks.Count == 0 ? AlertType.Warning : AlertType.Success;

            var alertMessage = totalGradebooks == 0
                ? "There are no gradebooks"
                : (gradebooks.Count > 0
                    ? $"There are {totalGradebooks} gradebooks and {gradebooks.Count} gradebooks have outcomes."
                    : $"There are {totalGradebooks} gradebooks but no gradebooks with outcomes."
                );

            StatusAlert.AddMessage(alertType, alertMessage);

            GradebookPanel.Visible = gradebooks.Count > 0;
        }

        private void LoadData()
        {
            var gradebookId = SelectedGradebook.ValueAsGuid;

            StandardScorePanel.Visible = gradebookId.HasValue;

            if (gradebookId.HasValue)
            {
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
            else
            {
                NoScoreAlert.AddMessage(AlertType.Warning, "No Scores");
            }
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

        private static List<UserEntity> GetStudents(GradebookState gradebook)
        {
            var list = new List<UserEntity>();

            if (gradebook.Enrollments != null)
            {
                foreach (var student in gradebook.Enrollments)
                {
                    var user = UserSearch.Select(student.Learner);

                    if (user != null)
                        list.Add(user);
                }

                list.Sort((a, b) => a.FullName.CompareTo(b.FullName));
            }

            return list;
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
