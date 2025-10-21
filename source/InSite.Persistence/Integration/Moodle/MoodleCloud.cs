using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Persistence.Integration.Moodle
{
    public class MoodleCloud
    {
        private readonly string _siteUrl;
        private readonly string _tokenSecret;
        private readonly string _progressCallbackUrl;

        public MoodleCloud(string siteUrl, string tokenSecret, string progressCallbackUrl)
        {
            _siteUrl = siteUrl;
            _tokenSecret = tokenSecret;
            _progressCallbackUrl = progressCallbackUrl;
        }

        public string GetLaunchUrl(Guid learnerId, string learnerName, string courseHook, Guid activityId)
        {
            try
            {
                var subject = learnerName;

                var payload = CreatePayload(learnerId, courseHook, activityId);

                var jwtEncoder = new JwtEncoder();

                var token = jwtEncoder.Encode(payload, _tokenSecret);

                var url = $"{_siteUrl}/login/index.php?token={token}";

                return url;
            }
            catch (Exception ex)
            {
                var error = $"An unexpected error occurred generating the URL for a learner ({learnerId}) to launch "
                    + $"a Moodle course ({courseHook}) from a Shift iQ learning activity ({activityId}).";

                throw new Exception(error, ex);
            }
        }

        private Dictionary<string, string> CreatePayload(Guid learner, string course, Guid activity)
        {
            var callback = _progressCallbackUrl.Replace("{activity}", activity.ToString());

            var json = $"{{\"userguid\":\"{learner}\",\"shortname\":\"{course}\",\"callbackurl\":\"{callback}\"}}";

            var serializer = new JsonSerializer2();

            var dictionary = serializer.Deserialize<Dictionary<string, string>>(json);

            return dictionary;
        }
    }
}