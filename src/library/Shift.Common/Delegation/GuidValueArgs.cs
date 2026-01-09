using System;

namespace Shift.Common.Events
{
    public delegate void GuidValueHandler(object sender, GuidValueArgs e);

    public class GuidValueArgs : EventArgs
    {
        public Guid Value { get; private set; }

        public GuidValueArgs(Guid value)
        {
            Value = value;
        }
    }
}