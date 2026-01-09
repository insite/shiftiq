using System.Text;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    public class ProcessState
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ExecutionState Execution { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Indicator Indicator { get; set; }

        public string Description { get; set; }
        public string[] Errors { get; set; }
        public string[] Warnings { get; set; }

        public bool HasErrors => Errors.IsNotEmpty();
        public bool HasWarnings => Warnings.IsNotEmpty();

        public ProcessState() { }

        public ProcessState(ExecutionState execution) { Execution = execution; }

        public string ToText()
        {
            var text = new StringBuilder();
            text.Append(Execution);

            if (Description.HasValue())
                text.Append(Description);

            if (Indicator != Indicator.None)
                text.Append(Indicator);

            if (HasWarnings)
                foreach (var w in Warnings)
                    text.Append("Warning: " + w);

            if (HasErrors)
                foreach (var e in Errors)
                    text.Append("Error: " + e);

            return text.ToString();
        }

        public string ToHtml()
        {
            var icon = "far fa-" + GetIndicatorIcon(Indicator);
            var popover = $"role='button' data-toggle='popover' title='' data-content='{Description}'";
            var indicator = Indicator == Indicator.None || Indicator == Indicator.Default ? "custom-default" : Indicator.ToString().ToLower();

            string html = $"<span {popover} class='badge bg-{indicator}'><i class='{icon}'></i> {Execution}</span>";

            return html;
        }

        private static string GetIndicatorIcon(Indicator indicator)
        {
            switch (indicator)
            {
                case Indicator.Danger: return "bomb";
                case Indicator.Info: return "info-circle";
                case Indicator.Primary: return "circle";
                case Indicator.Success: return "check";
                case Indicator.Warning: return "exclamation-triangle";
                default: return "circle";
            }
        }

        public bool ShouldSerializeIndicator() => Indicator != Indicator.None;
        public bool ShouldSerializeExecution() => Execution != ExecutionState.Undefined;
        public bool ShouldSerializeDescription() => !string.IsNullOrEmpty(Description);
        public bool ShouldSerializeErrors() => HasErrors;
        public bool ShouldSerializeWarnings() => HasWarnings;
        public bool ShouldSerializeHasErrors() => false;
        public bool ShouldSerializeHasWarnings() => false;
    }
}
