using System;

namespace Shift.Sdk.UI.Navigation
{
    public interface INavigationIdentity
    {
        string PartitionSlug { get; }
        bool IsAdministrator { get; }
        bool IsOperator { get; }

        bool IsInRole(string role);
        bool IsGranted(string resource);
    }
}
