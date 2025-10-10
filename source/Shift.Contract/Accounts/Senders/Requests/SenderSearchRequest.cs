namespace Shift.Contract.Requests
{
    public class SenderSearchRequest : PagedRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public bool? Enabled { get; set; }
    }
}