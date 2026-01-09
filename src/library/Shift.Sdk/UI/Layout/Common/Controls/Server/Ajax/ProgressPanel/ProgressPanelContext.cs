using System.IO;

namespace Shift.Sdk.UI
{
    public abstract class ProgressPanelContext
    {
        public bool IsEmpty => Items.Count == 0 && Variables.Count == 0;

        public bool IsComplete { get; set; }
        public bool IsCancelled { get; set; }

        public ProgressPanelContextItemCollection Items { get; protected set; }

        public ProgressPanelContextVariableCollection Variables { get; } = new ProgressPanelContextVariableCollection();

        public void ToJson(TextWriter output)
        {
            output.Write("{");

            var isEmpty = true;

            if (IsComplete)
                WriteProperty("complete", "true");

            if (IsCancelled)
                WriteProperty("cancelled", "true");

            WriteCollection("items", Items);
            WriteCollection("variables", Variables);

            output.Write("}");

            void WriteProperty(string propName, string propValue)
            {
                if (!isEmpty)
                    output.Write(",");

                output.Write($"\"{propName}\":{propValue}");

                isEmpty = false;
            }

            void WriteCollection(string propName, ProgressPanelContextBaseCollection collection)
            {
                if (collection.Count == 0)
                    return;

                if (!isEmpty)
                    output.Write(",");

                output.Write($"\"{propName}\":");

                collection.ToJson(output);

                isEmpty = false;
            }
        }
    }
}
