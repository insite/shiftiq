using System;
using System.Web;

using Shift.Common;

namespace InSite.Persistence.Content
{
    public class LaunchCard
    {
        /// <summary>
        /// Shown in place of a card image that is specified but cannot be found
        /// </summary>
        public const string PlaceholderImageUrl = "/UI/Layout/Portal/Images/CardPlaceholder.png";


        public Guid Identifier { get; set; }

        public int Sequence { get; set; }

        public string Category { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Indicator { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string BodyHtml { get; set; }
        public string Target { get; set; }
        public string Title { get; set; } = "Untitled";
        public string Url { get; set; }

        public bool Active { get; set; }
        public bool IsOverview { get; set; }

        public Flag Flag { get; set; }

        public Flag Progress { get; set; }

        public bool HasFlag() => !string.IsNullOrEmpty(Flag?.Color) && !string.IsNullOrEmpty(Flag?.Text);

        public bool HasIcon() => Icon.IsNotEmpty();

        public bool HasImage() => Image.IsNotEmpty();

        public bool HasProgress() => !string.IsNullOrEmpty(Progress?.Text);

        public string GetFlagHtml()
        {
            if (!HasFlag())
                return string.Empty;

            var color = Flag.Color.ToLower();
            var text = Flag.Text;
            var icon = Icon.IsNotEmpty() ? $"<i class='{Icon} me-2'></i>" : string.Empty;

            return $"<span class='badge badge-floating badge-pill bg-{color}'>{icon}{text}</span>";
        }

        public string GetIconHtml()
            => $"<span><i class='{Icon} fa-3x mb-3'></i></span>";

        /// <summary>
        /// Returns the image markup to render. A card image cannot be verified on the server: an
        /// application-relative URL such as "/files/orientations/cover.png" is rewritten to a
        /// handler that serves the file from tenant storage, so it never exists as a physical file
        /// under the application root. The specified URL is rendered as is, and the browser falls
        /// back to the placeholder when it cannot be loaded.
        /// </summary>
        public string GetImageHtml()
        {
            var value = Image?.Trim();

            var isPlaceholder = string.IsNullOrEmpty(value);

            // The placeholder is the only image whose dimensions are known here, so it is the only
            // one given an explicit size. The fallback applies the same size when it swaps a broken
            // image for the placeholder.

            var size = isPlaceholder ? " width=\"300\" height=\"200\"" : string.Empty;

            var src = HttpUtility.HtmlAttributeEncode(isPlaceholder ? PlaceholderImageUrl : value);
            var alt = HttpUtility.HtmlAttributeEncode(Title);

            var fallback = $"this.onerror=null;this.src='{PlaceholderImageUrl}';this.width=300;this.height=200;";

            return $"<img class=\"card-img-top\" src=\"{src}\" alt=\"{alt}\"{size} onerror=\"{fallback}\">";
        }

        public string GetProgressHtml()
        {
            if (!HasProgress())
                return string.Empty;

            Progress.Color = "Info";

            var text = Progress.Text;

            switch (text)
            {
                case "Started":
                    Icon = "fas fa-hourglass";
                    break;

                case "Completed":
                    Icon = "fas fa-check";
                    Progress.Color = "Success";
                    break;

                case "Expired":
                    Icon = "fas fa-alarm-clock";
                    Progress.Color = "Danger";
                    break;
            }

            var color = Progress.Color.ToLower();

            var icon = Icon.IsNotEmpty() ? $"<i class='{Icon} me-2'></i>" : string.Empty;

            return $"<span class='badge badge-pill bg-{color}'>{icon}{text}</span>";
        }
    }
}
