using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    public class LtiParameters
    {
        #region Properties

        public string HttpMethod { get; private set; }

        #endregion

        #region Fields

        private NameValueCollection _parameters;

        #endregion

        #region Construction

        public LtiParameters(string httpMethod)
        {
            HttpMethod = httpMethod;

            _parameters = new NameValueCollection();

            _parameters.Add("oauth_callback", "about:blank");
            _parameters.Add("oauth_nonce", UniqueIdentifier.Create().ToString("N"));
            _parameters.Add("oauth_signature_method", "HMAC-SHA1");
            _parameters.Add("oauth_timestamp", Clock.ToUnixTimestamp(DateTime.UtcNow).ToString());
            _parameters.Add("oauth_version", "1.0");

            _parameters.Add("launch_presentation_locale", CultureInfo.CurrentCulture.Name);
            _parameters.Add("lti_message_type", "basic-lti-launch-request");
            _parameters.Add("lti_version", "LTI-1p0");
        }

        #endregion

        #region Methods

        public void Add(string name, string value)
        {
            _parameters.Add(name, value);
        }

        public void Add(string name, params LtiRole[] roles)
        {
            var value = roles.Any() ? string.Join(",", roles.ToList()) : string.Empty;

            _parameters.Add(name, value);
        }

        public bool Contains(string name)
        {
            string value = _parameters[name];
            return value != null;
        }

        public NameValueCollection GetParameters()
        {
            return new NameValueCollection(_parameters);
        }

        #endregion
    }
}