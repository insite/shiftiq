using System;

namespace Shift.Sdk.UI
{
    public class MembershipSearchResultEventArgs : EventArgs
    {
        public MembershipSearchResult[] MembershipResults { get; }

        public MembershipSearchResultEventArgs(MembershipSearchResult[] membershipResults)
        {
            MembershipResults = membershipResults;
        }
    }
}
