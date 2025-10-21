using System;

using Shift.Common;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    [Serializable]
    internal class BreadcrumbDataItem : WebRoute
    {
        public BreadcrumbDataItem Parent { get; }

        public string NavigationParameters { get; }

        public BreadcrumbDataItem(BreadcrumbDataItem parent, string title, string name, string parameters)
        {
            Parent = parent;
            LinkTitle = title;
            Name = name;
            NavigationParameters = parameters;
        }

        public override IWebRoute GetParent() => Parent;
    }
}