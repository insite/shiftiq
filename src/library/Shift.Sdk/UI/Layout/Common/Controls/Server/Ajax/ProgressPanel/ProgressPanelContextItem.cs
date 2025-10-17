using System.IO;

namespace Shift.Sdk.UI
{
    public abstract class ProgressPanelContextItem
    {
        public string Id { get; }

        public ProgressPanelContextItem(string id)
        {
            Id = id;
        }

        internal void ToJson(TextWriter output)
        {
            output.Write($"\"{Id}\":");

            WriteJson(output);
        }

        protected abstract void WriteJson(TextWriter output);
    }
}
