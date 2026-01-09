using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.SessionState;

using InSite.Api.Settings;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Security")]
    [RoutePrefix("api/sessions")]
    public partial class SessionsController : ApiBaseController
    {
        private static DateTime _nextCleanTime = DateTime.MinValue;
        private static readonly Dictionary<SessionKey, DateTime> _sessionExpires =
            new Dictionary<SessionKey, DateTime>();

        [HttpGet]
        [Route("timeout")]
        public HttpResponseMessage Timeout(string session)
        {
            var expiration = GetExpiration(CurrentUser.UserIdentifier, session);

            return expiration == null ? JsonSuccess("Not Found") : JsonSuccess(expiration);
        }

        internal static ExpirationInfo GetExpiration(Guid user, string sessionId)
        {
            if (sessionId.IsEmpty())
                return null;

            DateTime expireDate;
            var key = new SessionKey(user, sessionId);
            lock (_sessionExpires)
                if (!_sessionExpires.TryGetValue(key, out expireDate))
                    return null;

            return new ExpirationInfo(expireDate);
        }

        internal static void UpdateSession(Guid user, HttpSessionState session)
        {
            var now = DateTime.UtcNow;
            if (now >= _nextCleanTime)
            {
                lock (_sessionExpires)
                {
                    if (now >= _nextCleanTime)
                    {
                        var keys = _sessionExpires.Keys.ToArray();
                        foreach (var rKey in keys)
                            if (now > _sessionExpires[rKey])
                                _sessionExpires.Remove(rKey);

                        _nextCleanTime = now.AddMinutes(1);
                    }
                }
            }

            var key = new SessionKey(user, session.SessionID);
            var expireDate = DateTime.UtcNow.AddMinutes(session.Timeout);

            lock (_sessionExpires)
                _sessionExpires[key] = expireDate;
        }
    }
}