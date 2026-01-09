using System.Collections.Generic;

namespace Shift.Common
{
    public class Error
    {
        public int Code { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public Error()
        {
            Data = new Dictionary<string, string>();
        }

        public Error(string summary)
            : this()
        {
            Summary = summary;
        }

        public Error(string summary, int code)
            : this()
        {
            Summary = summary;
            Code = code;
        }
    }
}