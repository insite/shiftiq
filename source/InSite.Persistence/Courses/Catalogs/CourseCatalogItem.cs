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

        public bool IsPaidProgram => IsProgram && ProgramFee.HasValue && ProgramFee.Value > 0;

        // The badge shown on the card. A paid program shows its fee; everything else keeps the
        // New/Updated/custom flag.
        public Flag CardFlag
        {
            get
            {
                if (IsPaidProgram)
                    return new Flag { Color = "info", Text = ProgramFee.Value.ToString("C") };

                return ItemFlag;
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

        // A program card points at the pre-enrollment page, keyed by program identifier on the query
        // string. It does not use the legacy ui/portal/learning/program/{slug} route: that one resolves
        // a content page slug scoped to the signed-in organization, so it can neither take an identifier
        // nor reach a partition-owned program from another organization. Enrollment-state routing from
        // the card is a later concern. A course card points at the course route.
        public string ItemStartUrl
            => IsProgram
                ? $"/ui/portal/learning/programs/request?program={ItemIdentifier}"
                : $"/ui/portal/learning/course/{ItemIdentifier}";

        // A catalogue card can show an item owned by another organization: a partition-wide program
        // appears in every organization's grid. The stored image path is host-free, so the browser
        // resolves it against whichever subdomain the learner is browsing, and the file endpoint
        // takes its organization from that subdomain (CurrentSessionModule.Initialize) and refuses
        // a file owned by anyone else (StorageService.Authorize), so the image 404s. Point the
        // browser at the owning organization's host instead. A card owned by the organization
        // browsing it keeps the relative path it has always had, so nothing changes for the common
        // case.
        public static string ResolveImageUrl(string url, string owner, string viewer)
        {
            if (url.IsEmpty() || owner.IsEmpty() || StringHelper.Equals(owner, viewer))
                return url;

            // Only a root-relative path needs a host. Anything else is already qualified.
            if (!url.StartsWith("/"))
                return url;

            return ServiceLocator.Urls.GetApplicationUrl(owner) + url;
        }

        public string ItemSubcategories { get; set; }

        public DateTimeOffset? Authored { get; set; }
        public DateTimeOffset? Posted { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string CourseFlagColor { get; set; }
        public string CourseFlagText { get; set; }
        public bool IsOverview { get; set; }

        public bool IsProgram { get; set; }
        public decimal? ProgramFee { get; set; }
    }
}
