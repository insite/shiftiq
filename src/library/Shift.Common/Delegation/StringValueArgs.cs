using System;

namespace Shift.Common.Events
{
    public delegate void StringValueHandler(object sender, StringValueArgs e);

    public class StringValueArgs : EventArgs
    {
        public string Value { get; private set; }

        public StringValueArgs(string value)
        {
            Value = value;
        }
    }
}