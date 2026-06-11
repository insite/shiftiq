using System;

using InSite.Application.Sites.Read;

using Shift.Constant;

namespace InSite.Persistence.Content
{
    public class PortalPageModel
    {
        public PortalPageModel()
        {
            
        }

        public Guid Identifier { get; set; }
        
        public QPage Page { get; set; }

        public string Body { get; set; }
        public string Control { get; set; }
        public string Counter { get; set; }
        public string CssClass { get; set; }
        public string EditLinkUrl { get; set; }
        public string Icon { get; set; }
        public string IconHtml { get; set; }
        public string Language { get; set; }
        public string LastUpdated { get; set; }
        public string NavigateUrl { get; set; }
        public string ParentNavigateUrl { get; set; }
        public string ParentTitle { get; set; }
        public string Path { get; set; }
        public string Progress { get; set; }
        public string SupportUrl { get; set; }
        public string Title { get; set; }

        public void SetEditLinkUrl(bool isAuthenticated, bool isOperator, bool hasWriteAccess, string appUrl)
        {
            EditLinkUrl = string.Empty;

            if (Identifier == Guid.Empty)
                return;

            if (!isAuthenticated)
                return;

            if (Page?.OrganizationIdentifier == OrganizationIdentifiers.Global)
            {
                if (isOperator)
                {
                    EditLinkUrl = $"<a href='{appUrl}/ui/admin/sites/pages/outline?panel=content&id={Identifier}'><i class='far fa-edit'></i></a>";
                }
            }
            else if (hasWriteAccess)
            {
                EditLinkUrl = $"<a href='/ui/admin/sites/pages/outline?panel=content&id={Identifier}'><i class='far fa-edit'></i></a>";
            }
        }

        public void SetIconHtml()
        {
            var icon = Icon ?? "far fa-book-alt";
            var html = $"<i class='{icon} text-body-secondary mt-2 me-2'></i>";

            if (Progress == "Launched")
                html = "<i class='far fa-check text-success mt-2 me-2' title='Launched'></i>";

            else if (Progress == "Started")
                html = "<i class='far fa-hourglass text-info mt-2 me-2' title='Started'></i>";

            else if (Progress == "Completed")
                html = "<i class='far fa-check text-success mt-2 me-2' title='Completed'></i>";

            IconHtml = html;
        }
    }
}