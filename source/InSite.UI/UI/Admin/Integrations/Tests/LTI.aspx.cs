using System;
using System.Linq;
using System.Web;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Integrations.Tests
{
    public partial class LTI : AdminBasePage
    {
        protected Uri LaunchUrl
        {
            get => (Uri)ViewState[nameof(LaunchUrl)];
            set => ViewState[nameof(LaunchUrl)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InputTypeList.AutoPostBack = true;
            InputTypeList.SelectedIndexChanged += (x, y) => InputTypeChanged();

            ValidateButton.Click += ValidateButton_Click;
            GoToStep1.Click += (x, y) => MultiView.SetActiveView(ViewStep1);
            LaunchButton.Click += LaunchButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LaunchScript.Visible = false;

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect("/ui/admin/home");

            PageHelper.AutoBindHeader(this);

            MultiView.SetActiveView(ViewStep1);

            LoadDefaults();
            InputTypeChanged();
        }

        private void LoadDefaults()
        {
            var url = Request.Url;

            LtiLearnerCode.Text = "BB123";
            LtiLearnerEmail.Text = "bugs.bunny@example.com";
            LtiLearnerNameFirst.Text = "Bugs";
            LtiLearnerNameLast.Text = "Bunny";
            LtiGroupName.Text = "Tunes";
            LtiOrganizationIdentifier.Text = "f85f5022-341e-4236-b69c-75b29e134972";
            LtiOrganizationSecret.Text = "bB8u6Tj60uJL2RKYR0OCyiGMdds9gaEUs9Q2d3bRTTVRKJ516CCc1LeSMChAI0rc";
            LtiLaunchUrl.Text = $"{url.Scheme}://{url.Host}/ui/lobby/integration/lti/launch";
        }

        private void InputTypeChanged()
        {
            switch (InputTypeList.SelectedValue)
            {
                case "Fields":
                    InputMultiView.SetActiveView(InputFieldsView);
                    break;
                case "Text":
                    InputMultiView.SetActiveView(InputTextView);
                    break;
                default:
                    InputMultiView.ActiveViewIndex = -1;
                    break;
            }
        }

        private void ValidateButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            LtiTicket ticket;

            try
            {
                switch (InputTypeList.SelectedValue)
                {
                    case "Fields":
                        ticket = CreateLtiTicketFromFields();
                        break;
                    case "Text":
                        ticket = CreateLtiTicketFromDataText();
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, ex.Message);
                return;
            }

            LaunchUrl = ticket.Url;

            ParameterRepeater.DataSource = ticket.Parameters.AllKeys
                .Select(x => new { Key = x, Value = ticket.Parameters[x] })
                .OrderBy(x => x.Key);
            ParameterRepeater.DataBind();

            MultiView.SetActiveView(ViewStep2);
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            LaunchScript.Visible = true;
        }

        private LtiTicket CreateLtiTicketFromFields()
        {
            var identifier = LtiOrganizationIdentifier.Text;
            var group = LtiGroupName.Text;
            var secret = LtiOrganizationSecret.Text;
            var code = LtiLearnerCode.Text;
            var email = LtiLearnerEmail.Text;
            var firstName = LtiLearnerNameFirst.Text;
            var lastName = LtiLearnerNameLast.Text;
            var role = "Learner";

            var url = LtiLaunchUrl.Text;

            var postParameters = new LtiParameters("POST");

            postParameters.Add("oauth_consumer_key", secret);

            postParameters.Add("user_id", code);

            postParameters.Add("lis_person_name_given", firstName);
            postParameters.Add("lis_person_name_family", lastName);
            postParameters.Add("lis_person_contact_email_primary", email);

            postParameters.Add("roles", role);

            postParameters.Add("shift_group_name", group);
            postParameters.Add("shift_organization_identifier", identifier);

            var ticket = LtiTicketHelper.GetTicket(secret, url, postParameters);

            ticket.Parameters.Add("oauth_signature", ticket.Signature);

            return ticket;
        }

        private LtiTicket CreateLtiTicketFromDataText()
        {
            var url = new Uri(LtiLaunchUrl.Text);
            var parameters = HttpUtility.ParseQueryString(DataText.Text);
            var signature = parameters["oauth_signature"];
            var ticket = new LtiTicket(url, signature, parameters);

            return ticket;
        }
    }
}