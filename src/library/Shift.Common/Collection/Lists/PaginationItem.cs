namespace Shift.Common
{
    public class PaginationItem
    {
        public int PageNumber { get; set; }
        public bool IsCurrent { get; set; }

        public string PageStatus => IsCurrent ? "active" : null;
    }
}