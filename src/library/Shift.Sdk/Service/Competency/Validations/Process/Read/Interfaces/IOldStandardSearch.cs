using System;

namespace InSite.Application.Standards.Read
{
    public interface IOldStandardSearch
    {
        VStandard GetStandard(Guid standard);
        VCompetency GetCompetency(Guid standard);
        string GetCalculationMethod(Guid standard);
    }
}