using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class ChangeCertificateLayout : Command
    {
        public string Code { get; set; }

        public ChangeCertificateLayout(Guid achievement, string code)
        {
            AggregateIdentifier = achievement;
            Code = code;
        }
    }
}