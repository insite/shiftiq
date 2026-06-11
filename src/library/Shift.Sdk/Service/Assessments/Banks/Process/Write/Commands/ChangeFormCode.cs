using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormCode : Command
    {
        public Guid Form { get; set; }
        public string Code { get; set; }
        public string Source { get; set; }
        public string Origin { get; set; }

        public ChangeFormCode(Guid bank, Guid form, string code, string source, string origin)
        {
            AggregateIdentifier = bank;
            Form = form;
            Code = code.NullIfWhiteSpace();
            Source = source.NullIfWhiteSpace();
            Origin = origin.NullIfWhiteSpace();
        }
    }
}
