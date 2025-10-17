namespace Engine.Common
{
    internal class VersionResult
    {
        public required Version Version { get; set; }
        public required int Major { get; set; }
        public required int Minor { get; set; }
        public required int Build { get; set; }
        public required int Revision { get; set; }
    }
}
