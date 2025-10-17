using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class SessionKey : MultiKey<Guid, string>
    {
        public SessionKey(Guid user, string session)
            : base(user, session)
        {

        }
    }
}