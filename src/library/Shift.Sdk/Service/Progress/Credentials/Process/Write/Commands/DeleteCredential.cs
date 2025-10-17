using System;

using Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class DeleteCredential : Command
    {
        public DeleteCredential(Guid credential)
        {
            AggregateIdentifier = credential;
        }
    }
}