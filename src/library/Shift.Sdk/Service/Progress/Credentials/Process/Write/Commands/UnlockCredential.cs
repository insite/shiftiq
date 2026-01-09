using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class UnlockCredential : Command
    {
        public UnlockCredential(Guid credential)
        {
            AggregateIdentifier = credential;
        }
    }
}