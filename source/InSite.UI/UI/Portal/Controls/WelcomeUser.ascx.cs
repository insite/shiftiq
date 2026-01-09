using System;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Controls
{
    public partial class WelcomeUser : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack || !Identity.IsAuthenticated)
                return;

            var time = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);
            TimeLiteral.Text = $"{time:dddd, MMMM d, yyyy}";

            var greeting = GetGreeting(time);
            Greeting.Text = $"{greeting}, {User.FirstName}";
        }

        private static string GetGreeting(DateTimeOffset time)
        {
            if (time.Hour > 3 && time.Hour < 12)
                return "Good Morning";

            if (time.Hour < 18)
                return "Good Afternoon";

            if (time.Hour < 23)
                return "Good Evening";

            return "Hello";
        }
    }
}