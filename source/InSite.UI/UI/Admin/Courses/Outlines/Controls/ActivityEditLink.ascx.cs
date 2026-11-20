using System;
using System.Linq;
using System.Web;

using InSite.Application.Courses.Read;
using InSite.Web.Integration;

using Shift.Common;
using Shift.Common.Integration.Scoop;
using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditLink : BaseActivityEdit
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);

            LinkType.AutoPostBack = true;
            LinkType.ValueChanged += (sender, args) => LinkTypeSelected();

            LinkUrl.AutoPostBack = true;
            LinkUrl.TextChanged += (sender, args) => SetNavigationUrl(LinkType.Value, LinkUrl.Text);

            // Ticking the box changes what the badge means, from one package to a set of language
            // variants, so it rebinds against whatever is in the textbox now.
            LinkIsMultilingual.AutoPostBack = true;
            LinkIsMultilingual.CheckedChanged += (sender, args) =>
                BindScoopPackageStatus(ActivityPlatform.Value, ActivityHook.Text, LinkIsMultilingual.Checked);
        }

        // The Scoop organization that owns this activity's packages, kept in ViewState so the
        // checkbox postback above needs no second lookup.
        private string ScoopTenantHandle
        {
            get { return ViewState["ScoopTenantHandle"] as string; }
            set { ViewState["ScoopTenantHandle"] = value; }
        }

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        protected override void BindModelToControls(QActivity activity)
        {
            LinkType.Value = activity.ActivityUrlType;
            ActivityPlatform.Value = activity.ActivityPlatform;
            ActivityHook.Text = activity.ActivityHook;
            LinkIsMultilingual.Checked = activity.ActivityIsMultilingual;

            // The OWNER of the course, not the signed-in organization. For a partition-hosted
            // global module these differ, and a badge built on the signed-in one would report
            // "not found" for shared packages.
            ScoopTenantHandle = ScoopPackageResolver.GetTenantHandle(activity.OrganizationIdentifier);

            BindScoopPackageStatus(activity.ActivityPlatform, activity.ActivityHook, activity.ActivityIsMultilingual);
            SetNavigationUrl(activity);
            LinkTarget.Value = activity.ActivityUrlTarget ?? "_blank";
            LinkTypeSelected();
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            activity.ActivityUrlType = LinkType.Value;
            activity.ActivityPlatform = ActivityPlatform.Value;
            activity.ActivityHook = ReadPackageId(activity.ActivityUrlType, activity.ActivityPlatform);
            activity.ActivityUrl = LinkUrl.Text;
            activity.ActivityUrlTarget = LinkTarget.Value;

            // Guarded on platform, not just on the checkbox: a hidden CheckBox renders nothing and
            // posts nothing, so Checked keeps whatever ViewState held. The guard also clears the
            // flag left behind on activities that were multilingual under SCORM Cloud, whenever one
            // of them is saved from this screen.
            activity.ActivityIsMultilingual =
                activity.ActivityUrlType == "SCORM"
                && activity.ActivityPlatform == "Scoop"
                && LinkIsMultilingual.Checked;

            if (activity.ActivityUrlType == "SCORM")
            {
                var url = $"/ui/portal/integrations/scorm/launch/{activity.ActivityIdentifier}";
                if (activity.ActivityUrl == null || !activity.ActivityUrl.StartsWith(url))
                    activity.ActivityUrl = url;

                // Scoop always renders its own UI in-tab with a return-to-Shift button. Persist
                // _self so the stored target reflects reality regardless of the "Open in a new
                // window or tab" checkbox state.

                if (activity.ActivityPlatform == "Scoop")
                    activity.ActivityUrlTarget = "_self";
            }

            if (activity.ActivityUrlType == "External" &&
                !activity.ActivityUrl.StartsWith("http://") && !activity.ActivityUrl.StartsWith("https://") && !activity.ActivityUrl.StartsWith("mailto:"))
            {
                activity.ActivityUrl = $"https://{activity.ActivityUrl}";
            }
        }

        // The column holds 100 characters. The textbox allows more, so that a pasted Scoop link
        // reaches the server whole: the browser used to cut a pasted enrol link at 100 characters,
        // and what was left was saved as the Package ID.
        private const int MaxPackageIdLength = 100;

        // A Scoop Package ID is stored as the handle alone, taken from a pasted link when that is
        // what was entered, and anything that is still not a handle is refused here rather than
        // sent to Scoop by every badge and every launch.
        private string ReadPackageId(string linkType, string platform)
        {
            var packageId = ActivityHook.Text;

            var isScoop = linkType == "SCORM" && platform == "Scoop";
            if (isScoop)
            {
                packageId = ScoopPackageNaming.ParseHandle(packageId);

                var isEmpty = packageId.IsEmpty();
                var isHandle = ScoopPackageNaming.IsHandle(packageId);
                if (!isEmpty && !isHandle)
                    throw new ApplicationError("SCORM Package ID must be a Scoop package handle: letters, digits and single hyphens, for example i-100-intro.");
            }

            if (packageId != null && packageId.Length > MaxPackageIdLength)
                throw new ApplicationError($"SCORM Package ID can be at most {MaxPackageIdLength} characters.");

            return packageId;
        }

        // Says what Scoop actually holds under the entered Package ID, so a typo or a package
        // that was never uploaded shows up here rather than when a learner opens the lesson.
        // Advisory only: it reflects the SAVED Package ID until the next save, because the textbox
        // has no AutoPostBack and adding one would fire a postback on every blur while an
        // administrator types an ID.
        private void BindScoopPackageStatus(string platform, string packageId, bool isMultilingual)
        {
            ScormPackageStatus.Text = string.Empty;

            try
            {
                if (!StringHelper.Equals(platform, "Scoop"))
                    return;

                if (packageId.IsEmpty() || ScoopTenantHandle.IsEmpty())
                    return;

                // Scoop would only answer a 400, so say so without asking. This is how an ID
                // saved before the check in ReadPackageId shows up.
                var isHandle = ScoopPackageNaming.IsHandle(packageId);
                if (!isHandle)
                {
                    ScormPackageStatus.Text = Badge("danger", "Not a package handle");
                    return;
                }

                // An administrator who has just uploaded a package should see
                // it at once, so the badge ignores the cache and warms it for
                // the learners who follow.
                var packages = ScoopPackageResolver.GetPackages(ScoopTenantHandle, packageId, true);

                if (!packages.IsAvailable)
                    return;

                ScormPackageStatus.Text = isMultilingual
                    ? DescribeVariants(packages, packageId)
                    : DescribeSinglePackage(packages, packageId);
            }
            catch (Exception ex)
            {
                // A badge is never worth a broken edit screen.
                AppSentry.CaptureException(ex);
            }
        }

        private static string DescribeSinglePackage(ScoopPackageSet packages, string packageId)
        {
            var course = packages.Courses.FirstOrDefault(c => StringHelper.Equals(c.Handle, packageId));

            if (course == null)
                return Badge("danger", "Not found in Scoop");

            var detail = course.Standard.IsEmpty() ? "Found" : $"Found - {course.Standard.ToUpperInvariant()}";

            if (course.UploadedAt.HasValue)
                detail += $" uploaded {course.UploadedAt.Value:yyyy-MM-dd}";

            return Badge("success", detail);
        }

        private static string DescribeVariants(ScoopPackageSet packages, string packageId)
        {
            var variants = packages.Courses
                .Where(c => !StringHelper.Equals(c.Handle, packageId))
                .Select(c => c.Variant)
                .Where(v => !v.IsEmpty())
                .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (variants.Count == 0)
                return Badge("danger", "No language variants found");

            var handles = string.Join(", ", packages.Courses.Select(c => c.Handle));

            var badges = string.Join(" ", variants.Select(v => Badge("success", v.ToLowerInvariant(), handles)));

            // The fallback rule leans on the English package, so its absence is
            // worth saying out loud rather than leaving to be discovered.
            var hasEnglish = variants.Any(v => StringHelper.Equals(v, ScoopPackageNaming.FallbackLanguage));

            return hasEnglish ? badges : badges + " " + Badge("warning", "No English fallback");
        }

        private static string Badge(string variant, string text, string title = null)
        {
            var tooltip = title.IsEmpty() ? string.Empty : $" title='{HttpUtility.HtmlAttributeEncode(title)}'";

            return $"<span class='badge bg-{variant} fs-sm'{tooltip}>{HttpUtility.HtmlEncode(text)}</span>";
        }

        private void SetNavigationUrl(string linkType, string url)
        {
            if (linkType == "External")
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("mailto:"))
                    LinkUrlTest.NavigateUrl = $"https://{url}";
                else
                    LinkUrlTest.NavigateUrl = url;
            }
            else if (linkType == "Internal")
            {
                LinkUrlTest.NavigateUrl = $"{Request.Url.Scheme}://{Request.Url.Host}{url}";
            }
        }

        private void SetNavigationUrl(QActivity activity)
        {
            var hasUrl = !string.IsNullOrEmpty(activity.ActivityUrl);

            LinkUrl.Text = activity.ActivityUrl;
            LinkUrlTest.Visible = hasUrl;
            if (hasUrl)
            {
                SetNavigationUrl(activity.ActivityUrlType, activity.ActivityUrl);
            }
        }

        private void LinkTypeSelected()
        {
            var isScorm = LinkType.Value == "SCORM";

            ActivityPlatform.Visible = isScorm;

            var isScoop = isScorm && ActivityPlatform.Value == "Scoop";

            ActivityHookField.Visible = isScorm;

            LinkIsMultilingual.Visible = isScoop;

            LinkUrlField.Visible = !isScorm;

            if (isScoop)
            {
                var linkGenerator = new ScoopLinkGenerator();

                var scoopUrl = linkGenerator.GenerateLibraryUrl(Identity, Request.Url.Host, Organization.Code, null, Request.RawUrl);

                ScoopLibraryUrl.Text = $"{Organization.Name} SCO Library";

                ScoopLibraryUrl.NavigateUrl = scoopUrl;

                ScoopLibraryUrl.Visible = true;

                ScoopLibraryUrl.Target = "_blank";

                // Hide-and-coerce: the Link Target combo above is hidden for Scoop, so pin its
                // ViewState value to the same _self that BindControlsToModel persists.

                LinkTarget.Value = "_self";
            }

            SetNavigationUrl(LinkType.Value, LinkUrl.Text);
        }
    }
}
