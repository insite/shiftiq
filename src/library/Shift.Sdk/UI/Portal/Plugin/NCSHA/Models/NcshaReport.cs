using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class NcshaReport
    {
        public string Code { get; private set; }
        public string Title { get; private set; }

        public NcshaReport(string code, string title)
        {
            Code = code;
            Title = title;
        }
    }
}