using System;

namespace Shift.Common
{
    public interface ISearchReport
    {
        Guid? Identifier { get; set; }
        string Name { get; set; }
    }
}
