using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace InSite.Common.Web.UI
{
    /// <remarks>
    /// The built-in Control.Visible property cannot be trusted because Microsoft implemented a recursive getter. This
    /// means the property value of a parent control overrides the property value of a child control, which is not
    /// expected or intuitive, and this breaks any logic that assumes the visibility of a parent control can be 
    /// decided after the visibility of its child controls has been determined using business rules. (I suspect the
    /// developers at Microsoft originally intended for the Visible property to be a read-only on the control hierarchy. 
    /// Regardless, we consider it unsafe, and it should be avoided.)
    /// 
    /// This helper class uses Bootstrap's d-none class exclusively to show/hide supported controls. Notice it does NOT
    /// get or set Visible (or IsHidden) property values. This is by design, to ensure this helper can be used where it
    /// is needed with no side-effects on any existing functionality related to visibility and rendering.
    /// </remarks>
    public class ControlVisibilityHelper
    {
        /// <summary>
        /// Hides a control by adding Bootstrap's d-none class
        /// </summary>
        /// <param name="control">The control to hide</param>
        public void Hide(Control control)
        {
            if (control == null)
                return;

            if (control is UserControl userControl)
            {
                AddUserControlClass(userControl, "d-none");
            }
            else if (control is WebControl webControl)
            {
                AddCssClass(webControl, "d-none");
            }
            else if (control is HtmlControl htmlControl)
            {
                AddHtmlClass(htmlControl, "d-none");
            }
            else if (control is BaseControl baseControl)
            {
                AddHtmlClass(baseControl.Attributes, "d-none");
            }
            else if (control is BaseToggle toggleControl)
            {
                AddHtmlClass(toggleControl.Attributes, "d-none");
            }
            else if (control is Button buttonControl)
            {
                buttonControl.CssClass = AddCssClass(buttonControl.CssClass, "d-none");
                AddHtmlClass(buttonControl.Attributes, "d-none");
            }
            else
            {
                throw new VisibilityException(control.GetType());
            }
        }

        /// <summary>
        /// Shows a control by removing Bootstrap's d-none class
        /// </summary>
        /// <param name="control">The control to show</param>
        public void Show(Control control)
        {
            if (control == null)
                return;

            if (control is UserControl userControl)
            {
                RemoveUserControlClass(userControl, "d-none");
            }
            else if (control is WebControl webControl)
            {
                RemoveCssClass(webControl, "d-none");
            }
            else if (control is HtmlControl htmlControl)
            {
                RemoveHtmlClass(htmlControl, "d-none");
            }
            else if (control is BaseControl baseControl)
            {
                RemoveHtmlClass(baseControl.Attributes, "d-none");
            }
            else if (control is BaseToggle toggleControl)
            {
                RemoveHtmlClass(toggleControl.Attributes, "d-none");
            }
            else if (control is Button buttonControl)
            {
                buttonControl.CssClass = RemoveCssClass(buttonControl.CssClass, "d-none");
                RemoveHtmlClass(buttonControl.Attributes, "d-none");
            }
            else
            {
                throw new VisibilityException(control.GetType());
            }
        }

        /// <summary>
        /// Toggles control visibility using Bootstrap's d-none class
        /// </summary>
        /// <param name="control">The control to toggle</param>
        /// <param name="show">True to show, false to hide</param>
        public void Toggle(Control control, bool show)
        {
            if (show)
                Show(control);
            else
                Hide(control);
        }

        /// <summary>
        /// Checks if a control is currently hidden via Bootstrap's d-none class
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <returns>True if the control appears to be hidden</returns>
        public bool IsHidden(Control control)
        {
            if (control == null)
                return true;

            if (control is UserControl userControl)
            {
                string cssClass = GetUserControlClass(userControl);
                return !string.IsNullOrEmpty(cssClass) && cssClass.Contains("d-none");
            }
            else if (control is WebControl webControl)
            {
                return !string.IsNullOrEmpty(webControl.CssClass) && webControl.CssClass.Contains("d-none");
            }
            else if (control is HtmlControl htmlControl)
            {
                string cssClass = htmlControl.Attributes["class"];
                return !string.IsNullOrEmpty(cssClass) && cssClass.Contains("d-none");
            }
            else if (control is BaseControl baseControl)
            {
                string cssClass = baseControl.Attributes["class"];
                return !string.IsNullOrEmpty(cssClass) && cssClass.Contains("d-none");
            }
            else if (control is BaseToggle toggleControl)
            {
                string cssClass = toggleControl.Attributes["class"];
                return !string.IsNullOrEmpty(cssClass) && cssClass.Contains("d-none");
            }
            else if (control is Button buttonControl)
            {
                string cssClass = buttonControl.Attributes["class"];
                return !string.IsNullOrEmpty(cssClass) && cssClass.Contains("d-none");
            }
            else
            {
                throw new VisibilityException(control.GetType());
            }
        }

        /// <summary>
        /// Batch operation to hide multiple controls
        /// </summary>
        /// <param name="controls">Array of controls to hide</param>
        public void Hide(Control[] controls)
        {
            if (controls == null) return;

            foreach (var control in controls)
            {
                Hide(control);
            }
        }

        /// <summary>
        /// Batch operation to show multiple controls
        /// </summary>
        /// <param name="controls">Array of controls to show</param>
        public void Show(Control[] controls)
        {
            if (controls == null) return;

            foreach (var control in controls)
            {
                Show(control);
            }
        }

        /// <summary>
        /// Advanced visibility management with conditional rules
        /// </summary>
        /// <param name="control">The control to manage</param>
        /// <param name="condition">Function that returns true if control should be visible</param>
        public void SetVisibility(Control control, Func<bool> condition)
        {
            if (control == null || condition == null)
                return;

            try
            {
                bool shouldShow = condition();
                Toggle(control, shouldShow);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occurred setting the visibility of {control.UniqueID}. " +
                    $"The control will be hidden by default and an exception will be thrown. {ex.Message}";

                Hide(control);
                throw new VisibilityException(message, ex);
            }
        }

        #region AttributeCollection Helper Methods

        private void AddHtmlClass(System.Web.UI.AttributeCollection attributes, string cssClass)
        {
            string currentClass = attributes["class"] ?? string.Empty;
            if (!currentClass.Contains(cssClass))
            {
                attributes["class"] = (currentClass.Trim() + " " + cssClass).Trim();
            }
        }

        private void RemoveHtmlClass(System.Web.UI.AttributeCollection attributes, string cssClass)
        {
            string currentClass = attributes["class"];
            if (!string.IsNullOrEmpty(currentClass))
            {
                attributes["class"] = RemoveCssClass(currentClass, cssClass);
            }
        }

        private void AddHtmlClass(AttributeCollection attributes, string cssClass)
        {
            string currentClass = attributes["class"] ?? string.Empty;
            if (!currentClass.Contains(cssClass))
            {
                attributes["class"] = (currentClass.Trim() + " " + cssClass).Trim();
            }
        }

        private void RemoveHtmlClass(AttributeCollection attributes, string cssClass)
        {
            string currentClass = attributes["class"];
            if (!string.IsNullOrEmpty(currentClass))
            {
                attributes["class"] = RemoveCssClass(currentClass, cssClass);
            }
        }

        private void AddHtmlClass(Dictionary<string, string> attributes, string cssClass)
        {
            if (attributes.TryGetValue("class", out string currentClass))
            {
                if (!currentClass.Contains(cssClass))
                {
                    attributes["class"] = (currentClass.Trim() + " " + cssClass).Trim();
                }
            }
        }

        private void RemoveHtmlClass(Dictionary<string, string> attributes, string cssClass)
        {
            if (attributes.TryGetValue("class", out string currentClass))
            {
                if (!string.IsNullOrEmpty(currentClass))
                {
                    attributes["class"] = RemoveCssClass(currentClass, cssClass);
                }
            }
        }

        #endregion

        #region HtmlControl Helper Methods

        private void AddHtmlClass(HtmlControl htmlControl, string cssClass)
        {
            string currentClass = htmlControl.Attributes["class"] ?? string.Empty;
            if (!currentClass.Contains(cssClass))
            {
                htmlControl.Attributes["class"] = (currentClass.Trim() + " " + cssClass).Trim();
            }
        }

        private void RemoveHtmlClass(HtmlControl htmlControl, string cssClass)
        {
            string currentClass = htmlControl.Attributes["class"];
            if (!string.IsNullOrEmpty(currentClass))
            {
                htmlControl.Attributes["class"] = RemoveCssClass(currentClass, cssClass);
            }
        }

        #endregion

        #region UserControl Helper Methods

        private void AddUserControlClass(UserControl userControl, string cssClass)
        {
            var wrapper = userControl.FindControl("Wrapper") as HtmlControl;
            if (wrapper == null)
            {
                throw new VisibilityException($"UserControl '{userControl.GetType().Name}' must contain a div with id='Wrapper' and runat='server' for visibility management.");
            }

            AddHtmlClass(wrapper, cssClass);
        }

        private void RemoveUserControlClass(UserControl userControl, string cssClass)
        {
            var wrapper = userControl.FindControl("Wrapper") as HtmlControl;
            if (wrapper == null)
            {
                throw new VisibilityException($"UserControl '{userControl.GetType().Name}' must contain a div with id='Wrapper' and runat='server' for visibility management.");
            }

            RemoveHtmlClass(wrapper, cssClass);
        }

        private string GetUserControlClass(UserControl userControl)
        {
            var wrapper = userControl.FindControl("Wrapper") as HtmlControl;
            if (wrapper == null)
            {
                throw new VisibilityException($"UserControl '{userControl.GetType().Name}' must contain a div with id='Wrapper' and runat='server' for visibility management.");
            }

            return wrapper.Attributes["class"] ?? string.Empty;
        }

        #endregion

        #region WebControl Helper Methods

        private void AddCssClass(WebControl webControl, string cssClass)
        {
            if (string.IsNullOrEmpty(webControl.CssClass))
            {
                webControl.CssClass = cssClass;
            }
            else if (!webControl.CssClass.Contains(cssClass))
            {
                webControl.CssClass = (webControl.CssClass.Trim() + " " + cssClass).Trim();
            }
        }

        private void RemoveCssClass(WebControl webControl, string cssClass)
        {
            if (!string.IsNullOrEmpty(webControl.CssClass))
            {
                webControl.CssClass = RemoveCssClass(webControl.CssClass, cssClass);
            }
        }

        #endregion

        #region Utility Methods

        private string AddCssClass(string cssClass, string classToAdd)
        {
            if (string.IsNullOrEmpty(cssClass))
                return classToAdd;

            // Add the specified class and clean up whitespace
            string cleaned = (cssClass + " " + classToAdd)
                .Replace("  ", " ")  // Replace double spaces
                .Trim();

            return cleaned;
        }

        /// <summary>
        /// Cleans CSS class string by removing specified class and extra whitespace
        /// </summary>
        /// <param name="cssClass">The CSS class string to clean</param>
        /// <param name="classToRemove">The specific class to remove</param>
        /// <returns>Cleaned CSS class string</returns>
        private string RemoveCssClass(string cssClass, string classToRemove)
        {
            if (string.IsNullOrEmpty(cssClass))
                return string.Empty;

            // Remove the specified class and clean up whitespace
            string cleaned = cssClass.Replace(classToRemove, "")
                                     .Replace("  ", " ")  // Replace double spaces
                                     .Trim();

            return cleaned;
        }

        #endregion
    }
}