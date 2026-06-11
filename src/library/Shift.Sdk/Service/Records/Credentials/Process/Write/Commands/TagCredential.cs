using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class TagCredential : Command
    {
        public TagCredential(Guid credential, string necessity, string priority)
        {
            AggregateIdentifier = credential;
            Necessity = necessity;
            Priority = priority;
        }

        public string Necessity { get; set; }
        public string Priority { get; set; }
    }
}