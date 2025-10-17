using System;

namespace Shift.Common.Events
{
    public delegate void IntValueHandler(object sender, IntValueArgs e);

    public class IntValueArgs : EventArgs
    {
        public int Value 
        { 
            get; 
            private set; 
        }

        public IntValueArgs(int value)
        {
            Value = value;
        }
    }
}