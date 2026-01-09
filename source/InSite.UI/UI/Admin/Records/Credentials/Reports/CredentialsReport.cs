using System;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

using TimeZone = Shift.Common.TimeZone;

namespace InSite.Admin.Achievements.Credentials.Reports
{
    public static class CredentialsReport
    {
        private class CredentialRow
        { 
            public string EmployerName { get; set; }

            public string StudentName { get; set; }

            public string AchievementName { get; set; }

            public string CredentialStatus { get; set; }

            public DateTime? Granted { get; set; }

            public DateTime? Expired { get; set; }

            public string AchievementDescription { get; set; }
        }

        public static byte[] GetXlsx(VCredentialFilter filter)
        {
            var user = CurrentSessionState.Identity.User;

            var credentials = ServiceLocator.AchievementSearch
                .GetCredentials(filter)
                .Select(x => new CredentialRow
                {
                    EmployerName = x.OriginalEmployerGroupName ?? x.EmployerGroupName,
                    StudentName= x.UserFullName,
                    AchievementName = x.AchievementTitle,
                    CredentialStatus = x.CredentialStatus,
                    Granted = x.CredentialGranted.HasValue ? TimeZone.ConvertTime(x.CredentialGranted.Value.UtcDateTime, TimeZones.Utc, user.TimeZone) : (DateTime?)null,
                    Expired = x.CredentialExpirationExpected.HasValue ? TimeZone.ConvertTime(x.CredentialExpirationExpected.Value.UtcDateTime, TimeZones.Utc, user.TimeZone) : (DateTime?)null,
                    AchievementDescription = x.AchievementDescription
                })
                .ToList();

            var boldStyle = new XlsxCellStyle { IsBold = true };
            var boldRightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right, IsBold = true };
            var boldCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center, IsBold = true };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var centerStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };
            var dateFormat = "MMM d, yyyy";

            var sheet = new XlsxWorksheet("Credentials");
            
            sheet.Columns[0].Width = 40; // EmployerName
            sheet.Columns[1].Width = 25; // StudentName
            sheet.Columns[2].Width = 30; // AchievementName
            sheet.Columns[3].Width = 15; // CredentialStatus
            sheet.Columns[4].Width = 15; // Granted
            sheet.Columns[5].Width = 15; // Expired
            sheet.Columns[6].Width = 35; // AchievementDescription 

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Employer", Style = boldStyle }); // EmployerName
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Student", Style = boldStyle }); // StudentName
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "AchievementName", Style = boldStyle }); // AchievementName
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = "CredentialStatus", Style = boldStyle }); // CredentialStatus
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Granted", Style = boldStyle }); // Granted
            sheet.Cells.Add(new XlsxCell(5, 0) { Value = "Expired", Style = boldStyle }); // Expired
            sheet.Cells.Add(new XlsxCell(6, 0) { Value = "Trade", Style = boldStyle }); // AchievementDescription


            for (int rowIndex = 0; rowIndex < credentials.Count; rowIndex++)
            {
                var row = credentials[rowIndex];

                sheet.Cells.Add(new XlsxCell(0, rowIndex + 1) { Value = row.EmployerName});
                sheet.Cells.Add(new XlsxCell(1, rowIndex + 1) { Value = row.StudentName });
                sheet.Cells.Add(new XlsxCell(2, rowIndex + 1) { Value = row.AchievementName });
                sheet.Cells.Add(new XlsxCell(3, rowIndex + 1) { Value = row.CredentialStatus });
                sheet.Cells.Add(new XlsxCell(4, rowIndex + 1) { Value = row.Granted, Style = centerStyle, Format = dateFormat });
                sheet.Cells.Add(new XlsxCell(5, rowIndex + 1) { Value = row.Expired, Style = centerStyle, Format = dateFormat });
                sheet.Cells.Add(new XlsxCell(6, rowIndex + 1) { Value = row.AchievementDescription});
            }

            return sheet.GetBytes();
        }

    }
}