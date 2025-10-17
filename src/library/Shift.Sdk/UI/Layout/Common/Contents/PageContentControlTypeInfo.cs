namespace Shift.Sdk.UI
{
    public class PageContentControlTypeInfo
    {
        public int Sequence { get; }
        public string Name { get; }
        public string Title { get; }
        public string Path { get; }
        public bool IsPublic { get; }

        public PageContentControlTypeInfo(string name, string title, string path, int sequence, bool isPublic = true)
        {
            Sequence = sequence;
            Name = name;
            Title = title;
            Path = path;
            IsPublic = isPublic;
        }
    }
}