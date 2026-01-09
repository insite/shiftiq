using System;

namespace Shift.Common.Events
{
    public delegate void RedirectUrlHandler(object sender, RedirectUrlArgs args);

    public class RedirectUrlArgs : EventArgs
    {
        public string Url { get; }

        public RedirectUrlArgs(string url)
        {
            Url = url;
        }
    }
}