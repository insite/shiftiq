using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Registrations.Reports
{
    public partial class DetailedRegistrationReport : UserControl
    {
        private class RegistrationItem
        {
            public int Number { get; set; }
            public string PersonCode { get; set; }

            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public DateTime? BirthDate { get; set; }
            public int? Age { get; set; }
            public bool IsMinor => Age.HasValue && Age < 19;
            public bool ESL { get; set; }
            public string UserPhones { get; set; }
            public string UserAddress { get; set; }
            public string EmergencyContact { get; set; }

            public Guid? EmployerIdentifier { get; set; }
            public string EmployerName { get; set; }
            public string EmployerAddress { get; set; }
            public string EmployerPhones { get; set; }
            public string EmployerContactName { get; set; }
            public string EmployerContactPhone { get; set; }
            public string EmployerContactEmail { get; set; }
            public string ApprovalStatus { get; set; }
            public string AttendanceStatus { get; set; }
            public DateTimeOffset? RegistrationDate { get; set; }

            public int? HoursWorked { get; set; }
            public decimal? Fee { get; set; }

            public Guid? CustomerIdentifier { get; set; }
            public string CustomerName { get; set; }
        }

        private class EventItem
        {
            public string EventTitle { get; set; }
            public string EventStart { get; set; }
            public string EventEnd { get; set; }

            public List<RegistrationItem> Registrations { get; set; }
        }

        public void LoadReport(QRegistrationFilter filter)
        {
            PageTitle.InnerText = "Registration Report (Detailed)";

            var data = GetData(filter);

            EventRepeater.ItemDataBound += EventRepeater_ItemDataBound;
            EventRepeater.DataSource = data;
            EventRepeater.DataBind();
        }

        public byte[] GetXlsx(QRegistrationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            var data = GetData(filter);

            BindAdditionalInfo(data);

            var @event = data[0];

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Top };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Top };

            var sheet = new XlsxWorksheet("Registration Report (Detailed)");

            for (int i = 0; i < 4; i++)
                sheet.Columns[i].Width = 40;

            string title = $"Registration Report (Detailed) \r\n" +
                $"{@event.EventTitle} ({@event.EventStart} - {@event.EventEnd})";

            sheet.Rows[0].Height = 30;
            sheet.Cells.Add(new XlsxCell(0, 0, 4) { Value = title, Style = boldStyle });

            for (int i = 0; i < @event.Registrations.Count; i++)
            {
                var registration = @event.Registrations[i];

                sheet.Rows[i + 1].Height = 120;

                string firstColumn = $"{registration.Number}. {registration.UserFullName} \r\n" +
                    $"Email: {registration.UserEmail} \r\n" +
                    $"ID #: {registration.PersonCode} \r\n" +
                    $"Birthdate: {registration.BirthDate.Format("MMMM dd, yyyy")} Age: {registration.Age} \r\n" +
                    $"ESL: {(registration.ESL ? "Yes" : "No")}";

                sheet.Cells.Add(new XlsxCell(0, i + 1) { Value = firstColumn, Style = normalStyle, WrapText = true });

                StringBuilder secondColumn = new StringBuilder();

                if (registration.UserPhones != "None")
                    secondColumn.Append($"Phones: \r\n{registration.UserPhones.Replace("<div>", "").Replace("</div>", "")}");
                else
                    secondColumn.AppendLine("Phones: None");

                if (registration.UserAddress != "None")
                    secondColumn.Append($"Home Address: \r\n{registration.UserAddress.Replace("<div>", "").Replace("</div>", "")}");
                else
                    secondColumn.AppendLine("Home Address: None");

                secondColumn.AppendLine($"Emergency Contact: \r\n{registration.EmergencyContact}");

                sheet.Cells.Add(new XlsxCell(1, i + 1) { Value = secondColumn.ToString(), Style = normalStyle, WrapText = true });

                StringBuilder thirdColumn = new StringBuilder();

                if (registration.EmployerName != null)
                {
                    thirdColumn.AppendLine($"Employer: {registration.EmployerName}");
                    if (registration.EmployerPhones != null)
                        thirdColumn.Append(registration.EmployerPhones.Replace("<div>", "").Replace("</div>", ""));

                    if (registration.EmployerAddress != null)
                        thirdColumn.Append($"Mailling Address: \r\n{registration.EmployerAddress.Replace("<div>", "").Replace("</div>", "")}");

                    if (registration.EmployerContactName != "None")
                    {
                        thirdColumn.AppendLine($"Employer Contact: \r\n" +
                            $"{registration.EmployerContactName}\r\n" +
                            $"{registration.EmployerContactPhone}\r\n" +
                            $"{registration.EmployerContactEmail}");
                    }
                    else
                        thirdColumn.AppendLine("Employer Contact: None");
                }
                else
                {
                    thirdColumn.AppendLine("Employer: N/A");
                }

                sheet.Cells.Add(new XlsxCell(2, i + 1) { Value = thirdColumn.ToString(), Style = normalStyle, WrapText = true });

                string fourthColumn = $"{registration.ApprovalStatus} on {registration.RegistrationDate.FormatDateOnly("MMMM, yyyy")} \r\n";

                if (!string.IsNullOrEmpty(registration.AttendanceStatus))
                    fourthColumn += $"{registration.AttendanceStatus} \r\n";

                fourthColumn += $"Hours Worked: {registration.HoursWorked} \r\n" +
                    $"Fee: {registration.Fee} \r\n" +
                    $"Paid By: {registration.CustomerName}";

                sheet.Cells.Add(new XlsxCell(3, i + 1) { Value = fourthColumn, Style = normalStyle, WrapText = true });
            }

            return XlsxWorksheet.GetBytes(sheet);
        }

        private List<EventItem> GetData(QRegistrationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            var data = ServiceLocator.RegistrationSearch
                .GetRegistrations(filter,
                    x => x.Event,
                    x => x.Candidate,
                    x => x.Employer,
                    x => x.Payment.CreatedByUser
                )
                .GroupBy(x => x.EventIdentifier)
                .Select(x =>
                {
                    var registration = x.First();
                    var number = 1;

                    return new EventItem
                    {
                        EventTitle = registration.Event.EventTitle,
                        EventStart = registration.Event.EventScheduledStart.FormatDateOnly(timeZone),
                        EventEnd = registration.Event.EventScheduledEnd.HasValue ? registration.Event.EventScheduledEnd.FormatDateOnly(timeZone) : "N/A",
                        Registrations = x
                            .OrderBy(y => y.ApprovalStatus)
                            .ThenBy(y => y.Candidate.UserFullName)
                            .Select(y => new RegistrationItem
                            {
                                Number = number++,
                                UserIdentifier = y.CandidateIdentifier,
                                PersonCode = y.Candidate.PersonCode,
                                UserFullName = y.Candidate.UserFullName,
                                BirthDate = y.Candidate.Birthdate,
                                UserEmail = y.Candidate.UserEmail,
                                Age = y.Candidate.Birthdate.HasValue
                                    ? GetAge(y.Candidate.Birthdate.Value, registration.Event.EventScheduledStart.Date)
                                    : (int?)null,

                                EmployerIdentifier = y.EmployerIdentifier,
                                EmployerName = y.Employer?.GroupName,

                                ApprovalStatus = y.ApprovalStatus,
                                AttendanceStatus = y.AttendanceStatus,
                                RegistrationDate = y.RegistrationRequestedOn,
                                HoursWorked = y.WorkBasedHoursToDate,
                                Fee = y.RegistrationFee,
                                CustomerName = y.Payment?.CreatedByUser?.UserFullName

                            }).ToList()
                    };
                })
                .OrderBy(x => x.EventTitle)
                .ToList();

            BindAdditionalInfo(data);

            return data;
        }

        private void BindAdditionalInfo(List<EventItem> events)
        {
            foreach (var @event in events)
            {
                foreach (var registration in @event.Registrations)
                {
                    var candidate = PersonSearch.Select(CurrentSessionState.Identity.Organization.Identifier, registration.UserIdentifier, x => x.User, x => x.HomeAddress);

                    registration.ESL = string.Equals(candidate.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? true : false;

                    var EmergencyContactName = string.IsNullOrEmpty(candidate.EmergencyContactName) ? "N/A" : candidate.EmergencyContactName;
                    var EmergencyContactPhoneNumber = string.IsNullOrEmpty(candidate.EmergencyContactPhone) ? "" : candidate.EmergencyContactPhone;
                    var EmergencyContactRelationship = string.IsNullOrEmpty(candidate.EmergencyContactRelationship) ? "" : candidate.EmergencyContactRelationship;
                    if (EmergencyContactName == "N/A")
                        registration.EmergencyContact = EmergencyContactName;
                    else
                        registration.EmergencyContact = EmergencyContactName + " (" + EmergencyContactRelationship + "): " + EmergencyContactPhoneNumber;

                    var phones = new StringBuilder();

                    if (candidate.Phone.HasValue())
                        phones.AppendLine($"<div>Preferred: {candidate.Phone}</div>");
                    if (candidate.PhoneHome.HasValue())
                        phones.AppendLine($"<div>Home: {candidate.PhoneHome}</div>");
                    if (candidate.PhoneWork.HasValue())
                        phones.AppendLine($"<div>Work: {candidate.PhoneWork}</div>");
                    if (candidate.User.PhoneMobile.HasValue())
                        phones.AppendLine($"<div>Cell: {candidate.User.PhoneMobile}</div>");
                    if (candidate.PhoneOther.HasValue())
                        phones.AppendLine($"<div>Other: {candidate.PhoneOther}</div>");

                    if (phones.Length > 0)
                        registration.UserPhones = phones.ToString();
                    else
                        registration.UserPhones = "None";

                    if (candidate.HomeAddress != null)
                    {
                        registration.UserAddress = ClassVenueAddressInfo.GetAddress(candidate.HomeAddress);

                        if (registration.UserAddress == "")
                            registration.UserAddress = "None";
                    }
                    else
                        registration.UserAddress = "None";

                    //bind employer
                    var employer = registration.EmployerIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(registration.EmployerIdentifier.Value) : null;

                    if (employer != null)
                    {
                        var address = ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Shipping);

                        var pN = new StringBuilder();

                        if (employer.GroupPhone.HasValue())
                            pN.AppendLine($"<div>Phone: {employer.GroupPhone}</div>");

                        if (pN.Length > 0)
                            registration.EmployerPhones = pN.ToString();

                        if (address != null)
                            registration.EmployerAddress = ClassVenueAddressInfo.GetAddress(address);

                        registration.EmployerContactName = "None";

                        var membership = MembershipSearch.SelectFirst(x => x.GroupIdentifier == employer.GroupIdentifier && x.MembershipType == MembershipType.EmployerContact);
                        if (membership != null)
                        {
                            var manager = PersonSearch.Select(CurrentSessionState.Identity.Organization.Identifier, membership.UserIdentifier, x => x.User);
                            if (manager != null)
                            {
                                registration.EmployerContactName = manager.User.FullName;
                                registration.EmployerContactEmail = manager.User.Email;

                                if (!string.IsNullOrEmpty(manager.Phone))
                                    registration.EmployerContactPhone = manager.Phone;
                            }
                        }
                    }
                    else
                    {
                        registration.EmployerName = "N/A";
                        registration.EmployerContactName = "None";
                        registration.EmployerAddress = "N/A";
                    }
                }
            }
        }

        private void EventRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var @event = (EventItem)e.Item.DataItem;

            var registrationRepeater = (Repeater)e.Item.FindControl("RegistrationRepeater");

            registrationRepeater.DataSource = @event.Registrations;
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