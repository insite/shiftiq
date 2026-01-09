using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Reports
{
    static class EntrySummaryReport
    {
        #region Classes

        class EntryItem
        {
            public string GAC { get; set; }
            public string Competency { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Employer { get; set; }
            public string Supervisor { get; set; }
            public string TrainingLocation { get; set; }
            public string Class { get; set; }
            public int EntryNumber { get; set; }
            public string Validated { get; set; }
            public decimal EntryHours { get; set; }
            public decimal CompetencyHours { get; set; }
            public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; set; }
            public int? SkillRating { get; set; }
        }

        class LogbookItem
        {
            public string Name { get; set; }
            public List<EntryItem> Entries { get; set; }

            public decimal SumEntryHours()
            {
                if (Entries.Count == 0)
                    return 0;

                return Entries
                    .GroupBy(x => x.EntryNumber)
                    .Select(x => x.First().EntryHours)
                    .Sum(x => x);
            }

            public decimal SumCompetencyHours()
                => Entries.Count > 0 ? Entries.Sum(x => x.CompetencyHours) : 0;

            public ExperienceCompetencySatisfactionLevel MaxSatisfactionLevel()
            {
                return Entries.Count > 0
                    ? (ExperienceCompetencySatisfactionLevel)Entries.Max(x => (int)x.SatisfactionLevel)
                    : ExperienceCompetencySatisfactionLevel.None;
            }

            public int MaxSkillRating()
                => Entries.Count > 0 ? Entries.Sum(x => x.SkillRating ?? 0) : 0;
        }

        class UserItem
        {
            public string Name { get; set; }
            public List<LogbookItem> Logbooks { get; set; }

            public decimal SumEntryHours()
                => Logbooks.Count > 0 ? Logbooks.Sum(x => x.SumEntryHours()) : 0;

            public decimal SumCompetencyHours()
                => Logbooks.Count > 0 ? Logbooks.Sum(x => x.SumCompetencyHours()) : 0;

            public ExperienceCompetencySatisfactionLevel MaxSatisfactionLevel()
            {
                return Logbooks.Count > 0
                    ? (ExperienceCompetencySatisfactionLevel)Logbooks.Max(x => (int)x.MaxSatisfactionLevel())
                    : ExperienceCompetencySatisfactionLevel.None;
            }

            public int MaxSkillRating()
                => Logbooks.Count > 0 ? Logbooks.Sum(x => x.MaxSkillRating()) : 0;
        }

        #endregion

        #region Constants

        static readonly XlsxCellStyle HeaderStyle;
        static readonly XlsxCellStyle HeaderCenterStyle;
        static readonly XlsxCellStyle NormalStyle;
        static readonly XlsxCellStyle CenterStyle;
        static readonly XlsxCellStyle NormalBorderStyle;
        static readonly XlsxCellStyle CenterBorderStyle;
        static readonly XlsxCellStyle BoldCenterStyle;

        const string HoursFormat = "####0.00";
        const string DateFormat = "MMM d, yyyy";

        static EntrySummaryReport()
        {
            var borderStyle = XlsxBorderStyle.Thin;
            var borderColor = Color.Black;

            HeaderStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            HeaderStyle.Border.BorderAround(XlsxBorderStyle.None);
            HeaderStyle.Border.Bottom.Style = borderStyle;
            HeaderStyle.Border.Bottom.Color = borderColor;

            HeaderCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
            HeaderCenterStyle.Border.BorderAround(XlsxBorderStyle.None);
            HeaderCenterStyle.Border.Bottom.Style = borderStyle;
            HeaderCenterStyle.Border.Bottom.Color = borderColor;

            NormalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            NormalStyle.Border.BorderAround(XlsxBorderStyle.None);

            CenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };
            CenterStyle.Border.BorderAround(XlsxBorderStyle.None);

            NormalBorderStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            NormalBorderStyle.Border.BorderAround(XlsxBorderStyle.None);
            NormalBorderStyle.Border.Bottom.Style = borderStyle;
            NormalBorderStyle.Border.Bottom.Color = borderColor;

            CenterBorderStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };
            CenterBorderStyle.Border.BorderAround(XlsxBorderStyle.None);
            CenterBorderStyle.Border.Bottom.Style = borderStyle;
            CenterBorderStyle.Border.Bottom.Color = borderColor;

            BoldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
            BoldCenterStyle.Border.BorderAround(XlsxBorderStyle.None);
        }

        #endregion

        #region Create worksheet

        public static byte[] Create(QExperienceFilter filter)
        {
            var data = GetData(filter);
            var sheet = CreateWorksheet(data);

            return sheet.GetBytes();
        }

        private static XlsxWorksheet CreateWorksheet(List<UserItem> data)
        {
            var sheet = new XlsxWorksheet("Entries Summary");

            AddWorksheetHeader(sheet);
            AddWorksheetData(sheet, data);

            return sheet;
        }

        private static void AddWorksheetData(XlsxWorksheet sheet, List<UserItem> data)
        {
            var row = 1;

            foreach (var userItem in data)
            {
                sheet.Cells.Add(new XlsxCell(0, row) { Value = userItem.Name, Style = NormalStyle });
                sheet.Cells.Add(new XlsxCell(13, row) { Value = userItem.SumEntryHours(), Style = BoldCenterStyle, Format = HoursFormat });
                sheet.Cells.Add(new XlsxCell(14, row) { Value = userItem.SumCompetencyHours(), Style = BoldCenterStyle, Format = HoursFormat });
                sheet.Cells.Add(new XlsxCell(15, row) { Value = userItem.MaxSatisfactionLevel().ToString(), Style = BoldCenterStyle });
                sheet.Cells.Add(new XlsxCell(16, row) { Value = userItem.MaxSkillRating(), Style = BoldCenterStyle });

                row++;

                foreach (var logbookItem in userItem.Logbooks)
                {
                    sheet.Cells.Add(new XlsxCell(1, row) { Value = logbookItem.Name, Style = NormalStyle });
                    sheet.Cells.Add(new XlsxCell(13, row) { Value = logbookItem.SumEntryHours(), Style = BoldCenterStyle, Format = HoursFormat });
                    sheet.Cells.Add(new XlsxCell(14, row) { Value = logbookItem.SumCompetencyHours(), Style = BoldCenterStyle, Format = HoursFormat });
                    sheet.Cells.Add(new XlsxCell(15, row) { Value = logbookItem.MaxSatisfactionLevel().ToString(), Style = BoldCenterStyle });
                    sheet.Cells.Add(new XlsxCell(16, row) { Value = logbookItem.MaxSkillRating(), Style = BoldCenterStyle });

                    row++;

                    row = AddEntryDataCells(sheet, logbookItem.Entries, row);
                }
            }
        }

        private static int AddEntryDataCells(XlsxWorksheet sheet, List<EntryItem> data, int row)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var entryItem = data[i];

                var isLast = i == data.Count - 1;
                var normalStyle = isLast ? NormalBorderStyle : NormalStyle;
                var centerStyle = isLast ? CenterBorderStyle : CenterStyle;

                if (isLast)
                {
                    sheet.Cells.Add(new XlsxCell(0, row) { Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(1, row) { Style = normalStyle });
                }

                sheet.Cells.Add(new XlsxCell(2, row) { Value = entryItem.GAC, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = entryItem.Competency, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(4, row) { Value = entryItem.CreatedDate, Style = normalStyle, Format = DateFormat });
                sheet.Cells.Add(new XlsxCell(5, row) { Value = entryItem.StartDate, Style = normalStyle, Format = DateFormat });
                sheet.Cells.Add(new XlsxCell(6, row) { Value = entryItem.EndDate, Style = normalStyle, Format = DateFormat });
                sheet.Cells.Add(new XlsxCell(7, row) { Value = entryItem.Employer, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(8, row) { Value = entryItem.Supervisor, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(9, row) { Value = entryItem.TrainingLocation, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(10, row) { Value = entryItem.Class, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(11, row) { Value = entryItem.EntryNumber, Style = centerStyle });
                sheet.Cells.Add(new XlsxCell(12, row) { Value = entryItem.Validated, Style = centerStyle });
                sheet.Cells.Add(new XlsxCell(13, row) { Value = entryItem.EntryHours, Style = centerStyle, Format = HoursFormat });
                sheet.Cells.Add(new XlsxCell(14, row) { Value = entryItem.CompetencyHours, Style = centerStyle, Format = HoursFormat });
                sheet.Cells.Add(new XlsxCell(15, row) { Value = entryItem.SatisfactionLevel, Style = centerStyle });
                sheet.Cells.Add(new XlsxCell(16, row) { Value = entryItem.SkillRating, Style = centerStyle });

                row++;
            }

            return row;
        }

        private static void AddWorksheetHeader(XlsxWorksheet sheet)
        {
            const int row = 0;

            sheet.Columns[0].Width = 20; // User
            sheet.Columns[1].Width = 20; // Logbook
            sheet.Columns[2].Width = 30; // GAC
            sheet.Columns[3].Width = 30; // Competency
            sheet.Columns[4].Width = 15; // Created
            sheet.Columns[5].Width = 15; // Start Date
            sheet.Columns[6].Width = 15; // End Date
            sheet.Columns[7].Width = 20; // Employer
            sheet.Columns[8].Width = 20; // Supervisor
            sheet.Columns[9].Width = 20; // Training Location
            sheet.Columns[10].Width = 20; // Class
            sheet.Columns[11].Width = 10; // Entry #
            sheet.Columns[12].Width = 13; // Validated ?
            sheet.Columns[13].Width = 13; // Entry Hours
            sheet.Columns[14].Width = 20; // Competency Hours
            sheet.Columns[15].Width = 20; // Satisfaction Level
            sheet.Columns[16].Width = 12; // Skill Rating

            sheet.Cells.Add(new XlsxCell(0, row) { Value = "User", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(1, row) { Value = "Logbook", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(2, row) { Value = "GAC", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(3, row) { Value = "Competency", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(4, row) { Value = "Created", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(5, row) { Value = "Start Date", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(6, row) { Value = "End Date", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(7, row) { Value = "Employer", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(8, row) { Value = "Supervisor", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(9, row) { Value = "Training Location", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(10, row) { Value = "Class", Style = HeaderStyle });
            sheet.Cells.Add(new XlsxCell(11, row) { Value = "Entry #", Style = HeaderCenterStyle });
            sheet.Cells.Add(new XlsxCell(12, row) { Value = "Validated ?", Style = HeaderCenterStyle });
            sheet.Cells.Add(new XlsxCell(13, row) { Value = "Entry Hours", Style = HeaderCenterStyle });
            sheet.Cells.Add(new XlsxCell(14, row) { Value = "Competency Hours", Style = HeaderCenterStyle });
            sheet.Cells.Add(new XlsxCell(15, row) { Value = "Satisfaction Level", Style = HeaderCenterStyle });
            sheet.Cells.Add(new XlsxCell(16, row) { Value = "Skill Rating", Style = HeaderCenterStyle });
        }

        #endregion

        #region Get data from the database

        private static List<UserItem> GetData(QExperienceFilter filter)
        {
            var records = ServiceLocator.JournalSearch.GetEntrySummary(filter);
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            var data = records
                .GroupBy(x => new { x.UserIdentifier, x.User })
                .Select(a => new UserItem
                {
                    Name = a.Key.User,
                    Logbooks = a
                        .GroupBy(b => new { b.JournalSetupIdentifier, b.Logbook })
                        .Select(b => new LogbookItem
                        {
                            Name = b.Key.Logbook,
                            Entries = b.Select(c => new EntryItem
                            {
                                GAC = c.GAC,
                                Competency = c.Competency,
                                CreatedDate = TimeZoneInfo.ConvertTime(c.Created, timeZone).DateTime,
                                StartDate = c.Started,
                                EndDate = c.Stopped,
                                Employer = c.Employer,
                                Supervisor = c.Supervisor,
                                TrainingLocation = c.TrainingLocation,
                                Class = c.Class,
                                EntryNumber = c.EntryNumber,
                                Validated = c.Validated ? "Yes" : "No",
                                EntryHours = c.EntryHours ?? 0,
                                CompetencyHours = c.CompetencyHours ?? 0,
                                SatisfactionLevel = c.CompetencySatisfactionLevel.ToEnum(ExperienceCompetencySatisfactionLevel.None),
                                SkillRating = c.CompetencySkillRating
                            })
                            .OrderBy(x => x.EntryNumber)
                            .ThenBy(x => x.Competency)
                            .ToList()
                        })
                        .OrderBy(x => x.Name)
                        .ToList()
                })
                .OrderBy(x => x.Name)
                .ToList();

            return data;
        }

        #endregion
    }
}