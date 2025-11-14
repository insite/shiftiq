using System;

using InSite.Persistence.Content;

using Shift.Common;

namespace InSite.Persistence
{
    public class CourseCatalogItem
    {
        public CourseCatalogItem(string title)
        {
            ItemTitle = title;
        }

        public Guid ItemIdentifier { get; set; }

        public string ItemType { get; set; }

        public string ItemTitle { get; set; }

        public string ItemRequirement { get; set; }

        public string ItemDescription { get; set; }

        public string ItemBadge
        {
            get
            {
                var isNew = Authored.HasValue && (DateTimeOffset.UtcNow - Authored.Value).TotalDays < 90;
                var isUpdated = Posted.HasValue && (DateTimeOffset.UtcNow - Posted.Value).TotalDays < 90;
                var isCustom = CourseFlagColor.HasValue() && CourseFlagText.HasValue();

                if (isNew)
                    return $"<span class='badge badge-floating badge-pill bg-success me-1'>New!</span>";
                else if (isCustom)
                    return $"<span class='badge badge-floating badge-pill bg-{CourseFlagColor} me-1'>{CourseFlagText}</span>";
                else if (isUpdated)
                    return $"<span class='badge badge-floating badge-pill bg-warning me-1'>Updated!</span>";

                return string.Empty;
            }
        }

        public Flag ItemFlag
        {
            get
            {
                var isNew = Authored.HasValue && (DateTimeOffset.UtcNow - Authored.Value).TotalDays < 90;
                var isUpdated = Posted.HasValue && (DateTimeOffset.UtcNow - Posted.Value).TotalDays < 90;

                var flag = new Flag();

                if (CourseFlagColor.HasValue() && CourseFlagText.HasValue())
                {
                    flag.Color = CourseFlagColor;
                    flag.Text = CourseFlagText;
                }
                else if (isNew)
                {
                    flag.Color = "success";
                    flag.Text = "New";
                }
                else if (isUpdated)
                {
                    flag.Color = "warning";
                    flag.Text = "Updated";
                }

                return flag;
            }
        }

        public int ItemPopularity { get; set; }

        public string ItemThumbnail
        {
            get
            {
                if (ThumbnailImageUrl.HasValue())
                    return $"<a class='card-img-top' href='{ItemStartUrl}'><img src='{ThumbnailImageUrl}' alt='{ItemTitle}'></a>";
                return string.Empty;
            }
        }

        public string ItemStartUrl
            => $"/ui/portal/learning/course/{ItemIdentifier}";

        public string ItemSubcategories { get; set; }

        public DateTimeOffset? Authored { get; set; }
        public DateTimeOffset? Posted { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string CourseFlagColor { get; set; }
        public string CourseFlagText { get; set; }
    }
}