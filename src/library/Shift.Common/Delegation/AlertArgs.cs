using System;

using Shift.Constant;

namespace Shift.Common.Events
{
    public delegate void AlertHandler(object sender, AlertArgs args);

    public class AlertArgs : EventArgs
    {
        public AlertType Type { get; }

        public string Icon { get; }

        public string Text { get; }

        public AlertArgs(AlertType type, string text)
        {
            Type = type;
            Text = text;
        }

        public AlertArgs(AlertType type, string icon, string text)
        {
            Type = type;
            Icon = icon;
            Text = text;
        }
    }
}