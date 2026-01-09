namespace Shift.Common
{
    public class Difference
    {
        public string PropertyName { get; private set; }
        public object ValueBefore { get; private set; }
        public object ValueAfter { get; private set; }

        public Difference(string name, object before, object after)
        {
            PropertyName = name;
            ValueBefore = before;
            ValueAfter = after;
        }
    }
}