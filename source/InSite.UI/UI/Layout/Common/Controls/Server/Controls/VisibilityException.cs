using System;

namespace InSite.Common.Web.UI
{
    /// <summary>
    /// Exception thrown when visibility operations fail
    /// </summary>
    public class VisibilityException : Exception
    {
        public VisibilityException(Type type)
            : this($"Control type '{type.Name}' is not supported for visibility management. Supported types: " +
                    "Button, BaseToggle, BaseControl, HtmlControl, UserControl, WebControl")
        {

        }
        public VisibilityException(string message) : base(message) { }
        public VisibilityException(string message, Exception innerException) : base(message, innerException) { }
    }
}