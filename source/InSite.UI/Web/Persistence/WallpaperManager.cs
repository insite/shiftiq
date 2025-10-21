using System;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.Common.Web
{
    public sealed class WallpaperManager
    {
        private const string ImageFolder = "/ui/lobby/images";
        private const string ShiftLogo = ImageFolder + "/shift.png";
        private const string CmdsLogo = ImageFolder + "/cmds.png";

        public static string GetCoverUrl(string customCover, string organizationCode)
        {
            var environment = ServiceLocator.AppSettings.Environment;

            var cover = $"{ImageFolder}/{environment.Name}.jpg";

            try
            {
                if (customCover.IsNotEmpty())
                {
                    customCover = ImageHelper.CreateAbsoluteUrl(customCover, ServiceLocator.AppSettings, organizationCode);

                    if (ImageHelper.Exists(new Uri(customCover)))
                        return customCover;
                }

                var isE03 = ServiceLocator.Partition.IsE03();

                if (isE03)
                {
                    const string cmdsCover = "/ui/lobby/images/signin-1.jpg";
                    return cmdsCover; // Add more images before doing this in v25.5 --> .Replace("1", GetCoverIndex().ToString());
                }
            }
            catch (Exception ex)
            {
                AppSentry.SentryWarning($"{ex.Message} -- The organization {organizationCode} is configured with an invalid cover image URL. Please forward this information to the account administrator.");
            }

            return cover;
        }

        public static string GetLogoUrl(string customLogo, string organizationCode)
        {
            var logo = ServiceLocator.Partition.IsE03() ? CmdsLogo : ShiftLogo;

            try
            {
                if (customLogo.IsEmpty())
                    customLogo = logo;

                customLogo = ImageHelper.CreateAbsoluteUrl(customLogo, ServiceLocator.AppSettings, organizationCode);

                if (ImageHelper.Exists(new Uri(customLogo)))
                    return customLogo;
            }
            catch (Exception ex)
            {
                AppSentry.SentryWarning($"{ex.Message} -- The organization {organizationCode} is configured with an invalid logo image URL. Please forward this information to the account administrator.");
            }

            return logo;
        }

        public static int GetCoverIndex()
        {
            const int AvailableImageCount = 4;

            var day = DateTimeOffset.Now.Day;

            return ((day - 1) % AvailableImageCount) + 1;
        }
    }
}