namespace Engine.Api.Metadata
{
    public class TUpgrade
    {
        public string? ScriptData { get; set; }
        public string ScriptName { get; set; } = null!;

        public DateTimeOffset ScriptExecuted { get; set; }
    }
}