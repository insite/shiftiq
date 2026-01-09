using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Assessment
{
    public partial class Validate : AdminBasePage, ICmdsUserControl
    {
        private static readonly object ThreadLock = new object();
        private int _counter;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                InitData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SubmitForValidation();
        }

        private void InitData()
        {
            PageHelper.AutoBindHeader(this);

            _counter = 1;

            rpCredentials.DataSource = Data;
            rpCredentials.DataBind();

            CompetenciesForValidation.Visible = Data.Rows.Count > 0;
            NoCompetenciesForValidation.Visible = Data.Rows.Count == 0;

            SaveButton.ConfirmText = Data.Rows.Count > 0
                ? "Are you sure you want to submit these competencies for validation?"
                : null;
        }

        protected string GetUrl(object competencyID)
        {
            return string.Format("/ui/cmds/portal/validations/competencies/edit?id={0}", competencyID);
        }

        protected string RowCssClass => ++_counter % 2 == 0 ? "alt1" : "alt2";

        private DataTable _data;

        private DataTable Data
        {
            get
            {
                if (_data == null)
                {
                    var filter = new EmployeeCompetencyFilter
                    {
                        OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                        Statuses = new[] { ValidationStatuses.NotApplicable, ValidationStatuses.SelfAssessed },
                        UserIdentifier = User.UserIdentifier,
                        ValidationDateMustBeNull = true
                    };

                    _data = UserCompetencyRepository.SelectSearchResults(filter, null, null);
                }

                return _data;
            }
        }

        private void SubmitForValidation()
        {
            var person = PersonSearch.Select(Organization.Identifier, User.UserIdentifier, x => x.User);

            var phones = "";

            if (!string.IsNullOrEmpty(person.User.PhoneMobile))
                phones += person.User.PhoneMobile + " mobile";

            if (!string.IsNullOrEmpty(person.PhoneWork))
            {
                if (phones.Length > 0)
                    phones += ", ";
                phones += person.PhoneWork + " work";
            }

            if (!string.IsNullOrEmpty(person.PhoneHome))
            {
                if (phones.Length > 0)
                    phones += ", ";
                phones += person.PhoneHome + " home";
            }

            if (string.IsNullOrEmpty(phones))
                phones = "(none)";

            var filter = new EmployeeCompetencyFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                Statuses = new[] { ValidationStatuses.NotApplicable, ValidationStatuses.SelfAssessed },
                UserIdentifier = User.UserIdentifier,
                ValidationDateMustBeNull = true
            };

            var table = UserCompetencyRepository.SelectSearchResults(filter, null, null);

            lock (ThreadLock)
            {
                UserCompetencyRepository.SubmitForValidation(User.UserIdentifier);
            }

            if (table.Rows.Count > 0)
                ServiceLocator.AlertMailer.Send(OrganizationIdentifiers.CMDS, User.UserIdentifier,
                    new Domain.Messages.AlertCompetencyValidationRequested
                    {
                        UserIdentifier = User.UserIdentifier,
                        Name = User.FullName,
                        Email = User.Email,
                        Phone = phones,
                        Company = Organization.CompanyName,
                        AssignmentCount = "competency".ToQuantity(table.Rows.Count),
                        AssignmentList = ToList(table),
                        Notified = DateTimeOffset.UtcNow
                    },
                    GetValidators(User.UserIdentifier));

            Response.Redirect(@"/ui/cmds/portal/validations/competencies/submit");
        }

        Guid[] GetValidators(Guid user)
        {
            var list = new List<Guid>();

            var validators = UserConnectionSearch.Bind(
                x => x.FromUser,
                x => x.IsValidator && x.ToUserIdentifier == user);

            foreach (var validator in validators)
                if (validator.Email != null)
                    list.Add(validator.UserIdentifier);

            return list.ToArray();
        }

        private string ToList(DataTable table)
        {
            var sb = new StringBuilder();

            foreach (DataRow row in table.Rows)
            {
                sb.AppendFormat($"- {row["SelfAssessmentStatus"]}: Competency {row["Number"]} - {row["Summary"]}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
