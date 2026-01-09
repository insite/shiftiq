using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Classes.Reports
{
    public partial class ScoresReport : UserControl
    {
        private class ScoreHeader
        {
            public Guid ItemKey { get; set; }
            public string ItemName { get; set; }
        }

        private class Student
        {
            public string StudentName { get; set; }
            public string RegisteredDate { get; set; }
            public string Approval { get; set; }
            public string PersonCode { get; set; }
            public string[] Scores { get; set; }
        }

        private class DataSource
        {
            public List<ScoreHeader> ScoreHeaders { get; set; }
            public List<Student> Students { get; set; }
        }

        private DataSource _dataSource;
        public int StudentCount => _dataSource?.Students?.Count ?? 0;

        public void LoadReport(QEvent @event)
        {
            PageTitle.InnerText = "Scores Report";

            var user = CurrentSessionState.Identity.User;

            ClassTitle.Text = @event.EventTitle;
            ClassStartDate.Text = @event.EventScheduledStart.FormatDateOnly(user.TimeZone);
            ClassEndDate.Text = @event.EventScheduledEnd.FormatDateOnly(user.TimeZone);

            LoadDataSource(@event);

            if (_dataSource != null)
            {
                ScoreHeaderRepeater.DataSource = _dataSource.ScoreHeaders;
                ScoreHeaderRepeater.DataBind();

                StudentRepeater.DataSource = _dataSource.Students;
                StudentRepeater.ItemDataBound += StudentRepeater_ItemDataBound;
                StudentRepeater.DataBind();
            }
        }

        public byte[] GetXlsx(QEvent @event)
        {
            var user = CurrentSessionState.Identity.User;

            LoadDataSource(@event);

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var normalCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };

            var sheet = new XlsxWorksheet("Scores Report");

            string title = $"Class Scores Report \n" +
                $"{@event.EventTitle} \n" +
                $"{@event.EventScheduledStart.FormatDateOnly(user.TimeZone)} - {@event.EventScheduledEnd.FormatDateOnly(user.TimeZone)}";

            sheet.Rows[0].Height = 45;
            sheet.Cells.Add(new XlsxCell(0, 0, 2 + _dataSource.ScoreHeaders.Count) { Value = title, Style = boldCenterStyle });

            string summary = $"Total Number: {_dataSource.Students.Count} Students";

            sheet.Cells.Add(new XlsxCell(0, 2 + _dataSource.Students.Count, 2 + _dataSource.ScoreHeaders.Count) { Value = summary, Style = boldStyle });

            sheet.Columns[0].Width = 40;
            sheet.Columns[1].Width = 20;
            sheet.Columns[2].Width = 20;
            sheet.Columns[3].Width = 20;

            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "Name", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(1, 1) { Value = "Registered", Style = boldCenterStyle });
            sheet.Cells.Add(new XlsxCell(2, 1) { Value = "Approval", Style = boldCenterStyle });
            sheet.Cells.Add(new XlsxCell(3, 1) { Value = "ID #", Style = boldCenterStyle });

            for (int i = 0; i < _dataSource.Students.Count; i++)
            {
                sheet.Cells.Add(new XlsxCell(0, i + 2) { Value = _dataSource.Students[i].StudentName, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(1, i + 2) { Value = _dataSource.Students[i].RegisteredDate, Style = normalCenterStyle });
                sheet.Cells.Add(new XlsxCell(2, i + 2) { Value = _dataSource.Students[i].Approval, Style = normalCenterStyle });
                sheet.Cells.Add(new XlsxCell(3, i + 2) { Value = _dataSource.Students[i].PersonCode, Style = normalCenterStyle });
            }

            for (int i = 0; i < _dataSource.ScoreHeaders.Count; i++)
            {
                sheet.Columns[i + 4].Width = _dataSource.ScoreHeaders[i].ItemName.Length * 2;
                sheet.Cells.Add(new XlsxCell(i + 4, 1) { Value = _dataSource.ScoreHeaders[i].ItemName, Style = boldCenterStyle });

                for (int j = 0; j < _dataSource.Students.Count; j++)
                    sheet.Cells.Add(new XlsxCell(i + 4, j + 2) { Value = _dataSource.Students[j].Scores[i], Style = normalCenterStyle });
            }

            return XlsxWorksheet.GetBytes(sheet);
        }

        private void StudentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var student = (Student)e.Item.DataItem;
            var scoreRepeater = (Repeater)e.Item.FindControl("ScoreRepeater");

            scoreRepeater.DataSource = student.Scores;
            scoreRepeater.DataBind();
        }

        private DataSource LoadDataSource(QEvent @event)
        {
            var result = new DataSource { ScoreHeaders = new List<ScoreHeader>(), Students = new List<Student>() };
            var gradebook = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter { PrimaryEventIdentifier = @event.EventIdentifier }).FirstOrDefault();

            if (gradebook == null)
            {
                _dataSource = result;
                return result;
            }

            var data = ServiceLocator.RecordSearch.GetGradebookState(gradebook.GradebookIdentifier);
            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(
                new QProgressFilter { GradebookIdentifier = gradebook.GradebookIdentifier },
                null,
                null,
                x => x.GradeItem
            );

            AddScoreHeaders(data.RootItems, result.ScoreHeaders, 0, 2);

            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            foreach (var registration in @event.Registrations)
            {
                if (data.ContainsLearner(registration.CandidateIdentifier))
                {
                    var user = ServiceLocator.UserSearch.GetUser(registration.CandidateIdentifier);
                    var person = ServiceLocator.PersonSearch.GetPerson(registration.CandidateIdentifier, @event.OrganizationIdentifier);

                    var student = new Student
                    {
                        StudentName = user?.FullName ?? "",
                        RegisteredDate = registration.RegistrationRequestedOn.HasValue ? registration.RegistrationRequestedOn.FormatDateOnly(timeZone) : "N/A",
                        Approval = registration.ApprovalStatus,
                        PersonCode = person?.PersonCode,
                        Scores = new string[result.ScoreHeaders.Count]
                    };

                    for (int i = 0; i < result.ScoreHeaders.Count; i++)
                    {
                        var scoreHeader = result.ScoreHeaders[i];
                        var score = progresses.Find(x => x.UserIdentifier == registration.CandidateIdentifier && x.GradeItemIdentifier == scoreHeader.ItemKey);
                        var item = data.FindItem(scoreHeader.ItemKey);

                        student.Scores[i] = GradebookHelper.GetScoreValue(score, item, false);
                    }

                    result.Students.Add(student);
                }
            }

            result.Students.Sort((a, b) => a.StudentName.CompareTo(b.StudentName));

            _dataSource = result;

            return result;
        }

        private void AddScoreHeaders(List<GradeItem> items, List<ScoreHeader> scoreHeaders, int level, int maxLevel)
        {
            foreach (var item in items)
            {
                if (item.IsReported)
                {
                    scoreHeaders.Add(new ScoreHeader
                    {
                        ItemKey = item.Identifier,
                        ItemName = !string.IsNullOrEmpty(item.ShortName) ? item.ShortName : item.Abbreviation
                    });
                }
            }

            if (level < maxLevel)
            {
                foreach (var item in items)
                {
                    if (item.IsReported)
                        AddScoreHeaders(item.Children, scoreHeaders, level + 1, maxLevel);
                }
            }
        }
    }
}