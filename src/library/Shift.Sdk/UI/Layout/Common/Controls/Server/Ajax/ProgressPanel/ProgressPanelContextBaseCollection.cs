using System.IO;

namespace Shift.Sdk.UI
{
    public abstract class ProgressPanelContextBaseCollection
    {
        public abstract int Count { get; }

        internal abstract void ToJson(TextWriter output);
    }
}