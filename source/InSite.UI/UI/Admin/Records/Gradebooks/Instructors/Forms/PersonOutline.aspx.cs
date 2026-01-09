using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Contacts.People.Controls;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

using static InSite.Persistence.AddressSearch;

namespace InSite.UI.Admin.Records.Instructors.Forms
{
    public partial class PersonOutline : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        protected Guid? UserIdentifier => Guid.TryParse(Request.QueryString["contact"], out var contact) ? contact : (Guid?)null;

        private Guid GradebookIdentifier => Guid.TryParse(Request.QueryString["gradebook"], out var gradebook) ? gradebook : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AddressList.EnablePaging = false;
            Registrations.EnablePaging = false;

            if (!IsPostBack)
                LoadData();

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "initTreeViews",
                "(function () { initGradeTreeView(); })();",
                true);

        }

        #endregion

        #region Binding

        private void LoadData()
        {
            if (UserIdentifier == null)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/instructors/search");
                return;
            }

            var person = PersonSearch.Select(Organization.Identifier, UserIdentifier.Value,
                x => x.User,
                x => x.EmployerGroup,
                x => x.HomeAddress,
                x => x.WorkAddress,
                x => x.BillingAddress,
                x => x.ShippingAddress,
                x => x.EmployerGroup.Parent
                );

            if (person == null)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/instructors/search");

            var title = person.User.FullName;

            if (person.PersonCode != null)
                title += $" <span class='form-text'>Account #{person.PersonCode}</span>";

            PageHelper.AutoBindHeader(this, qualifier: title);

            FullName.Text = person.User.FullName;
            Honorific.Text = person.User.Honorific.HasValue() ? person.User.Honorific + " " : "";
            Email.Text = person.User.Email.HasValue() ? person.User.Email.ToLower() : null;
            Phone.Text = person.Phone.HasValue() ? person.Phone : "None";
            PhoneHome.Text = person.PhoneHome.HasValue() ? person.PhoneHome : "None";
            PhoneWork.Text = person.PhoneWork.HasValue() ? person.PhoneWork : "None";
            PhoneMobile.Text = person.User.PhoneMobile.HasValue() ? person.User.PhoneMobile : "None";
            PhoneOther.Text = person.PhoneOther.HasValue() ? person.PhoneOther : "None";
            ContactCode.Text = person.PersonCode.HasValue() ? person.PersonCode : "None";
            Birthdate.Text = $"{person.Birthdate:MMM d, yyyy}";
            Region.Text = person.Region.HasValue() ? person.Region : "None";
            PersonCode.Text = person.PersonCode.IfNullOrEmpty("None");
            Gender.Text = person.Gender.HasValue() ? person.Gender : "None";

            EmergencyContactName.Text = person.EmergencyContactName;
            EmergencyContactPhoneNumber.Text = person.EmergencyContactPhone;
            EmergencyContactRelationship.Text = person.EmergencyContactRelationship;
            if (EmergencyContactRelationship.Text == "" && EmergencyContactPhoneNumber.Text == "" && EmergencyContactRelationship.Text == "")
                EmergencyContactName.Text = "None";
            ESL.Checked = string.Equals(person.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase);

            BindAddresses(person);
            BindRegistrations(person.User);
            BindGrades();
            BindAchievements(person.User);
        }

        private void BindAddresses(Person person)
        {
            var list = new List<UserAddress>();

            if (person.HomeAddress != null)
                list.Add(new UserAddress { AddressType = "Home", Address = person.HomeAddress });

            if (person.WorkAddress != null)
                list.Add(new UserAddress { AddressType = "Work", Address = person.WorkAddress });

            if (person.BillingAddress != null)
                list.Add(new UserAddress { AddressType = "Billing", Address = person.BillingAddress });

            if (person.ShippingAddress != null)
                list.Add(new UserAddress { AddressType = "Shipping", Address = person.ShippingAddress });

            AddressList.DataSource = list;
            AddressList.DataBind();

            AddressSection.SetTitle("Addresses", list.Count);
        }

        private void BindRegistrations(User user)
        {
            var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByCandidate(user.UserIdentifier)
                .Select(x => new
                {
                    x.Event,
                    x.RegistrationRequestedOn,
                    x.WorkBasedHoursToDate,
                    x.ApprovalStatus,
                    Customer = x.CustomerIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(x.CustomerIdentifier.Value) : null,
                    Employer = x.EmployerIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(x.EmployerIdentifier.Value) : null,
                    x.RegistrationFee,
                    x.AttendanceStatus,
                    x.Score,
                    x.RegistrationComment
                })
                .OrderByDescending(x => x.RegistrationRequestedOn)
                .ThenBy(x => x.Event.EventTitle)
                .ToList();

            Registrations.DataSource = registrations;
            Registrations.DataBind();

            RegistrationSection.SetTitle("Registrations", registrations.Count);
        }

        private void BindGrades()
        {
            int gradeCount;
            var gradeHierarchy = GetGradeHierarchy(out gradeCount);

            NoGrades.Visible = gradeHierarchy.Count == 0;
            GradePanel.Visible = gradeHierarchy.Count > 0;

            Grades.LoadData(gradeHierarchy);

            GradeSection.SetTitle("Records", gradeCount);
        }

        private List<GradeTreeViewNode.Grade> GetGradeHierarchy(out int gradeCount)
        {
            gradeCount = 0;

            var scores = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { StudentUserIdentifier = UserIdentifier }, null, null, x => x.Gradebook);
            var gradebookIdentifiers = scores.Select(x => x.GradebookIdentifier).Distinct().ToList();
            var items = new List<GradeTreeViewNode.Grade>();

            foreach (var gradebookIdentifier in gradebookIdentifiers)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebookState(gradebookIdentifier);
                var gradebookQuery = scores.Find(x => x.GradebookIdentifier == gradebookIdentifier).Gradebook;

                var gradebookItem = new GradeTreeViewNode.Grade
                {
                    ID = gradebookIdentifier.ToString(),
                    Name = gradebook.Name,
                    ClassName = gradebookQuery.Event?.EventTitle,
                    ClassStartDate = gradebookQuery.Event != null ? gradebookQuery.Event.EventScheduledStart : (DateTimeOffset?)null,
                    ClassEndDate = gradebookQuery.Event?.EventScheduledEnd,
                    Level = 0,
                    ScoreValue = null,
                    Comment = null,
                    Children = new List<GradeTreeViewNode.Grade>()
                };

                items.Add(gradebookItem);

                gradeCount += AddItems(gradebook, gradebookItem, gradebook.RootItems, 1);
            }

            return items;
        }

        private int AddItems(GradebookState dataGradebook, GradeTreeViewNode.Grade parent, List<GradeItem> input, int level)
        {
            if (input.IsEmpty())
                return 0;

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(
                new QProgressFilter { GradebookIdentifier = dataGradebook.Identifier },
                null,
                null,
                x => x.GradeItem
            );

            var gradeCount = 0;

            foreach (var inputItem in input)
            {
                if (inputItem.IsReported)
                {
                    var score = progresses.Find(x => x.GradeItemIdentifier == inputItem.Identifier && x.UserIdentifier == UserIdentifier);
                    var scoreValue = GradebookHelper.GetScoreValue(score, inputItem);

                    var outputItem = new GradeTreeViewNode.Grade
                    {
                        ID = $"{parent.ID}-{inputItem.Identifier}",
                        Name = inputItem.Name,
                        Level = level,
                        ScoreValue = scoreValue,
                        Comment = score?.ProgressComment,
                        Children = new List<GradeTreeViewNode.Grade>()
                    };

                    parent.Children.Add(outputItem);

                    if (scoreValue != null)
                        gradeCount++;

                    gradeCount += AddItems(dataGradebook, outputItem, inputItem.Children, level + 1);
                }
            }

            return gradeCount;
        }

        private void BindAchievements(User user)
        {
            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = user.UserIdentifier
            };

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(filter);
            AchievementGrid.DataSource = credentials;

            AchievementSection.SetTitle("Achievements", credentials.Count);
        }

        protected string IndentHtml(int indent)
            => $"<span style='display:inline-block;width:{20 * indent}px;'></span>";

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/gradebook-outline"))
                return $"id={GradebookIdentifier}";

            return null;
        }

        #endregion
    }
}
