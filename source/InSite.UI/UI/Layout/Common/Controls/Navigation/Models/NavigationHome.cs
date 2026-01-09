namespace InSite.UI.Layout.Common.Controls.Navigation.Models
{
    public class NavigationHome
    {
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public string Href { get; set; }

        public NavigationHome Clone()
        {
            return new NavigationHome
            {
                Icon = this.Icon,
                Image = this.Image,
                Text = this.Text,
                Href = this.Href
            };
        }
    }
}