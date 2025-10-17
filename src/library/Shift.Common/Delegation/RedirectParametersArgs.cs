using System;
using System.Collections.Specialized;

namespace Shift.Common.Events
{
    public delegate void RedirectParametersHandler(object sender, RedirectParametersArgs args);

    public class RedirectParametersArgs : EventArgs
    {
        public bool IsCancelled { get; set; }
        public NameValueCollection Parameters { get; } = new NameValueCollection();
    }
}