using System;

namespace InSite.Domain.Foundations
{
    public interface ISiteOutline 
    {
        Guid Identifier { get; set; }
        Guid Organization { get; set; }
        string Domain { get; set; }
        PageTree Pages { get; set; }
    }
}
