using InSite.Application.Organizations.Read;

namespace InSite.Application.Organizations.Write
{
    internal interface IHasValidation
    {
        void Validate(IOrganizationSearch search);
    }
}
