using System.Net;

namespace Shift.Hub
{
    public class ApiError
    {
        public ApiError()
        {
            Code = HttpStatusCode.BadRequest;
            Type = "Error";
            Text = "Error";
        }

        public HttpStatusCode Code { get; set; }
        public string Name => Code.ToString();

        public string Type { get; set; }
        public string Text { get; set; }
    }
}
