using System;

using Shift.Common;

namespace InSite.Persistence.Content
{
    public class LaunchCard
    {
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

        public string GetImageHtml()
            => $"<img class='card-img-top' src='{Image}' alt='{Title}'>";

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