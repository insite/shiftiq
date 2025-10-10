namespace Shift.Contract.Requests
{
    public class PagedRequest
    {
        public int Page { get; set; } = 1;
        public int Take { get; set; } = 20;

        public string[] Sort { get; set; }
    }
}