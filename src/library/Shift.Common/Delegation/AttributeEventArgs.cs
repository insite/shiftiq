using System;

namespace Shift.Common.Events
{
    public delegate void AttributeEventHandler(object sender, AttributeEventArgs e);

    public class AttributeEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public string Value { get; set; }
        public bool Cancel { get; set; }

        public AttributeEventArgs(string name, string value)
        {
            Name = name;
            Value = value;
            Cancel = false;
        }
    }
}