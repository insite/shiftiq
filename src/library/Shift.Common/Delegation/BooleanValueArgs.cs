using System;

namespace Shift.Common.Events
{
    public delegate void BooleanValueHandler(object sender, BooleanValueArgs args);

    public class BooleanValueArgs : EventArgs
    {
        public bool Value { get; private set; }

        public BooleanValueArgs(bool value)
        {
            Value = value;
        }
    }
}