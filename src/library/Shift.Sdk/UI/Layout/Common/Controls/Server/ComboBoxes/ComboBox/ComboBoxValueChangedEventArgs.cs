using System;

namespace Shift.Sdk.UI
{
    public delegate void ComboBoxValueChangedEventHandler(object sender, ComboBoxValueChangedEventArgs e);

    public class ComboBoxValueChangedEventArgs : EventArgs
    {
        public string NewValue { get; }

        public string OldValue { get; }

        public ComboBoxValueChangedEventArgs(string newValue, string oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}