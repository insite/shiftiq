using System;
using System.Web;
using System.Web.UI;

using InSite.Common.Web;

namespace InSite.UI.Layout.Admin
{
    /// <remarks>
    /// Alan wants a Dark theme mode, starting with header navigation. Dark mode now implemented for the Admin header 
    /// only, and can be more fully implemented in the next release. Note the standard convention is for Light mode to
    /// be the default.
    /// </remarks>
    public partial class ModeSwitch : UserControl
    {
        private const string CookieName = "Shift.UI.ThemeMode";

        private const int CookieLifetimeInDays = 90;

        private string _currentMode;

        public string CurrentMode
        {
            get
            {
                if (_currentMode == null)
                {
                    _currentMode = GetModeFromCookie();
                }
                return _currentMode ?? "Light";
            }
            set
            {
                _currentMode = value;

                SaveModeToCookie(value);
            }
        }

        public string IconOff { get; set; }

        public string IconOn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // On initial page load, determine current theme mode from cookie
                _currentMode = GetModeFromCookie();

                // Set initial checkbox state from current theme mode
                SetCheckboxFromMode();
            }

            // Wire up the checkbox change event for postback
            themeMode.ServerChange += ThemeMode_ServerChange;

            // Add JavaScript to trigger postback on checkbox click
            AddPostBackTrigger();

            if (IconOff != null)
                offIcon.Attributes["class"] = IconOff;

            if (IconOn != null)
                onIcon.Attributes["class"] = IconOn;
        }

        /// <summary>
        /// Adds JavaScript to trigger postback when checkbox is clicked
        /// </summary>
        private void AddPostBackTrigger()
        {
            var postBackScript = Page.ClientScript.GetPostBackEventReference(themeMode, string.Empty);

            themeMode.Attributes.Add("onclick", postBackScript);
        }

        /// <summary>
        /// Handles the checkbox state change event
        /// </summary>
        protected void ThemeMode_ServerChange(object sender, EventArgs e)
        {
            // Update CurrentState based on checkbox state (checked/unchecked)
            CurrentMode = themeMode.Checked ? "Dark" : "Light";

            // Redirect to update cookie on the client
            HttpResponseHelper.Redirect(Request.RawUrl);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (_currentMode != null)
            {
                // Ensure state is saved before rendering
                SaveModeToCookie(_currentMode);
            }
        }

        /// <summary>
        /// Loads current state value from cookie
        /// </summary>
        private string GetModeFromCookie()
        {
            try
            {
                HttpCookie cookie = Request.Cookies[CookieName];

                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    return HttpUtility.UrlDecode(cookie.Value);
                }
            }
            catch (Exception)
            {
                // Ignore unexpected errors for now
            }

            return null;
        }

        /// <summary>
        /// Saves the state value to a cookie
        /// </summary>
        /// <param name="mode">The state value to save</param>
        private void SaveModeToCookie(string mode)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(CookieName);

                if (!string.IsNullOrEmpty(mode))
                {
                    cookie.Value = HttpUtility.UrlEncode(mode);
                    cookie.Expires = DateTime.Now.AddDays(CookieLifetimeInDays);
                    cookie.HttpOnly = true; // Security: prevent client-side script access
                    cookie.SameSite = SameSiteMode.Lax; // CSRF protection
                    cookie.Domain = ServiceLocator.AppSettings.Security.Domain;
                }
                else
                {
                    // Clear the cookie by setting expiry to past date
                    cookie.Expires = DateTime.Now.AddDays(-1);
                }

                Response.Cookies.Add(cookie);
            }
            catch (Exception)
            {
                // Ignore unexpected errors for now
            }
        }

        /// <summary>
        /// Sets the checkbox state based on the current state value
        /// </summary>
        private void SetCheckboxFromMode()
        {
            if (!string.IsNullOrEmpty(_currentMode))
            {
                themeMode.Checked = _currentMode.Equals("Dark", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                themeMode.Checked = false; // Default mode is Light (box is unchecked)
                CurrentMode = "Light";
            }
        }

        /// <summary>
        /// Gets whether checkbox is currently checked
        /// </summary>
        public bool IsChecked
        {
            get { return themeMode.Checked; }
            set
            {
                themeMode.Checked = value;
                CurrentMode = value ? "Dark" : "Light";
            }
        }

        /// <summary>
        /// Clears current state and removes the cookie
        /// </summary>
        public void ClearState()
        {
            _currentMode = string.Empty;

            SaveModeToCookie(null);

            themeMode.Checked = true; // Reset to default
        }

        /// <summary>
        /// Refreshes the mode from the cookie (useful if cookie is modified externally)
        /// </summary>
        public void RefreshStateFromCookie()
        {
            _currentMode = GetModeFromCookie();
        }

        /// <summary>
        /// Gets the current theme mode from the current request
        /// </summary>
        public static string GetCurrentThemeMode()
        {
            var mode = "Light";

            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];

                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    var cookieValue = HttpUtility.UrlDecode(cookie.Value);

                    return cookieValue;
                }
            }
            catch (Exception)
            {
                // Ignore unexpected errors for now
            }

            return mode;
        }
    }
}
