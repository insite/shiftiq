namespace Shift.Sdk.UI.Help
{
    public class MarkdownSection
    {
        public string Path { get; set; }

        public string Body { get; set; }

        public MarkdownSection(string heading, string body)
        {
            Path = heading;

            Body = body;
        }

        public override string ToString()
        {
            return $"Heading: {Path}\nBody: {Body}\n";
        }
    }
}