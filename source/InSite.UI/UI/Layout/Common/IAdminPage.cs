namespace InSite.Common.Web.UI
{
    public interface IAdminPage : IHasTranslator, IHasWebRoute
    {
        System.Web.UI.Control ActionControl { get; }
    }
}