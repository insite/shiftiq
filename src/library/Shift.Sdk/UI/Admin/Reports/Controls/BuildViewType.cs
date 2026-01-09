using System;

namespace Shift.Sdk.UI
{
    public enum BuildViewType
    {
        Edit,
        View
    }

    public class BuildViewChangedArgs : EventArgs
    {
        public BuildViewType View { get; }

        public BuildViewChangedArgs(BuildViewType view)
        {
            View = view;
        }
    }

    public delegate void BuildViewChangedHandler(object sender, BuildViewChangedArgs args);
}