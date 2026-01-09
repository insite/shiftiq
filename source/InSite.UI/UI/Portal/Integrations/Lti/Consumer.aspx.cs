using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

using InSite.Common.Web;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Integrations.Lti
{
    public partial class Consumer : PortalBasePage
    {
        private class Model
        {
            public string Url { get; set; }
            public NameValueCollection Parameters { get; set; }
            public string Signature { get; set; }
        }

        private int? Number => int.TryParse(Request["number"], out var number) ? number : (int?)null;

        private bool Debug => bool.TryParse(Request["debug"], out var debug) ? debug : false;

        protected string FormUrl { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (!Number.HasValue)
                HttpResponseHelper.SendHttp400();

            var model = GetModel();

            var sortedParameters = new SortedDictionary<string, string>();
            foreach (var key in model.Parameters.AllKeys)
                sortedParameters.Add(key, model.Parameters[key]);

            var parameterItems = sortedParameters.Select(x => new { Key = x.Key, Value = x.Value });

            DebugPanel.Visible = Debug;
            NoneDebugPanel.Visible = !Debug;

            FormUrl = model.Url;

            if (!Debug)
            {
                ParameterRepeater.DataSource = parameterItems;
                ParameterRepeater.DataBind();
            }
            else
            {
                ParameterRepeater2.DataSource = parameterItems;
                ParameterRepeater2.DataBind();

                ParameterRepeater3.DataSource = parameterItems;
                ParameterRepeater3.DataBind();
            }
        }

        private Model GetModel()
        {
            var organization = CurrentSessionState.Identity.Organization;
            var user = CurrentSessionState.Identity.User;
            var asset = Persistence.LtiLinkSearch.Select(organization.Identifier, Number.Value);

            if (asset == null)
                HttpResponseHelper.Redirect("/ui/admin/courses/links/search");

            var email = user.Email;
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var role = "Learner";
            var url = asset.ToolProviderUrl;
            var secret = asset.ToolConsumerSecret;
            var consumer = asset.ToolConsumerKey;

            var postParameters = new LtiParameters("POST");

            postParameters.Add("oauth_consumer_key", consumer);
            postParameters.Add("user_id", email);
            postParameters.Add("lis_person_name_given", firstName);
            postParameters.Add("lis_person_name_family", lastName);
            postParameters.Add("lis_person_contact_email_primary", email);
            postParameters.Add("roles", role);

            // Add static LTI parameters

            var rootUrl = ServiceLocator.Urls.GetApplicationUrl(organization.Code);

            AddPresentationParameters(postParameters, rootUrl);

            AddLearningParameters(postParameters, Number.Value, firstName + " " + lastName, email, rootUrl);

            AddResourceParameters(postParameters);

            var settings = ServiceLocator.AppSettings;

            var partition = ServiceLocator.Partition;

            var product = StringHelper.Sanitize(settings.Release.Brand, '-');

            AddToolParameters(postParameters,
                product,
                settings.Release.Version,
                partition.Email,
                settings.Security.Domain,
                settings.Release.Brand,
                rootUrl);

            AddCustomParameters(postParameters, asset.ResourceParameters);

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = $"https://{url}";

            var ticket = LtiTicketHelper.GetTicket(secret, url, postParameters);

            var parameters = new NameValueCollection();
            foreach (string key in ticket.Parameters)
                parameters.Add(key, ticket.Parameters[key]);

            parameters.Add("oauth_signature", ticket.Signature);

            return new Model { Url = ticket.Url.ToString(), Parameters = parameters, Signature = ticket.Signature };
        }

        private void AddPresentationParameters(LtiParameters parameters, string url)
        {
            parameters.Add("launch_presentation_css_url", url);
            parameters.Add("launch_presentation_document_target", "frame");
            parameters.Add("launch_presentation_locale", "en-US");
            parameters.Add("launch_presentation_return_url", url);
        }

        private void AddLearningParameters(LtiParameters parameters, int number, string name, string email, string url)
        {
            parameters.Add("lis_course_offering_sourcedid", number.ToString());
            parameters.Add("lis_course_section_sourcedid", number.ToString() + ":1");
            parameters.Add("lis_person_name_full", name);
            parameters.Add("lis_person_sourcedid", "sis:1");
            parameters.Add("lis_outcome_service_url", url);
            parameters.Add("lis_result_sourcedid", number.ToString() + ":" + email);
        }

        private void AddResourceParameters(LtiParameters parameters)
        {
            parameters.Add("resource_link_description", "-");
            parameters.Add("resource_link_id", "0");
            parameters.Add("resource_link_title", "-");
        }

        private void AddToolParameters(LtiParameters parameters,
            string product,
            string version,
            string supportEmail,
            string domain,
            string brand,
            string defaultUrl
            )
        {
            parameters.Add("tool_consumer_info_product_family_code", product);
            parameters.Add("tool_consumer_info_version", version);
            parameters.Add("tool_consumer_instance_contact_email", supportEmail);
            parameters.Add("tool_consumer_instance_description", brand);
            parameters.Add("tool_consumer_instance_guid", domain);
            parameters.Add("tool_consumer_instance_name", brand);
            parameters.Add("tool_consumer_instance_url", defaultUrl);
        }

        private void AddCustomParameters(LtiParameters parameters, string custom)
        {
            if (!string.IsNullOrEmpty(custom))
            {
                var customParameters = custom.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var customParameter in customParameters)
                {
                    var match = Regex.Match(customParameter, @"^([^=]+)\=(.+)$");
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value.Trim();
                        var value = match.Groups[2].Value.Trim();

                        if (!parameters.Contains(name))
                            parameters.Add(name, value);
                    }
                }
            }
        }
    }
}