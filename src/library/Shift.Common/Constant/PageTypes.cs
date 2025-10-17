using System.Linq;

namespace Shift.Common
{
    public static class PageTypes
    {
        private static string[] _types = new[] { "Block", "Folder", "Page", "Template" };

        public static bool Exists(string type) 
            => _types.Any(x => x == type);

        public static string[] GetAll() 
            => _types;
    }
}