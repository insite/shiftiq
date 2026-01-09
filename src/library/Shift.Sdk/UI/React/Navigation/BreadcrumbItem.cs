using System;

namespace Shift.Contract
{
    [Serializable]
    public class BreadcrumbItem
    {
        public BreadcrumbItem() { }

        public BreadcrumbItem(string text, string href, string icon = null, string active = null)
        {
            Text = text;
            Href = href;
            Icon = icon;
            Active = active;
        }

        public string Text { get; set; }
        public string Href { get; set; }
        public string Icon { get; set; }
        public string Active { get; set; }

        public string Anchor
        {
            get
            {
                if (Href == null)
                    return Text;

                return $"<a href={Href}>{Text}</a>";
            }
        }

        public string CssClass { get; set; }
        public bool IsActive { get; set; }
        public string NavigateUrl { get; set; }
        public string Target { get; set; }
        public bool Visible { get; set; } = true;

        public string IconClass
        {
            get
            {
                return !string.IsNullOrEmpty(_iconClass)
                    ? _iconClass
                    : !string.IsNullOrEmpty(Icon)
                        ? "fas fa-" + Icon
                        : null;
            }
            set
            {
                _iconClass = value;
            }
        }

        private string _iconClass;
    }
}