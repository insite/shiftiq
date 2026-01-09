using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract.Presentation
{
    public class PageSettings // Proposed rename = RouteSettings
    {
        public Guid ActionId { get; set; }
        public Guid? PageId { get; set; }
        public AboutModel CoreAbout { get; set; }
        public AboutModel CustomAbout { get; set; }
        public List<BreadcrumbModel> Breadcrumbs { get; set; }
        public bool DisplayCalendar { get; set; } = false;
        public string ActionTitle { get; set; }
        public bool FullWidth { get; set; } = true;
        public List<Error> Errors { get; set; } = new List<Error>();

        public DownloadSettings Download { get; set; }

        public PageSettings()
        {
            // TODO: Implement this for search forms.
            Download = MockDownloadSettings();
        }

        public void AddError(string text, int code)
        {
            Errors.Add(new Error(text, code));
        }

        private DownloadSettings MockDownloadSettings()
        {
            var settings = new DownloadSettings();

            settings.AddColumn("BillingAddressCountry");
            settings.AddColumn("UserLicenseAccepted", "Accepted Terms");
            settings.AddColumn("EmailVerified", "Verified Email");
            settings.AddColumn("UserPasswordChanged", "Successfully Reset Password", false);

            return settings;
        }

        public class BreadcrumbModel
        {
            public string Text { get; set; }
            public string Url { get; set; }
        }

        public class AboutModel
        {
            public string Heading { get; set; }
            public string Body { get; set; }
        }

        public class DownloadSettings
        {
            public List<DownloadColumnSettings> Columns { get; set; } = new List<DownloadColumnSettings>();

            internal void AddColumn(string name, string heading = null, bool? hide = false)
            {
                Columns.Add(new DownloadColumnSettings(name, heading, hide));
            }
        }

        public class DownloadColumnSettings
        {
            public DownloadColumnSettings(string name, string heading = null, bool? hide = false)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException(nameof(name));

                PhysicalName = name;
                LogicalHeading = heading ?? name.ToTitleCase();
                HideByDefault = hide ?? false;
            }

            /// <summary>
            /// The actual name of the column in the database table
            /// </summary>
            public string PhysicalName { get; set; }

            /// <summary>
            /// A logical user-friendly alias for the column name
            /// </summary>
            public string LogicalHeading { get; set; }

            /// <summary>
            /// True if this column is available but unselected by default for new downloads
            /// </summary>
            public bool HideByDefault { get; set; }
        }
    }
}