namespace Engine.Api.Metadata
{
    public class Upgrade
    {
        public Upgrade() { }

        public Upgrade(string name, DateTimeOffset executed, string data)
        {
            ScriptName = name;
            ScriptExecuted = executed;
            ScriptData = data;
        }

        public string ScriptName { get; set; } = null!;
        public DateTimeOffset ScriptExecuted { get; set; }
        public string ScriptData { get; set; } = null!;
    }
}