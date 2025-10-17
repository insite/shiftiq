using System;

namespace Shift.Sdk.UI
{
    public delegate void FindEntityValueChangedEventHandler(object sender, FindEntityValueChangedEventArgs e);

    public class FindEntityValueChangedEventArgs : EventArgs
    {
        public Guid[] NewValues { get; }

        public Guid[] OldValues { get; }

        public Guid? NewValue => _isSingleValue && NewValues.Length == 1 ? NewValues[0] : (Guid?)null;

        public Guid? OldValue => _isSingleValue && OldValues.Length == 1 ? OldValues[0] : (Guid?)null;

        private bool _isSingleValue;

        public FindEntityValueChangedEventArgs(Guid[] newValues, Guid[] oldValues, bool isSingleValue)
        {
            NewValues = newValues;
            OldValues = oldValues;

            _isSingleValue = isSingleValue;
        }
    }
}