namespace Shift.Common
{
    public class CookieSettings
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Secret { get; set; }
        public int Lifetime { get; set; }
        public bool Encrypt { get; set; }
        public bool Debug { get; set; }
    }
}
