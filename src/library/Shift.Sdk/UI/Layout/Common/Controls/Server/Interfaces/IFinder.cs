namespace Shift.Sdk.UI
{
    public interface IFinder
    {
        string EntityName { get; }
        ISearchResults Results { get; }
    }
}
