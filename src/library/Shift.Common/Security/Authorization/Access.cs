using Shift.Common;

namespace Shift.Common
{
    public class Access
    {
        public BasicAccess Basic { get; set; }
        public DataAccess Data { get; set; }
        public HttpAccess Http { get; set; }
        public AuthorityAccess Authority { get; set; }
    }
}