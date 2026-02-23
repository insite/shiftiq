namespace Shift.Contract
{
    internal static class AdminMenu
    {
        public static readonly NavigationList[] Groups = new NavigationList[]
        {
            new NavigationList
            {
                Title = "Communication",
                MenuItems = new NavigationItem[]
                {
                    new NavigationItem { Url = "/ui/admin/home", Icon = "grid-round", Title = "Toolkits (Apps)" },
                    new NavigationItem { Url = "/ui/admin/contacts/home", Icon = "file-chart-line", Title = "Contacts" },
                    new NavigationItem { Url = "/ui/admin/workflow/forms/home", Icon = "check-square", Title = "Forms (Surveys)" },
                    new NavigationItem { Url = "/ui/admin/messages/home", Icon = "paper-plane", Title = "Messages" },
                    new NavigationItem { Url = "/ui/admin/sites/home", Icon = "clouds", Title = "Sites" }
                }
            },
            new NavigationList
            {
                Title = "Learning",
                MenuItems = new NavigationItem[]
                {
                    new NavigationItem { Url = "/ui/admin/assessments/home", Icon = "balance-scale", Title = "Assessments" },
                    new NavigationItem { Url = "/ui/admin/courses/home", Icon = "users-class", Title = "Courses" },
                    new NavigationItem { Url = "/ui/admin/events/home", Icon = "calendar-alt", Title = "Events" },
                    new NavigationItem { Url = "/ui/admin/records/home", Icon = "pencil-ruler", Title = "Records" },
                    new NavigationItem { Url = "/ui/admin/standards/home", Icon = "ruler-triangle", Title = "Standards" },
                }
            },
            new NavigationList
            {
                Title = "Management",
                MenuItems = new NavigationItem[]
                {
                    new NavigationItem { Url = "/ui/admin/assets/home", Icon = "inventory", Title = "Assets" },
                    new NavigationItem { Url = "/ui/admin/reporting", Icon = "file-chart-line", Title = "Reports" },
                    new NavigationItem { Url = "/ui/admin/sales/home", Icon = "file-invoice-dollar", Title = "Sales" },
                    new NavigationItem { Url = "/ui/admin/workflow/home", Icon = "arrow-progress", Title = "Workflows" },
                }
            },
            new NavigationList
            {
                Title = "Utilities",
                MenuItems = new NavigationItem[]
                {
                    new NavigationItem { Url = "/ui/admin/setup/home", Icon = "cogs", Title = "Platform" },
                    new NavigationItem { Url = "/ui/admin/security/home", Icon = "shield", Title = "Security" },
                    new NavigationItem { Url = "/ui/admin/timeline/home", Icon = "timer", Title = "Timeline" },
                    new NavigationItem { Url = "/ui/admin/integration/home", Icon = "plug", Title = "Integration" },
                }
            }
        };
    }
}
