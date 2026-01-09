using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Web;
using System.Web.SessionState;

namespace InSite.Common.Web
{
    public class InSiteSessionState
    {
        private HttpSessionState Session => HttpContext.Current?.Session;

        private string _keyPrefix;

        public InSiteSessionState()
            : this(typeof(InSiteSessionState).FullName)
        {

        }

        public InSiteSessionState(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _keyPrefix = name + ".";
        }

        public object GetValue([CallerMemberName] string name = null)
        {
            if (name == null || Session == null)
                return null;

            try
            {
                return Session[_keyPrefix + name];
            }
            catch (SerializationException)
            {
                return null;
            }
        }

        public void SetValue(object value, [CallerMemberName] string name = null)
        {
            if (name == null || Session == null)
                return;

            Session[_keyPrefix + name] = value;
        }
    }
}
