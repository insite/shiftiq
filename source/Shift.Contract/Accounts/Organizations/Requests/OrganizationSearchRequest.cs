namespace Shift.Contract.Requests
{
    public class OrganizationSearchRequest : PagedRequest
    {
        public string Name { get; set; }
    }
}