namespace Shift.Contract.Requests
{
    public class SenderOrganizationSearchRequest : PagedRequest
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}