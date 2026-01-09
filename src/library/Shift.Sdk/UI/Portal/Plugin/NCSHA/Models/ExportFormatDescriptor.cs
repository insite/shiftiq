namespace Shift.Sdk.UI
{
    public class ExportFormatDescriptor
    {
        public string Id { get; }
        public string Icon { get; }
        public string Name { get; }
        public string Extension { get; }

        public ExportFormatDescriptor(string id, string icon, string name, string extension)
        {
            Id = id;
            Icon = icon;
            Name = name;
            Extension = extension;
        }
    }
}